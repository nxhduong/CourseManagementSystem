using System.Text;
using System.Security.Cryptography;

namespace CourseManagementSystem.Utilities
{
    public static class Cryptography
    {
        private static readonly MD5 MD5Encryptor = MD5.Create();

        public static string FromByteArrayToString(this byte[] arrInput)
        {
            StringBuilder strOutput = new(arrInput.Length);
            for (var i = 0; i < arrInput.Length; i++)
            {
                strOutput.Append(arrInput[i].ToString("X2"));
            }
            return strOutput.ToString();
        }

        public static byte[] ComputeMD5(this byte[] buffer)
        {
            return MD5Encryptor.ComputeHash(buffer);
        }
    }
}
