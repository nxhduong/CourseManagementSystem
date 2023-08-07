using System.Text;
using System.Security.Cryptography;

namespace CourseManagementSystem.Data
{
    public class Crypto
    {
        public static readonly MD5 MD5Encryptor = MD5.Create();

        public static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder strOutput = new(arrInput.Length);
            for (var i = 0; i < arrInput.Length; i++)
            {
                strOutput.Append(arrInput[i].ToString("X2"));
            }
            return strOutput.ToString();
        }
    }
}
