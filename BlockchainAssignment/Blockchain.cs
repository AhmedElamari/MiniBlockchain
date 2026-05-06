using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAssignment
{
    public class Blockchain
    {
        private const float DifficultyTolerance = 0.001f;
        private readonly List<Block> blocks = new List<Block>();
        private int transactionsPerBlock = 5;
        public List<Transaction> transactionPool = new List<Transaction>();

        private readonly List<Validator> validators = new List<Validator>();
        private readonly Random random = new Random();

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
            {
                throw new InvalidOperationException("Total validator stake must be greater than zero.");
            }

            decimal roll = (decimal)random.NextDouble() * totalStake;
            decimal runningTotal = 0;

            foreach (Validator validator in validators)
            {
                runningTotal += validator.stake;
                if (roll <= runningTotal)
                {
                    return validator;
                }
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
                return false;

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

            if (block.consensusType == "ProofOfStake")
            {
                if (string.IsNullOrWhiteSpace(block.validatorAddress) || !validators.Any(v => v.publicKey == block.validatorAddress && v.stake > 0))
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

        public List<Transaction> getPendingTransactionsPool()
        {
            return transactionPool.ToList();
        }

        public void addPendingTransaction(Transaction transaction)
        {
            transactionPool.Add(transaction);
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
