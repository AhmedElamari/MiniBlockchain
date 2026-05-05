using BlockchainAssignment.HashCode;
using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAssignment
{
    public class Blockchain
    {
        private const float DifficultyTolerance = 0.001f;
        private readonly List<Block> blocks = new List<Block>();
        private int transactionsPerBlock = 5;
        public List<Transaction> transactionPool = new List<Transaction>();

        private readonly List<Validator> pendingValidators = new List<Validator>();

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
            if (ContainsValidatorKey(key))
            {
                errorMessage = "A validator with this public key is already registered.";
                return;
            }

            if (stake <= 0)
            {
                errorMessage = "Stake must be greater than zero.";
                return;
            }

            pendingValidators.Add(new Validator(key, stake));
        }

        public List<Validator> getValidators()
        {
            var map = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
            Block tip = getLastBlock();
            for (int i = 1; i <= tip.Index && i < blocks.Count; i++)
                ApplyValidatorRegistryLines(map, blocks[i].validatorRegistryUpdates);

            foreach (Validator v in pendingValidators)
                map[v.publicKey] = v.stake;

            return map.Select(kv => new Validator(kv.Key, kv.Value)).OrderBy(v => v.publicKey, StringComparer.Ordinal).ToList();
        }

        public string FormatPendingValidatorRegistryCanonical()
        {
            if (pendingValidators.Count == 0)
                return string.Empty;

            var sorted = pendingValidators.OrderBy(v => v.publicKey, StringComparer.Ordinal).ToList();
            var sb = new StringBuilder();
            foreach (Validator v in sorted)
            {
                sb.Append(v.publicKey);
                sb.Append('|');
                sb.Append(v.stake.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        public bool TryComputeProofOfStakeProposer(Block lastBlock, string canonicalRegistryUpdatesForNewBlock, out Validator selectedValidator, out string selectionProofHex, out string errorMessage)
        {
            selectedValidator = null;
            selectionProofHex = string.Empty;
            errorMessage = null;

            List<Validator> validators = BuildValidatorsForProofOfStake(lastBlock, canonicalRegistryUpdatesForNewBlock);
            if (validators.Count == 0)
            {
                errorMessage = "No validators registered (pending registrations are committed when the next block is forged).";
                return false;
            }

            selectedValidator = SelectProofOfStakeValidator(lastBlock.Hash, lastBlock.Index + 1, validators, out selectionProofHex);
            if (selectedValidator == null)
            {
                errorMessage = "Could not select a proof-of-stake validator.";
                return false;
            }

            return true;
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
            pendingValidators.Clear();

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
                List<Validator> validatorsForLottery = BuildValidatorsForProofOfStake(previous, block.validatorRegistryUpdates ?? string.Empty);
                Validator expectedValidator = SelectProofOfStakeValidator(previous.Hash, block.Index, validatorsForLottery, out string expectedProofHex);
                if (expectedValidator == null || string.IsNullOrWhiteSpace(block.validatorAddress) || expectedValidator.publicKey != block.validatorAddress)
                {
                    failureMessage = "Block " + block.Index + " does not match deterministic proof-of-stake validator selection.";
                    return false;
                }

                if (!string.Equals(block.selectionProof ?? string.Empty, expectedProofHex ?? string.Empty, StringComparison.Ordinal))
                {
                    failureMessage = "Block " + block.Index + " has an invalid proof-of-stake selection proof.";
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

        private bool ContainsValidatorKey(string key)
        {
            foreach (Validator v in EnumerateCommittedValidators())
            {
                if (v.publicKey == key)
                    return true;
            }

            return pendingValidators.Any(v => v.publicKey == key);
        }

        private IEnumerable<Validator> EnumerateCommittedValidators()
        {
            Block tip = getLastBlock();
            var map = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
            for (int i = 1; i <= tip.Index && i < blocks.Count; i++)
                ApplyValidatorRegistryLines(map, blocks[i].validatorRegistryUpdates);

            foreach (KeyValuePair<string, decimal> kv in map)
                yield return new Validator(kv.Key, kv.Value);
        }

        private List<Validator> BuildValidatorsForProofOfStake(Block previousBlock, string registryUpdatesForNewBlock)
        {
            var map = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
            for (int i = 1; i <= previousBlock.Index && i < blocks.Count; i++)
                ApplyValidatorRegistryLines(map, blocks[i].validatorRegistryUpdates);

            ApplyValidatorRegistryLines(map, registryUpdatesForNewBlock ?? string.Empty);

            return map.Select(kv => new Validator(kv.Key, kv.Value)).OrderBy(v => v.publicKey, StringComparer.Ordinal).ToList();
        }

        private static void ApplyValidatorRegistryLines(IDictionary<string, decimal> map, string lines)
        {
            if (string.IsNullOrWhiteSpace(lines))
                return;

            foreach (string raw in lines.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int sep = raw.LastIndexOf('|');
                if (sep <= 0 || sep >= raw.Length - 1)
                    continue;

                string pk = raw.Substring(0, sep).Trim();
                string stakeStr = raw.Substring(sep + 1).Trim();
                if (!decimal.TryParse(stakeStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal stake))
                    continue;

                if (string.IsNullOrWhiteSpace(pk) || stake <= 0)
                    continue;

                map[pk] = stake;
            }
        }

        private static Validator SelectProofOfStakeValidator(string previousHash, int nextBlockIndex, IList<Validator> validators, out string selectionProofSeedHex)
        {
            selectionProofSeedHex = string.Empty;
            if (validators == null || validators.Count == 0)
                return null;

            List<Validator> ordered = validators.OrderBy(v => v.publicKey, StringComparer.Ordinal).ToList();
            decimal total = ordered.Sum(v => v.stake);
            if (total <= 0)
                return null;

            var sb = new StringBuilder();
            sb.Append(previousHash ?? string.Empty);
            sb.Append('|');
            sb.Append(nextBlockIndex.ToString(CultureInfo.InvariantCulture));
            sb.Append('|');
            foreach (Validator v in ordered)
            {
                sb.Append(v.publicKey);
                sb.Append('=');
                sb.Append(v.stake.ToString(CultureInfo.InvariantCulture));
                sb.Append(';');
            }

            byte[] seed;
            using (SHA256 sha = SHA256.Create())
                seed = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            selectionProofSeedHex = HashTools.ByteArrayToString(seed);

            ulong roll = BitConverter.ToUInt64(seed, 0);
            decimal ratio = (decimal)roll / ((decimal)ulong.MaxValue + 1m);
            decimal target = ratio * total;

            decimal cumulative = 0;
            foreach (Validator v in ordered)
            {
                cumulative += v.stake;
                if (target < cumulative)
                    return v;
            }

            return ordered[ordered.Count - 1];
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
