using System.Text;
using System.Diagnostics;


namespace TestRC32
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("CRC32 Test 1!");

            var crc = new CRC32();

            // Convert a C# string to a byte array  
            string author = "Valon Hoti";


            // Convert a C# string to a byte array  
            ////convert binary file to byte[] ...
            byte[] bytes = Encoding.UTF8.GetBytes(author);
            string myRes = "";
            foreach (byte s in crc.ComputeHash(bytes))
            {
                myRes += s.ToString("x2").ToUpper();
            }

            Debug.WriteLine("returned result = " + myRes); //need to reverse it 
            Debug.WriteLine("reversed proper result = "+stringReverseString(myRes));

            while (true)
            {

            }
        }

        public static string stringReverseString(string str)
        {
            char[] chars = str.ToCharArray();
            for (int i = 0, j = str.Length - 1; i < j; i++, j--)
            {
                char c = chars[i];
                chars[i] = chars[j];
                chars[j] = c;
            }
            return new string(chars);
        }
    }
}
