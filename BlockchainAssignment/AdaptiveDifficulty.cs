using BlockchainAssignment.HashCode;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BlockchainAssignment
{
    public static class AdaptiveDifficulty
    {
        private const int HashByteCount = 32;
        private const int HashHexLength = HashByteCount * 2;
        private const float InitialDifficulty = 4f;
        private const float MinimumDifficulty = 1f;
        private const float MaximumDifficulty = 6f;
        private const double TargetBlockTimeSeconds = 20d;
        private const double EmaAlpha = 0.35d;
        private const long TargetScale = 1000000000L;

        public static float GetNextDifficulty(List<Block> blocks)
        {
            return GetNextDifficulty(blocks, blocks == null ? 0 : blocks.Count);
        }

        public static float GetNextDifficulty(List<Block> blocks, int count)
        {
            if (blocks == null || count < 2)
            {
                return InitialDifficulty;
            }

            if (count > blocks.Count)
            {
                count = blocks.Count;
            }

            double emaSeconds = GetAverageIntervalSeconds(blocks, count);
            double ratio = TargetBlockTimeSeconds / emaSeconds;
            double change = Math.Log(ratio) / Math.Log(16d);
            float nextDifficulty = (float)(blocks[count - 1].difficulty + change);

            return ClampDifficulty((float)Math.Round(nextDifficulty, 3, MidpointRounding.AwayFromZero));
        }

        public static bool HashMeetsDifficulty(string hashHex, float difficulty)
        {
            byte[] hashBytes;
            if (!TryParseHash(hashHex, out hashBytes))
            {
                return false;
            }

            return HashMeetsDifficulty(hashBytes, GetTargetBytes(difficulty));
        }

        internal static byte[] GetTargetBytes(float difficulty)
        {
            return ToBigEndianBytes(GetTarget(difficulty));
        }

        internal static bool HashMeetsDifficulty(byte[] hashBytes, byte[] targetBytes)
        {
            if (hashBytes == null || targetBytes == null || hashBytes.Length != HashByteCount || targetBytes.Length != HashByteCount)
            {
                return false;
            }

            for (int i = 0; i < HashByteCount; i++)
            {
                if (hashBytes[i] == targetBytes[i])
                {
                    continue;
                }

                return hashBytes[i] < targetBytes[i];
            }

            return true;
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

        private static double GetAverageIntervalSeconds(List<Block> blocks, int count)
        {
            double emaSeconds = GetIntervalSeconds(blocks[1], blocks[0]);

            for (int i = 2; i < count; i++)
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

        private static byte[] ToBigEndianBytes(BigInteger value)
        {
            byte[] bytes = new byte[HashByteCount];
            byte[] littleEndian = value.ToByteArray();
            int count = Math.Min(HashByteCount, littleEndian.Length);

            for (int i = 0; i < count; i++)
            {
                bytes[HashByteCount - 1 - i] = littleEndian[i];
            }

            return bytes;
        }

        private static bool TryParseHash(string hashHex, out byte[] hashBytes)
        {
            hashBytes = null;
            if (string.IsNullOrWhiteSpace(hashHex) || hashHex.Length != HashHexLength)
            {
                return false;
            }

            try
            {
                hashBytes = HashTools.StringToByteArray(hashHex);
                return hashBytes.Length == HashByteCount;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
