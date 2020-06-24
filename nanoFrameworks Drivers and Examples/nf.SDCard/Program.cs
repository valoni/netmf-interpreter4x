using System;
using System.Diagnostics;
using System.Threading;

using nanoFramework.Hardware.Esp32;


namespace nfSDCARD
{
    public class Program
    {
        public static NfStorage SD;

        public static void Main()
        {
            Debug.WriteLine("Hello SD CARD storage - ESP32! \n");

            //Wait
            Thread.Sleep(100);

            // Set pins
            //Configuration.SetPinFunction(GpioPin, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);

            //Wait
            Thread.Sleep(100);

            // Set constructor to call SPI SDCard 
            //                   SP1 , CS_GpioPin
            SD = new NfStorage("SPI1",5);
    

            // Internal storage does not support directories
            // Get the files in the starting directory
            // Example: SD.GetFiles("folder1")
            string[] Files = SD.GetFiles();

            foreach (var File in Files)
            {
                Debug.WriteLine("File -> " + File);

            }


            // Internal storage does not support directories
            // Get the directoriess in the starting directory
            // Example: SD.GetDirectories("folder1")
            //  string[] Directories = SD.GetDirectories();

            // foreach (var dir in Directories)
            //  {
            //      Debug.WriteLine("Directory -> " + dir);
            //  }


            // Write text in starting directory
            // Write over the current file if it exists (default is append the current file)
            // Example: SD.WriteText("FOLDER1\\FOLDER2\\TEST2.TXT", "Hello World! \n");
            SD.WriteText("Test1.txt", "Hello internal storage!", false);

            Boolean Exists = SD.FileExists("Test1.txt");
            if (Exists)
                Debug.WriteLine("Test1.txt exists");

            // Delete file
            // SD.DeleteFile("test1.txt");

            // Rename file
            // SD.RenameFile("test1.txt", "test2.txt");

            // Read a text file from starting directory
            // Example: string textreturned = SD.ReadText("FOLDER1\\FOLDER2\\TEST2.TXT");

            //Write binary file 
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 100 };

            SD.WriteBuffer("BITES.BIN", bytes, false);

            //Read binary file
            byte[] rb = SD.ReadBuffer("BITES.BIN");

            Debug.WriteLine("Bytes read from BITES.BIN");

            foreach (byte b in rb)
                Debug.Write($"{b:X},");
            Debug.WriteLine($"");


            SD.SDCardUnmount();

            Thread.Sleep(Timeout.Infinite);

        }
    }
}
