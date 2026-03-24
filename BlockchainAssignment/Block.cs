using BlockchainAssignment.HashCode;
using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    public class Block
    {
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
        {
            if (lastBlock == null) throw new ArgumentNullException(nameof(lastBlock));

            this.timeStamp = DateTime.Now;
            this.Index = lastBlock.Index + 1;
            this.previousHash = lastBlock.Hash;
            this.transactionList = transactions ?? new List<Transaction>();
            this.nonce = 0;
            this.minerAddress = minerRewardAddress;
            rewardMiner();
            this.merikleRoot = MerkleRoot(this.transactionList);
            this.Hash = Mine();

        }



        public string createHash()
        {
            SHA256 hasher = SHA256Managed.Create();
            string input = Index.ToString() + timeStamp.ToString() + previousHash + nonce + difficulty + reward + merikleRoot;
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            String hash = string.Empty;
            foreach (byte x in hashByte)
                hash += String.Format("{0:x2}", x);
            return hash;
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
                   "\n Merical Root: " + merikleRoot;
        }

        public string Mine()
        {
            string target = new string('0', (int)difficulty);
            Hash = createHash();
            while (!Hash.StartsWith(target))
            {
                nonce++;

                Hash = createHash();
            }
            return Hash;
        }

        public string rewardMiner()
        {
           decimal fees = transactionList.Where(t => t.sender != Transaction.miningRewardSenderID).Sum(t => t.fee);
            Transaction rewardTransaction = new Transaction("", Transaction.miningRewardSenderID, minerAddress, reward + fees, 0);
            transactionList.Add(rewardTransaction); 
            return rewardTransaction.ToString();
        }

        public void verifyBlock(List<Block> blocks)
        {
            if (blocks == null)
            {
                throw new ArgumentNullException(nameof(blocks));
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
                    if (transaction.recipient == walletAddress)
                    {
                        balance += transaction.amount;
                    }
                }
                if (currentBlock.minerAddress == walletAddress)
                {
                    balance += currentBlock.reward + currentBlock.transactionList.Sum(t => t.fee);
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