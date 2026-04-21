using BlockchainAssignment.HashCode;
using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlockchainAssignment
{
    public class Block
    {
        public delegate void MiningMessageCallback(string message);

        public DateTime timeStamp { get; set; }
        public int Index { get; set; }
        public string previousHash, Hash;
        public List<Transaction> transactionList = new List<Transaction>();
        public int nonce;
        public float difficulty = 4;
        public string minerAddress;
        public decimal reward = 10;
        public string merikleRoot;

        public Block()
        {
            this.timeStamp = DateTime.Now;
            this.Index = 0;
            this.previousHash = string.Empty;
            this.nonce = 0;
            this.merikleRoot = string.Empty;
            this.minerAddress = string.Empty;
            this.reward = 0;
            this.merikleRoot = MerkleRoot(transactionList);
            this.Hash = Mine();
        }

        public Block(Block lastBlock, List<Transaction> transactions, string minerRewardAddress)
            : this(lastBlock, transactions, minerRewardAddress, DateTime.Now, 4f, true)
        {
        }

        public Block(Block lastBlock, List<Transaction> transactions, string minerRewardAddress, DateTime timeStamp, float difficulty, bool autoMine)
        {
            if (lastBlock == null) throw new ArgumentNullException("lastBlock");

            this.timeStamp = timeStamp;
            this.Index = lastBlock.Index + 1;
            this.previousHash = lastBlock.Hash;
            this.transactionList = new List<Transaction>(transactions ?? new List<Transaction>());
            this.nonce = 0;
            this.minerAddress = minerRewardAddress ?? string.Empty;
            this.difficulty = difficulty;
            rewardMiner(this.timeStamp);
            this.merikleRoot = MerkleRoot(this.transactionList);
            this.Hash = autoMine ? Mine() : string.Empty;
        }

        public static Block CreateUnminedCandidate(Block lastBlock, List<Transaction> transactions, string minerRewardAddress, DateTime timeStamp, float difficulty)
        {
            return new Block(lastBlock, transactions, minerRewardAddress, timeStamp, difficulty, false);
        }

        public string createHash()
        {
            return CreateHashForNonce(nonce);
        }

        public string CreateHashForNonce(int nonceValue)
        {
            using (SHA256 hasher = SHA256.Create())
                return HashTools.ByteArrayToString(HashBytesForNonce(hasher, nonceValue));
        }

        private byte[] HashBytesForNonce(SHA256 hasher, int nonceValue)
        {
            string input = Index.ToString() + timeStamp.ToString() + previousHash + nonceValue + difficulty + reward + merikleRoot;
            return hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public override string ToString()
        {
            return "Block Index: " + Index +
                   ", TimeStamp: " + timeStamp.ToString("O") +
                   ",\n Hash: " + Hash +
                   ",\n PreviousHash: " + previousHash +
                   "\n Transactions: " + string.Join("\n", transactionList.Select(t => t.ToString())) +
                   "\n Nonce: " + nonce % 1000 +
                   "\n Difficulty: " + difficulty +
                   "\n Reward: " + reward +
                   "\n Merkle Root: " + merikleRoot;
        }

        public string Mine()
        {
            return Mine(Environment.ProcessorCount, null);
        }

        public string Mine(int workerCount, MiningMessageCallback callback = null)
        {
            if (workerCount < 1)
                workerCount = 1;

            if (callback != null)
                callback("Mining (" + workerCount + " thread(s))...");

            object winnerLock = new object();
            string winningHash = null;
            int winningNonce = 0;
            int stopMining = 0;
            byte[] targetBytes = AdaptiveDifficulty.GetTargetBytes(difficulty);

            Thread[] threads = new Thread[workerCount];
            for (int i = 0; i < workerCount; i++)
            {
                int workerIndex = i;
                threads[i] = new Thread(() =>
                {
                    using (SHA256 hasher = SHA256.Create())
                    {
                        int n = workerIndex;
                        while (Volatile.Read(ref stopMining) == 0)
                        {
                            byte[] hashBytes = HashBytesForNonce(hasher, n);
                            if (AdaptiveDifficulty.HashMeetsDifficulty(hashBytes, targetBytes))
                            {
                                string hash = HashTools.ByteArrayToString(hashBytes);
                                lock (winnerLock)
                                {
                                    if (winningHash == null)
                                    {
                                        winningNonce = n;
                                        winningHash = hash;
                                        Volatile.Write(ref stopMining, 1);
                                    }
                                }
                                break;
                            }
                            n += workerCount;
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads)
                t.Join();

            this.nonce = winningNonce;
            this.Hash = winningHash ?? string.Empty;

            if (callback != null)
                callback("Done. Nonce " + winningNonce + ".");

            return this.Hash;
        }

        public string rewardMiner(DateTime blockTimestamp)
        {
            decimal fees = transactionList.Where(t => t.sender != Transaction.miningRewardSenderID).Sum(t => t.fee);
            Transaction rewardTransaction = new Transaction("", Transaction.miningRewardSenderID, minerAddress, reward + fees, 0, blockTimestamp);
            transactionList.Add(rewardTransaction);
            return rewardTransaction.ToString();
        }

        public void verifyBlock(List<Block> blocks)
        {
            if (blocks == null)
            {
                throw new ArgumentNullException("blocks");
            }

            for (int i = 1; i < blocks.Count; i++)
            {
                Block currentBlock = blocks[i];
                Block previousBlock = blocks[i - 1];

                if (currentBlock.previousHash != previousBlock.Hash)
                {
                    MessageBox.Show("Block " + currentBlock.Index + " has invalid previous hash.");
                    return;
                }
            }

            MessageBox.Show("All blocks are valid.");
        }

        public decimal checkBalance(List<Block> blocks, string walletAddress)
        {
            decimal balance = 0;
            foreach (Block currentBlock in blocks)
            {
                foreach (Transaction transaction in currentBlock.transactionList)
                {
                    if (transaction.sender == walletAddress)
                    {
                        balance -= transaction.amount + transaction.fee;
                    }
                    else if (transaction.recipient == walletAddress)
                    {
                        balance += transaction.amount;
                    }
                }
            }
            return balance;
        }

        public static string MerkleRoot(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                return string.Empty;
            }
            List<string> hashes = transactions.Select(t => t.createHashTransaction()).ToList();
            while (hashes.Count > 1)
            {
                List<string> newHashes = new List<string>();
                for (int i = 0; i < hashes.Count; i += 2)
                {
                    string left = hashes[i];
                    string right = (i + 1 < hashes.Count) ? hashes[i + 1] : left;
                    string combinedHash = HashTools.CombineHash(left, right);
                    newHashes.Add(combinedHash);
                }
                hashes = newHashes;
            }
            return hashes[0];
        }
    }
}
