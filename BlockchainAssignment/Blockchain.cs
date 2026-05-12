using BlockchainAssignment.HashCode;
using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BlockchainAssignment
{
    public class Blockchain
    {
        private const float DifficultyTolerance = 0.001f;
        private const decimal SlashPercentagePerOffense = 0.10m;
        private const decimal StakeScaleDecimal = 1000000m;
        private readonly List<Block> blocks = new List<Block>();
        private int transactionsPerBlock = 5;
        public List<Transaction> transactionPool = new List<Transaction>();

        private readonly List<Validator> validators = new List<Validator>();

        public Blockchain()
        {
            Block genesisBlock = new Block();
            blocks.Add(genesisBlock);
        }

        public void addValidator(string publicKey, decimal stake, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                errorMessage = "Public key is required.";
                return;
            }

            string key = publicKey.Trim();
            if (validators.Any(v => v.publicKey == key))
            {
                errorMessage = "A validator with this public key is already registered.";
                return;
            }

            if (stake <= 0)
            {
                errorMessage = "Stake must be greater than zero.";
                return;
            }

            decimal confirmed = getLastBlock().checkBalance(blocks, key);
            decimal pendingOutgoing = transactionPool
                .Where(t => t.sender == key && t.sender != Transaction.miningRewardSenderID)
                .Sum(t => t.amount + t.fee);
            decimal available = confirmed - pendingOutgoing;

            if (stake > available)
            {
                errorMessage = "Stake exceeds available balance (" + available + "). Mine or receive coins for this wallet first.";
                return;
            }

            validators.Add(new Validator(key, stake));
        }

        public List<Validator> getValidators()
        {
            return validators.ToList();
        }

        public Validator SelectValidator()
        {
            if (validators.Count == 0)
                return null;

            decimal totalStake = validators.Sum(v => v.stake);
            if (totalStake <= 0)
                throw new InvalidOperationException("Total validator stake must be greater than zero.");

            Block previous = getLastBlock();
            BigInteger hashInt = HexHashToUnsignedBigInteger(previous.Hash);

            BigInteger totalScaled = BigInteger.Zero;
            foreach (Validator v in validators)
                totalScaled += StakeScaled(v.stake);

            if (totalScaled.IsZero)
                throw new InvalidOperationException("Total validator stake must be greater than zero.");

            BigInteger roll = hashInt % totalScaled;

            BigInteger runningScaled = BigInteger.Zero;
            foreach (Validator validator in validators)
            {
                BigInteger w = StakeScaled(validator.stake);
                runningScaled += w;
                if (roll < runningScaled)
                    return validator;
            }

            return validators.Last();
        }

        public string returnBlockchain(int blockIndex)
        {
            Block b = blocks.FirstOrDefault(x => x.Index == blockIndex);
            return b != null ? b.ToString() : "Block not found.";
        }

        public Block getLastBlock()
        {
            return blocks[blocks.Count - 1];
        }

        public List<Block> getBlocks()
        {
            return blocks;
        }

        public float getDifficultyForNextBlock()
        {
            return AdaptiveDifficulty.GetNextDifficulty(blocks);
        }

        public bool addBlock(Block block, out string failureMessage)
        {
            failureMessage = null;
            if (block == null)
            {
                failureMessage = "Block is null.";
                return false;
            }

            if (blocks.Count == 0)
            {
                failureMessage = "Chain has no genesis block.";
                return false;
            }

            Block previous = getLastBlock();
            float expectedDifficulty = getDifficultyForNextBlock();
            if (!validateNonGenesisBlock(block, previous, expectedDifficulty, out failureMessage))
            {
                TrySlashProofOfStakeRejection(block, ref failureMessage);
                return false;
            }

            blocks.Add(block);

            if (block.consensusType == "ProofOfStake")
            {
                Validator forger = validators.FirstOrDefault(v => v.publicKey == block.validatorAddress);
                if (forger != null)
                    forger.IncrementBlocksForged();
            }

            foreach (Transaction transaction in block.transactionList.Where(t => t.sender != Transaction.miningRewardSenderID))
            {
                transactionPool.Remove(transaction);
            }

            return true;
        }

        public bool validateBlockchain(out string message)
        {
            if (blocks.Count == 0)
            {
                message = "Chain is empty.";
                return false;
            }

            for (int i = 1; i < blocks.Count; i++)
            {
                Block current = blocks[i];
                Block prev = blocks[i - 1];
                float expectedDifficulty = AdaptiveDifficulty.GetNextDifficulty(blocks, i);
                string fail;
                if (!validateNonGenesisBlock(current, prev, expectedDifficulty, out fail))
                {
                    message = fail;
                    return false;
                }
            }

            message = "All blocks are valid.";
            return true;
        }

        private bool validateNonGenesisBlock(Block block, Block previous, float expectedDifficulty, out string failureMessage)
        {
            failureMessage = null;
            if (block.previousHash != previous.Hash)
            {
                failureMessage = "Block " + block.Index + " has invalid previous hash.";
                return false;
            }

            if (block.Index != previous.Index + 1)
            {
                failureMessage = "Block " + block.Index + " has invalid index.";
                return false;
            }

            if (Math.Abs(block.difficulty - expectedDifficulty) > DifficultyTolerance)
            {
                failureMessage = "Block " + block.Index + " has unexpected difficulty. Expected " + expectedDifficulty + " but was " + block.difficulty + ".";
                return false;
            }

            if (block.merikleRoot != Block.MerkleRoot(block.transactionList))
            {
                failureMessage = "Block " + block.Index + " has invalid Merkle root.";
                return false;
            }

            if (block.consensusType == "ProofOfStake")
            {
                if (string.IsNullOrWhiteSpace(block.validatorAddress) || !validators.Any(v => v.publicKey == block.validatorAddress))
                {
                    failureMessage = "Block " + block.Index + " has an invalid proof-of-stake validator.";
                    return false;
                }

                string expectedProof = previous.Hash + block.timeStamp.ToString("O") + block.validatorAddress;
                if (block.selectionProof != expectedProof)
                {
                    failureMessage = "Block " + block.Index + " has an invalid proof-of-stake selection proof.";
                    return false;
                }

                Transaction rewardTx = block.transactionList.FirstOrDefault(t => t.sender == Transaction.miningRewardSenderID);
                if (rewardTx == null || rewardTx.recipient != block.validatorAddress)
                {
                    failureMessage = "Block " + block.Index + " mining reward must pay the proof-of-stake validator.";
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(block.Hash) || !AdaptiveDifficulty.HashMeetsDifficulty(block.Hash, block.difficulty))
                {
                    failureMessage = "Block " + block.Index + " does not satisfy proof of work.";
                    return false;
                }
            }

            if (block.Hash != block.createHash())
            {
                failureMessage = "Block " + block.Index + " hash does not match computed hash.";
                return false;
            }

            return true;
        }

        private static BigInteger StakeScaled(decimal stake)
        {
            decimal scaled = decimal.Truncate(stake * StakeScaleDecimal);
            if (scaled < 0m)
                scaled = 0m;
            BigInteger result = new BigInteger(scaled);
            if (stake > 0m && result.IsZero)
                result = BigInteger.One;
            return result;
        }

        private static BigInteger HexHashToUnsignedBigInteger(string hexHash)
        {
            if (string.IsNullOrWhiteSpace(hexHash))
                throw new ArgumentException("Hash cannot be empty.", "hexHash");

            byte[] be = HashTools.StringToByteArray(hexHash.Trim());
            if (be.Length == 0)
                throw new ArgumentException("Hash cannot be decoded.", "hexHash");

            byte[] le = new byte[be.Length + 1];
            for (int i = 0; i < be.Length; i++)
                le[i] = be[be.Length - 1 - i];

            return new BigInteger(le);
        }

        private void TrySlashProofOfStakeRejection(Block block, ref string failureMessage)
        {
            if (block == null || block.consensusType != "ProofOfStake")
                return;

            string addr = block.validatorAddress == null ? string.Empty : block.validatorAddress.Trim();
            if (string.IsNullOrWhiteSpace(addr))
                return;

            Validator offender = validators.FirstOrDefault(v => v.publicKey == addr);
            if (offender == null)
                return;

            failureMessage = failureMessage ?? string.Empty;

            Validator expected = null;
            try
            {
                expected = SelectValidator();
            }
            catch (InvalidOperationException)
            {
                return;
            }

            if (expected == null || expected.publicKey != addr)
                return;

            offender.IncrementPenalties();
            decimal slashAmount = offender.stake * SlashPercentagePerOffense;
            offender.SlashStake(slashAmount);

            int p = offender.penalties;
            decimal stakeRemaining = offender.stake;
            string pk = offender.publicKey;

            failureMessage += " Slashing applied to validator " + pk + ": penalties=" + p
                + ", stake after slash=" + stakeRemaining + ".";
        }

        public List<Transaction> getPendingTransactionsPool()
        {
            return transactionPool.ToList();
        }

        public void addPendingTransaction(Transaction transaction)
        {
            transactionPool.Add(transaction);
        }

        public bool tryAddPendingTransaction(Transaction transaction, out string failureMessage)
        {
            failureMessage = null;
            if (transaction == null || !transaction.isValid())
            {
                failureMessage = "Invalid transaction (check keys, amount, fee, and signature).";
                return false;
            }

            decimal required = transaction.amount + transaction.fee;
            decimal available = getSpendableBalance(transaction.sender);
            if (transaction.sender != Transaction.miningRewardSenderID && required > available)
            {
                failureMessage = "Insufficient funds. Available balance: " + available + ", required: " + required + ".";
                return false;
            }

            transactionPool.Add(transaction);
            return true;
        }

        private decimal getSpendableBalance(string walletAddress)
        {
            if (string.IsNullOrWhiteSpace(walletAddress))
                return 0;

            decimal confirmedBalance = getLastBlock().checkBalance(blocks, walletAddress);
            decimal pendingOutgoing = transactionPool
                .Where(t => t.sender == walletAddress && t.sender != Transaction.miningRewardSenderID)
                .Sum(t => t.amount + t.fee);

            decimal lockedStake = validators.Where(v => v.publicKey == walletAddress).Sum(v => v.stake);

            return confirmedBalance - pendingOutgoing - lockedStake;
        }

        public List<Transaction> getTransactionsForNextBlock(int transactionsPerBlock)
        {
            int n = Math.Min(transactionsPerBlock, transactionPool.Count);
            return transactionPool.Take(n).ToList();
        }

        public List<Transaction> getTransactionsForNextBlock(int transactionsPerBlock, MiningPolicy miningPolicy, string minerAddress)
        {
            string miner = (minerAddress ?? string.Empty).Trim();
            IEnumerable<Transaction> filteredPool = transactionPool;

            switch (miningPolicy)
            {
                case MiningPolicy.FirstComeFirstServe:
                    break;
                case MiningPolicy.HighestFeeFirst:
                    filteredPool = filteredPool.OrderByDescending(t => t.fee);
                    break;
                case MiningPolicy.LongestWait:
                    filteredPool = filteredPool.OrderBy(t => t.timestamp);
                    break;
                case MiningPolicy.Random:
                    filteredPool = filteredPool.OrderBy(t => Guid.NewGuid());
                    break;
                case MiningPolicy.AddressPreferential:
                    {
                        IEnumerable<Transaction> preferred = filteredPool.Where(t => t.recipient == miner);
                        IEnumerable<Transaction> rest = filteredPool.Where(t => t.recipient != miner).OrderBy(t => t.timestamp);
                        filteredPool = preferred.Concat(rest);
                        break;
                    }
                default:
                    filteredPool = filteredPool.OrderBy(t => t.timestamp);
                    break;
            }
            return filteredPool.Take(transactionsPerBlock).ToList();
        }

        public int getTransactionsPerBlock()
        {
            return transactionsPerBlock;
        }

        public enum MiningPolicy
        {
            FirstComeFirstServe,
            HighestFeeFirst,
            LongestWait,
            Random,
            AddressPreferential
        }
    }
}
