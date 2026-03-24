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

        public string returnBlockchain(Block Index)
        {
            return Index.ToString();
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

        public List<Transaction> getPendingTransactionsPool()
        {
            return transactionPool.ToList();
        }

        public List<Transaction> getTransactionsForNextBlock(int transactionsPerBlock)    
        {
            int n = Math.Min(transactionsPerBlock, transactionPool.Count);
            List<Transaction> pendingTransactions = transactionPool.AsQueryable().Take(n).ToList();
            transactionPool = transactionPool.Except(pendingTransactions).ToList();
            return pendingTransactions;

        }
    }
}

   