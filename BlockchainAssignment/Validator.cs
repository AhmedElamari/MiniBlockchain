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
        public int Penalties { get; private set; }
    }
}
