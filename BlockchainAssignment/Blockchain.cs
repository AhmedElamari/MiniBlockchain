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
            if (!validateNonGenesisBlock(block, previous, out failureMessage))
                return false;

            blocks.Add(block);

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
                if (!validateNonGenesisBlock(current, prev, out string fail))
                {
                    message = fail;
                    return false;
                }
            }

            message = "All blocks are valid.";
            return true;
        }

        private static bool validateNonGenesisBlock(Block block, Block previous, out string failureMessage)
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

            string target = new string('0', (int)block.difficulty);
            if (string.IsNullOrEmpty(block.Hash) || !block.Hash.StartsWith(target))
            {
                failureMessage = "Block " + block.Index + " does not satisfy proof of work.";
                return false;
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

        public List<Transaction> getTransactionsForNextBlock()
        {
            return getTransactionsForNextBlock(transactionsPerBlock);
        }

        public int getTransactionsPerBlock()
        {
            return transactionsPerBlock;
        }
    }
}
