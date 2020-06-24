using System;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace EEprom
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("----------------------------------");
            Debug.WriteLine("AT24Cxx Eeprom from nanoFramework!");
            Debug.WriteLine("----------------------------------");

            //initialise eeprom ..
            Debug.WriteLine("Initialise EEprom on Bus I2C1 on address 0x50 ");
            Debug.WriteLine("----------------------------------");

            //set pages and blocko of byte
            //------------------
            //AT24C32 128,32 = 4096 
            //AT24C64 256,32 = 8192
            //------------------
            //AT24C128 256,64 = 16xxx
            //AT24C256 512,64 = 32768
            //------------------
            //start writing from start address 0
            AT24Cxx eeprom = new AT24Cxx("I2C1", 0x50, 512 , 64 );
            Thread.Sleep(100);

            //start writing from start address 0
            Debug.WriteLine("Write to the EEprom by starting from Address = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15} ");
            Debug.WriteLine(".......and Write on the EEprom those Values  = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} ");
            eeprom.write(0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            Debug.WriteLine("----------------------------------");

            //start reading again after write 
            Debug.WriteLine("Read EEprom by starting from A[0]..A[15]");
            Debug.WriteLine("----------------------------------------");

            var memory = eeprom.read(0, 16);

            //display results from reading
            for (ushort index = 0; index < 16; index++)
            {
                Debug.Write("A[" + index + "]= " + memory[index] + "; ");
                if (index == 7) Debug.WriteLine("");
            }
            Debug.WriteLine("");
            Debug.WriteLine("--------------------------------------------------------------------");
            //do write again from start address 3 than from start address 7
            Debug.WriteLine("Write EEprom at A[3]=10 and starting from A[7]+={ 1, 2, 3, 4 }");
            Debug.WriteLine("--------------------------------------------------------------------");

            eeprom.write(3, new byte[] { 10 });
            eeprom.write(7, new byte[] { 1, 2, 3, 4 });

            //start reading from start address 0 , 16 byte length ...
            Debug.WriteLine("Read EEprom starting from A[0]");
            Debug.WriteLine("--------------------------------------------------------------------");

            memory = eeprom.read(0, 16);

            //display results from reading
            for (ushort index = 0; index < 16; index++)
            {
                Debug.Write("A[" + index + "]= " + memory[index] + "; ");
                if (index == 7) Debug.WriteLine("");
            }
            Debug.WriteLine("");
            Debug.WriteLine("----------------------------------");
            Debug.WriteLine("End of test ..");

           Thread.Sleep(Timeout.Infinite);
        }
    }
}
