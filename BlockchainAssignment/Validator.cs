namespace BlockchainAssignment
{
    public class Validator
    {
        public Validator(string publicKey, decimal stake)
        {
            this.publicKey = publicKey;
            this.stake = stake;
        }

        public string publicKey { get; private set; }
        public decimal stake { get; private set; }
        public int blocksForged { get; private set; }
        public int penalties { get; private set; }

        public void IncrementBlocksForged()
        {
            blocksForged++;
        }

        public void IncrementPenalties()
        {
            penalties++;
        }

        public void SlashStake(decimal amount)
        {
            if (amount <= 0)
                return;

            stake = stake > amount ? stake - amount : 0m;
        }
    }
}
