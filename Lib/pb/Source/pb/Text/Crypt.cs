using System.Security.Cryptography;
using System.Text;

namespace pb.Text
{
    public static class Crypt
    {
        //public static string ComputeMD5Hash(string text, Encoding encoding = null)
        public static byte[] ComputeMD5Hash(string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (MD5 md5Hash = MD5.Create())
                // Convert the input string to a byte array and compute the hash. 
                //byte[] data = md5Hash.ComputeHash(encoding.GetBytes(text));

                //// Create a new Stringbuilder to collect the bytes 
                //// and create a string.
                //StringBuilder sb = new StringBuilder();

                //// Loop through each byte of the hashed data  
                //// and format each one as a hexadecimal string. 
                //for (int i = 0; i < data.Length; i++)
                //{
                //    //data[i].zToHex();
                //    sb.Append(data[i].ToString("x2"));
                //}

                //// Return the hexadecimal string. 
                //return sb.ToString();
                return md5Hash.ComputeHash(encoding.GetBytes(text));
        }

        public static byte[] ComputeSHA1Hash(string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (SHA1Managed sha1 = new SHA1Managed())
                return sha1.ComputeHash(encoding.GetBytes(text));
        }
    }
}
