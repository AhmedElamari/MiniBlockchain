using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using BlockchainAssignment.HashCode;

namespace BlockchainAssignment.Wallet
{
    class Wallet
    {
        // Public ID - viewable to all (Derived from the users private key)
        public String publicID; 

        public Wallet(out String privateKey)
        {
            privateKey = String.Empty;
            
            byte[] pubKey;
            byte[] privKey;

            CngKeyCreationParameters keyCreationParameters = new CngKeyCreationParameters();
            keyCreationParameters.ExportPolicy = CngExportPolicies.AllowPlaintextExport;
            keyCreationParameters.KeyUsage = CngKeyUsages.Signing;

            CngKey key = CngKey.Create(CngAlgorithm.ECDsaP256, null, keyCreationParameters);

            byte[] KeyBlob = key.Export(CngKeyBlobFormat.EccPrivateBlob); 
            byte[] pubBlob = key.Export(CngKeyBlobFormat.EccPublicBlob);

            pubKey = KeyBlob.Skip(8).Take(KeyBlob.Length - 40).ToArray();
            privKey = KeyBlob.Skip(72).Take(KeyBlob.Length).ToArray();

            publicID = Convert.ToBase64String(pubKey);
            privateKey = Convert.ToBase64String(privKey);
        }

        public static bool ValidatePrivateKey(String privateKey, String publicID)
        {
            // Random string used to create a verification signature
            String testHash = "0000abc1e11b8d37c1e1232a2ea6d290cddb0c678058c37aa766f813cbbb366e"; 

            if (privateKey.Length != 44 || publicID.Length != 88)
                return false;

            String sig = createSignature(publicID, privateKey, testHash);

            return ValidateSignature(publicID, testHash, sig);
        }

        public static bool ValidateSignature(String publicID, String datahash, String datasig)
        {
            if (publicID.Equals("Mine Rewards"))
                publicID = "QfF3+9GgTxyGLvb+ScOAI6nJxBh8IyZbeD0r6BJBMyabZmyuP82yrSLKMq/F05OG0VZ4gg63uHFZUKzCu3wZuA==";

            if (publicID.Length != 88 || datasig.Equals("null"))
                return false;

            CngKey key = createKey(publicID);

            if (key == null)
                return false;

            try
            {
                using (ECDsaCng dsa = new ECDsaCng(key))
                    return dsa.VerifyData(HashTools.StringToByteArray(datahash), Convert.FromBase64String(datasig));
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        public static String createSignature(String publicID, String privateKey, String datahash)
        {
            CngKey key = createKey(publicID, privateKey);

            if (key == null)
                return "null";
            
            Byte[] datahashByte = HashTools.StringToByteArray(datahash);

            ECDsaCng dsa = new ECDsaCng(key);
            Byte[] byteSig = dsa.SignData(datahashByte);

            return Convert.ToBase64String(byteSig);
        }

        private static CngKey createKey(String publicID, String privateKey = "")
        {
            try
            {
                if (publicID.Equals("Mine Rewards") && privateKey.Equals(String.Empty))
                {
                    publicID = "QfF3+9GgTxyGLvb+ScOAI6nJxBh8IyZbeD0r6BJBMyabZmyuP82yrSLKMq/F05OG0VZ4gg63uHFZUKzCu3wZuA==";
                    privateKey = "mkT1Iu3YF4NSruHBptVytyDkNcxwemrkclndJH0+73o=";
                }

                CngKey key;
                byte[] keyByte = new Byte[] { 69, 67, 83, 49, 32, 0, 0, 0 }; //first 8 bytes always same
                byte[] publicBytes = Convert.FromBase64String(publicID);
                byte[] keyByteCombine1 = new Byte[72];
                keyByte.CopyTo(keyByteCombine1, 0);
                publicBytes.CopyTo(keyByteCombine1, keyByte.Length);

                if (!privateKey.Equals(String.Empty))
                {
                    keyByteCombine1[3] = 50; //must be set to 50 to be a private block
                    byte[] privateBytes = Convert.FromBase64String(privateKey);
                    byte[] keyByteCombine2 = new Byte[104];
                    keyByteCombine1.CopyTo(keyByteCombine2, 0);
                    privateBytes.CopyTo(keyByteCombine2, keyByteCombine1.Length);

                    key = CngKey.Import(keyByteCombine2, CngKeyBlobFormat.EccPrivateBlob);
                    return key;
                }
                key = CngKey.Import(keyByteCombine1, CngKeyBlobFormat.EccPublicBlob);
                return key;
            }

            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                return null;
            }
        }
    }
}
