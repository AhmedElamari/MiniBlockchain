using BlockchainAssignment.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    public class Blockchain
    {
        List<Block> Blocks = new List<Block>();
        private int transactionsPerBlock = 5;
        public List<Transaction> transactionPool = new List<Transaction>();
        public Blockchain()
        {
            Block genesisBlock = new Block();
            Blocks.Add(genesisBlock);

        }
        public string returnBlockchain(Block Index)
        {
            return Index.ToString();
        }

        public Block GetLastBlock()
        {
            return Blocks[Blocks.Count - 1];
        }
        public List<Block> GetBlocks()
        {
            return Blocks;
        }
        public void AddBlock(Block block, List<Transaction> chosenTransaction)
        {
            Blocks.Add(block);
            transactionPool = transactionPool.Except(chosenTransaction).ToList();
        }

        public List<Transaction>GetPendingTransactions()    
        {
            int n = Math.Min(transactionsPerBlock, transactionPool.Count);
            List<Transaction> pendingTransactions = transactionPool.AsQueryable().Take(n).ToList();
            transactionPool = transactionPool.Except(pendingTransactions).ToList();
            return pendingTransactions;

        }
    }
}

   