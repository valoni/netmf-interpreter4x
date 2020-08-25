
using it

```csharp

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
            Debug.WriteLine("reversed proper result = "+stringReverseString(myRes)); //do not use it on desktop version

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

```

class

```csharp

using System;

namespace TestRC32
{
    class CRC32
    {
        private uint[] ChecksumTable;
        private uint Polynomial = 0xEDB88320;

        public CRC32()
        {
            ChecksumTable = new uint[0x100];

            for (uint index = 0; index < 0x100; ++index)
            {
                uint item = index;
                for (int bit = 0; bit < 8; ++bit)
                    item = ((item & 1) != 0) ? (Polynomial ^ (item >> 1)) : (item >> 1);
                ChecksumTable[index] = item;
            }
        }

        public byte[] ComputeHash(byte[] stream)
        {
            UInt32 result = 0xFFFFFFFF;
            int current;

            for (int i = 0; i < stream.Length; i++)
            {
                current = stream[i];
                result = ChecksumTable[(result & 0xFF) ^ (byte)current] ^ (result >> 8);
            }

            byte[] hash = BitConverter.GetBytes(~result);
            byte[] reverseHash = Reverse(hash);

            return Reverse(reverseHash);
        }

        private byte[] Reverse(byte[] Array)
        {
            byte[] toReverse = new byte[Array.Length];

            int j = 0;

            for (int i = Array.Length - 1; i >= 0; i--)
            {
                toReverse[j] = Array[i];
                j++;
            }
            return toReverse;
        }

    }
}


```
