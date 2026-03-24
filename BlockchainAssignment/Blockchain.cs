using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAssignment
{
    public class Blockchain
    {
        private readonly List<Block> blocks = new List<Block>();
        private int transactionsPerBlock = 5;
        public List<Transaction> transactionPool = new List<Transaction>();

        public Blockchain()
        {
            Block genesisBlock = new Block();
            blocks.Add(genesisBlock);
        }

        public string ReturnBlockchain(Block block)
        {
            return block.ToString();
        }

        public Block GetLastBlock()
        {
            return blocks[blocks.Count - 1];
        }

        public List<Block> GetBlocks()
        {
            return blocks;
        }

        public bool AddBlock(Block block, out string failureMessage)
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

            Block previous = GetLastBlock();
            if (!validateNonGenesisBlock(block, previous, out failureMessage))
                return false;

            blocks.Add(block);

            foreach (Transaction transaction in block.transactionList.Where(t => t.sender != Transaction.miningRewardSenderID))
            {
                transactionPool.Remove(transaction);
            }

            return true;
        }

        public List<Transaction> GetPendingTransactionsPool()
        {
            return transactionPool.ToList();
        }

        public List<Transaction> GetTransactionsForNextBlock()
        {
            return GetTransactionsForNextBlock(transactionsPerBlock);
        }

        public List<Transaction> GetTransactionsForNextBlock(int transactionsPerBlock)
        {
            int n = Math.Min(transactionsPerBlock, transactionPool.Count);
            return transactionPool.Take(n).ToList();
        }

        public bool IsValidBlockchain(out string message)
        {
            List<Block> chain = GetBlocks();
            if (chain.Count == 0)
            {
                message = "Blockchain has no blocks.";
                return false;
            }

            if (!isValidGenesisBlock(chain[0], out message))
                return false;

            for (int i = 1; i < chain.Count; i++)
            {
                if (!validateNonGenesisBlock(chain[i], chain[i - 1], out message))
                    return false;
            }

            message = "Blockchain is valid.";
            return true;
        }

        private static bool isValidGenesisBlock(Block block, out string message)
        {
            message = null;
            if (block.Index != 0)
            {
                message = "Genesis block: wrong index.";
                return false;
            }

            if (!string.IsNullOrEmpty(block.previousHash))
            {
                message = "Genesis block: previous hash should be empty.";
                return false;
            }

            if (!isPOWValid(block, out message))
                return false;

            string expectedMerkle = Block.MerkleRoot(block.transactionList);
            if ((block.merikleRoot ?? "") != (expectedMerkle ?? ""))
            {
                message = "Genesis block: bad Merkle root.";
                return false;
            }

            foreach (Transaction tx in block.transactionList)
            {
                if (!tx.isValid())
                {
                    message = "Genesis block: invalid transaction.";
                    return false;
                }
            }

            return true;
        }

        private static bool validateNonGenesisBlock(Block block, Block previous, out string message)
        {
            message = null;

            if (block.Index != previous.Index + 1)
            {
                message = "Non-Genesis block " + block.Index + ": wrong index.";
                return false;
            }

            if (block.previousHash != previous.Hash)
            {
                message = "Non-Genesis block " + block.Index + ": bad previous hash.";
                return false;
            }

            if (!isPOWValid(block, out message))
                return false;

            string expectedMerkle = Block.MerkleRoot(block.transactionList);
            if ((block.merikleRoot ?? "") != (expectedMerkle ?? ""))
            {
                message = "Non-Genesis block " + block.Index + ": bad Merkle root.";
                return false;
            }

            foreach (Transaction tx in block.transactionList)
            {
                if (!tx.isValid())
                {
                    message = "Non-Genesis block " + block.Index + ": invalid transaction.";
                    return false;
                }
            }

            List<Transaction> rewardTxs = block.transactionList.Where(t => t.sender == Transaction.miningRewardSenderID).ToList();
            if (rewardTxs.Count != 1)
            {
                message = "Non-Genesis block " + block.Index + ": need exactly one mining reward transaction.";
                return false;
            }

            Transaction rewardTx = rewardTxs[0];
            decimal nonRewardFees = block.transactionList.Where(t => t.sender != Transaction.miningRewardSenderID).Sum(t => t.fee);
            decimal expectedRewardAmount = block.reward + nonRewardFees;
            if (rewardTx.amount != expectedRewardAmount)
            {
                message = "Non-Genesis block " + block.Index + ": wrong reward amount.";
                return false;
            }

            if (rewardTx.recipient != block.minerAddress)
            {
                message = "Non-Genesis block " + block.Index + ": reward payee must be miner.";
                return false;
            }

            return true;
        }

        private static bool isPOWValid(Block block, out string message)
        {
            message = null;
            string recomputed = block.createHash();
            if (block.Hash != recomputed)
            {
                message = "Block " + block.Index + ": hash mismatch.";
                return false;
            }

            string target = new string('0', (int)block.difficulty);
            if (block.Hash == null || !block.Hash.StartsWith(target))
            {
                message = "Block " + block.Index + ": proof of work failed.";
                return false;
            }

            return true;
        }
    }
}
