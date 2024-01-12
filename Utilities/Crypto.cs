using System.Text;
using System.Security.Cryptography;

namespace CourseManagementSystem.Utilities
{
    public static class Crypto
    {
        private static readonly MD5 MD5Encryptor = MD5.Create();

        public static string ConvertByteArrayToString(byte[] inputByteArray)
        {
            StringBuilder outputString = new(inputByteArray.Length);
            for (var i = 0; i < inputByteArray.Length; i++)
            {
                outputString.Append(inputByteArray[i].ToString("X2"));
            }
            return outputString.ToString();
        }

        public static byte[] ComputeMD5(this byte[] buffer)
        {
            return MD5Encryptor.ComputeHash(buffer);
        }
    }
}
