using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PsdBasesSetter.Repositories.Objects;

namespace PsdBasesSetter.Crypto
{

    class DividedPassword
    {
        public const int MaxPassLength = 128;

        public PassItem SrcPass;
        public byte[] Part1 { get; set; } = new byte[MaxPassLength];
        public byte[] Part2 { get; set; } = new byte[MaxPassLength];
        RNGCryptoServiceProvider _rngCsp = new RNGCryptoServiceProvider();

        public DividedPassword(PassItem pass)
        {
            SrcPass = pass;
            DivideAndAdd(pass);
            _rngCsp.Dispose();
        }


        private void DivideAndAdd(PassItem pass)
        {
            var passPart2Bytes = new byte[MaxPassLength];
            _rngCsp.GetBytes(passPart2Bytes);

            var realPassBytes = pass.GetBytes();

            var passPart1Bytes = XORArrays(passPart2Bytes, realPassBytes);

            if (!CheckCorrect(passPart2Bytes, passPart1Bytes, realPassBytes))
                throw new Exception("WTF? Xor worked funny..");

            Part1 = passPart2Bytes;
            Part2 = passPart1Bytes;
        }


        private bool CheckCorrect(byte[] part1, byte[] part2, byte[] realPass)
        {
            String origPass = Encoding.ASCII.GetString(realPass);
            String resPass = Encoding.ASCII.GetString(XORArrays(part1, part2)).TrimEnd('\0');
            return origPass.Equals(resPass);
        }

        private byte[] XORArrays(byte[] arr1, byte[] arr2)
        {
            int length = Math.Max(arr1.Length, arr2.Length);
            byte[] resBytes = new byte[length];

            Array.Copy(arr1, 0, resBytes, 0, arr1.Length);

            for (int i = 0; i < length; i++)
            {
                byte selectedByte;
                if (i < arr2.Length)
                    selectedByte = arr2[i];
                else
                    selectedByte = 0;
                resBytes[i] = (byte)((byte)selectedByte ^ (byte)resBytes[i]);
            }

            return resBytes;
        }


    }

}
