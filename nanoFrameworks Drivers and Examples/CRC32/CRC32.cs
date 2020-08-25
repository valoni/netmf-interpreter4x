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
