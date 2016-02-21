using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace PsdBasesSetter.Crypto
{
    public class BasePasswords
    {

        private const String PSD_PASS_SALT = "psd_salt-v2";
        private const String PC_BASE_PASS_SALT = "pc_base_salt-v2";
        private const String PHONE_BASE_PASS_SALT = "phone_base_salt-v2";

        private const byte KEY_LENGTH = 32;
        private const int ITERATIONS_COUNT = 10000;

        public byte[] PhonePassword { get; private set; }
        public byte[] BasePassword { get; private set; }
        public byte[] PsdLoginPass { get; set; }


        public BasePasswords(string pass)
        {
            BasePassword = GeneratePcPassword(pass);
            PhonePassword = GeneratePhonePassword(pass);
            PsdLoginPass = GeneratePsdPassword(pass);
        }


        private byte[] GeneratePcPassword(String pass)
        {
            return GenerateKey(pass, PC_BASE_PASS_SALT);
        }
        private byte[] GeneratePhonePassword(String pass)
        {
            return GenerateKey(pass, PHONE_BASE_PASS_SALT);
        }
        private byte[] GeneratePsdPassword(String pass)
        {
            return GenerateKey(pass, PSD_PASS_SALT);
        }

        private static byte[] GenerateKey(String pass, String salt)
        {
            byte[] passBytes = Encoding.ASCII.GetBytes(pass);
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passBytes, saltBytes, ITERATIONS_COUNT);
            byte[] hash = rfc2898DeriveBytes.GetBytes(KEY_LENGTH);

            return hash;
        }

        private static byte[] ConcatArrays(byte[] arr1, byte[] arr2)
        {
            var result = new byte[arr1.Length + arr2.Length];
            arr1.CopyTo(result, 0);
            arr2.CopyTo(result, arr1.Length);
            return result;
        }
    }
}
