using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace BlockchainAssignment
{
    public static class AdaptiveDifficulty
    {
        private const float InitialDifficulty = 4f;
        private const float MinimumDifficulty = 1f;
        private const float MaximumDifficulty = 6f;
        private const double TargetBlockTimeSeconds = 20d;
        private const double EmaAlpha = 0.35d;
        private const long TargetScale = 1000000000L;

        public static float GetNextDifficulty(List<Block> blocks)
        {
            if (blocks == null || blocks.Count < 2)
            {
                return InitialDifficulty;
            }

            double emaSeconds = GetAverageIntervalSeconds(blocks);
            double ratio = TargetBlockTimeSeconds / emaSeconds;
            double change = Math.Log(ratio) / Math.Log(16d);
            float nextDifficulty = (float)(blocks[blocks.Count - 1].difficulty + change);

            return ClampDifficulty((float)Math.Round(nextDifficulty, 3, MidpointRounding.AwayFromZero));
        }

        public static bool HashMeetsDifficulty(string hashHex, float difficulty)
        {
            if (string.IsNullOrWhiteSpace(hashHex))
            {
                return false;
            }

            BigInteger hashValue = ParseHex(hashHex);
            BigInteger target = GetTarget(difficulty);
            return hashValue <= target;
        }

        private static float ClampDifficulty(float difficulty)
        {
            if (difficulty < MinimumDifficulty)
            {
                return MinimumDifficulty;
            }

            if (difficulty > MaximumDifficulty)
            {
                return MaximumDifficulty;
            }

            return difficulty;
        }

        private static double GetAverageIntervalSeconds(List<Block> blocks)
        {
            double emaSeconds = GetIntervalSeconds(blocks[1], blocks[0]);

            for (int i = 2; i < blocks.Count; i++)
            {
                double intervalSeconds = GetIntervalSeconds(blocks[i], blocks[i - 1]);
                emaSeconds = (EmaAlpha * intervalSeconds) + ((1d - EmaAlpha) * emaSeconds);
            }

            return emaSeconds;
        }

        private static double GetIntervalSeconds(Block current, Block previous)
        {
            double seconds = (current.timeStamp - previous.timeStamp).TotalSeconds;
            return Math.Max(1d, seconds);
        }

        private static BigInteger GetTarget(float difficulty)
        {
            float clampedDifficulty = ClampDifficulty(difficulty);
            int wholeSteps = (int)Math.Floor(clampedDifficulty);
            double fractionalStep = clampedDifficulty - wholeSteps;
            int bits = 256 - (wholeSteps * 4);
            BigInteger baseTarget = bits > 0 ? BigInteger.One << bits : BigInteger.One;
            long divisor = (long)Math.Round(Math.Pow(16d, fractionalStep) * TargetScale, MidpointRounding.AwayFromZero);

            if (divisor < 1)
            {
                divisor = 1;
            }

            BigInteger target = ((baseTarget * TargetScale) / divisor) - BigInteger.One;
            return target < BigInteger.Zero ? BigInteger.Zero : target;
        }

        private static BigInteger ParseHex(string hashHex)
        {
            string evenHex = hashHex.Length % 2 == 0 ? hashHex : "0" + hashHex;
            byte[] bytes = new byte[(evenHex.Length / 2) + 1];

            for (int i = 0; i < evenHex.Length; i += 2)
            {
                int byteIndex = bytes.Length - 2 - (i / 2);
                bytes[byteIndex] = byte.Parse(evenHex.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return new BigInteger(bytes);
        }
    }
}
