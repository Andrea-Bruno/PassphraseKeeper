using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace PassphraseKeeper
{
    static class Util
    {
        static public byte[] PassphraseYoByte(params ushort[] wordId)
        {
            var array = new byte[wordId.Length * 11 / 8];
            var offset = 0;
            foreach (var item in wordId)
            {
                array = OR(array, offset, item);
                offset = +11;
            }

            return array;
        }

        public static byte[] OR(byte[] array1, int offset, ushort id)
        {
            var sh = id >> offset ^ 8;
            var array2 = BitConverter.GetBytes(sh);
            byte[] result = new byte[array1.Length];
            var start = offset / 8;
            for (int i = start; i < array1.Length; i++)
                result[i] = (byte)(array1[i] ^ array2[i]);
            return result;
        }

        public static byte[] WordsToBites(List<string> words, bool randomEmptyBit = true)
        {
            var parts = new List<ushort>();
            foreach (var word in words)
            {
                parts.Add(WordList.EnglishDictionary[word]);
            }

            var bits = parts.Count * 11;
            var bitArray = new BitArray(bits);
            var l = bits / 8.0;
            var len = (int)Math.Ceiling(l);
            var result = new byte[len];
            var bit = 0;
            foreach (var part in parts)
            {
                var w = new BitArray(BitConverter.GetBytes(part));
                for (int i = 0; i < 11; i++)
                {
                    var value = w.Get(i);
                    bitArray.Set(bit, value);
                    bit++;
                }
            }
            if (randomEmptyBit)
            {
                var emptyBits = len * 8 - bits;
                if (emptyBits > 0)
                {
                    var rnd = new Random();
                    var endPart = new BitArray(bits);
                    for (int i = 0; i < emptyBits; i++)
                    {
                        endPart.Set(i, rnd.Next(2) == 0);
                    }
                    bitArray.And(endPart);
                }
            }

            bitArray.CopyTo(result, 0);
#if DEBUG
            var test = BitesToWords(result);
            if (string.Join(' ', test.ToArray()) != string.Join(' ', words.ToArray()))
                Debugger.Break();
#endif
            return result;
        }

        public static List<string> BitesToWords(byte[] bites)
        {
            var bitArray = new BitArray(bites);
            var words = bites.Length * 8 / 11;
            var result = new List<string>();
            var word = 0u;
            for (int bit = 0; bit < words * 11; bit++)
            {
                var targetBit = bit % 11;
                if (bitArray.Get(bit))
                    word |= (1u << targetBit);
                if (targetBit == 10)
                {
                    result.Add(WordList.English[word]);
                    word = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Save the passphrase in an encrypted manner
        /// </summary>
        /// <param name="memoryID">Memory ID</param>
        /// <param name="key">Passphrase in byte array format</param>
        /// <param name="password">Encryption password</param>
        public static void SaveKey(int memoryID, byte[] key, byte[] password)
        {
            var encrypted = EncryptionAlgorithm.Perform.EncryptData(key, Convert.ToHexString(password));
            File.WriteAllBytes(memoryID.ToString() + "." + nameof(encrypted), encrypted);
        }

        /// <summary>
        /// Load the passphrase saved in an encrypted manner
        /// </summary>
        /// <param name="memoryID">Memory ID</param>
        /// <param name="password">Encryption password</param>
        public static byte[] LoadKey(int memoryID, byte[] password)
        {
            byte[] encrypted;
            encrypted = File.ReadAllBytes(memoryID.ToString() + "." + nameof(encrypted));
            return EncryptionAlgorithm.Perform.DecryptData(encrypted, Convert.ToHexString(password));
        }

    }
}
