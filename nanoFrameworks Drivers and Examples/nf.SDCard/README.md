### Wiring
![alt text](https://github.com/valoni/netmf-interpreter4x/blob/master/nanoFrameworks%20Drivers%20and%20Examples/AT24Cxx%20Eeprom/AT24Cxx.png "AT24Cxx")

### How to use class / Program.cs
```csharp
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

```

### Driver / SDCardSPI.cs
```csharp
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Devices;
using Windows.Storage.Streams;


// Thanks to Adrian Soundly and Jose Somoes and Davide Waver
// and everyone helped make "Storage" possible for nanoFramework
// Storage is still under construction
// Check the samples here https://github.com/nanoframework/Samples/tree/master/samples/Storage
// for updates and changes
// SPI Card ...
// You may need a separate 3.3v power source for the SD card reader 


namespace nfSDCARD
{

    /// <summary>
    /// Class library for SD card or internal storage (SPIFFS)
    /// </summary>
    public class NfStorage
    {

        public StorageFolder SDevice;
        private StorageFolder Sfolder;
        private string Fname;
        private readonly Boolean IsInternalStorage = false;


        /// <summary>
        /// Constructor for mounting SD card or internal storage..
        /// Example NfStorage SD = new SDCardSPI("SPI1",26)
        /// One or more SPI pins can be changed
        /// </summary>
        public NfStorage(string SpiDevice, int CSPin)
        {
               try
                {

                    // Mount a SPI connected SDCard passing the SPI bus and the Chip select pin
                    SDCard.MountSpi(SpiDevice, CSPin);

                    if (SDCard.IsMounted)
                    {
                        Debug.WriteLine("Success SDCard is mounted");

                        StorageFolder externalDevices = Windows.Storage.KnownFolders.RemovableDevices;

                        var removableDevices = externalDevices.GetFolders();

                        //Set the Storage variable
                        SDevice = removableDevices[0];

                    }

                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to mount SDCard \n" + ex.Message);
                }

            
        }

        /// <summary>
        /// Get the files in the starting directory
        /// </summary>
        /// <returns>String array of files</returns>
        public String[] GetFiles(string StartingDirectory = "Root")
        {

            if (StartingDirectory == "Root")
            {
                Sfolder = SDevice;

            }
            else
            {
                StartingDirectory = StartingDirectory.ToUpper();

                SetDirectory(StartingDirectory);

            }

            var FilesInDevice = Sfolder.GetFiles();

            int fl = FilesInDevice.Length;

            string[] rs = new string[fl];

            int i = 0;

            foreach (StorageFile file in FilesInDevice)
            {
                rs[i] = file.Name;

                i += 1;

                Debug.WriteLine("Files -> " + file.Path);

            }

            return rs;

        }

        /// <summary>
        /// Read text file
        /// </summary>
        /// <returns>Text from file</returns>
        public string ReadText(string FilePath)
        {

            // Not capitalized in internal storage
            if (IsInternalStorage == false)
                FilePath = FilePath.ToUpper();

            try
            {

                //  Rem directory and filename set in FileExists
                if (FileExists(FilePath))
                {

                    var File = Sfolder.CreateFile(Fname, CreationCollisionOption.OpenIfExists);

                    return FileIO.ReadText(File);

                }

                return string.Empty;

            }

            catch (Exception ex)
            {
                return "Error: Reading file " + ex.Message;
            }

        }

        /// <summary>
        /// Read binary file
        /// </summary>
        /// <returns>Byte array from file</returns>
        public byte[] ReadBuffer(string FilePath)
        {
            byte[] ErrorByte = { 0x0 };

            // Not capitalized in internal storage
            if (IsInternalStorage == false)
                FilePath = FilePath.ToUpper();


            try
            {

                // Directory and file name set in FileExists
                if (FileExists(FilePath))
                {

                    var File = Sfolder.CreateFile(Fname, CreationCollisionOption.OpenIfExists);

                    IBuffer readBuffer = FileIO.ReadBuffer(File);

                    using (DataReader dataReader = DataReader.FromBuffer(readBuffer))
                    {
                        byte[] cBuf = new byte[readBuffer.Length];

                        dataReader.ReadBytes(cBuf);

                        Debug.WriteLine("Buffer length" + cBuf.Length);

                        return cBuf;

                    }

                }

                return ErrorByte;

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);

                return ErrorByte;

            }

        }

        /// <summary>
        /// Write to binary file
        /// Default is to append the existing file if the file exists
        /// </summary>
        public void WriteBuffer(string FilePath, byte[] Buffer, Boolean Append = true)
        {

            try
            {

                // Not capitalized in internal storage
                if (IsInternalStorage == false)
                    FilePath = FilePath.ToUpper();

                SetDirectoryandFilename(FilePath);

                if (Append)
                {

                    byte[] rBuffer = ReadBuffer(FilePath);

                    byte[] writeBuffer = new byte[rBuffer.Length + Buffer.Length];

                    Array.Copy(rBuffer, 0, writeBuffer, 0, rBuffer.Length - 1);

                    Array.Copy(Buffer, 0, writeBuffer, rBuffer.Length, Buffer.Length - 1);

                    var File = Sfolder.CreateFile(Fname, CreationCollisionOption.ReplaceExisting);

                    FileIO.WriteBytes(File, writeBuffer);

                    Debug.WriteLine("Wrote " + writeBuffer.Length + " bytes to " + FilePath + " for append");

                }

                else
                {
                    var File = Sfolder.CreateFile(Fname, CreationCollisionOption.ReplaceExisting);

                    FileIO.WriteBytes(File, Buffer);

                    Debug.WriteLine("Wrote " + Buffer.Length + " bytes to " + FilePath);

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error writing binary file: " + ex.Message);

            }
        }

        /// <summary>
        /// Get directories from a starting directory
        /// Example: If StartingDirectory =  "folder1\\folder2"
        /// Directories in folder2 are found
        /// </summary>
        /// <returns>String array of directories</returns>
        public String[] GetDirectories(string StartingDirectory = "Root")
        {

            try
            {

                if (IsInternalStorage)
                {
                    Debug.WriteLine("\n Directories are currently not supported for internal storage.\n");

                }

                if (StartingDirectory == "Root")
                {
                    Sfolder = SDevice;

                }
                else
                {
                    StartingDirectory = StartingDirectory.ToUpper();

                    SetDirectory(StartingDirectory);

                }

                var foldersInDevice = Sfolder.GetFolders();

                int fl = foldersInDevice.Length;

                string[] rs = new string[fl];

                int i = 0;

                foreach (StorageFolder folder in foldersInDevice)
                {
                    rs[i] = folder.Path;

                    i += 1;

                    Debug.WriteLine($"Folder ->{folder.Name}");

                }

                return rs;

            }

            catch (Exception)
            {
                Debug.WriteLine("\n Directories are currently not supported for internal storage.\n");

                string[] emptyStringArray = new string[0];

                return emptyStringArray;

            }

        }

        /// <summary>
        /// Sets the private varable Sfolder to the current directory 
        /// Sets the private varable Fname to the curent file name
        /// </summary>
        /// <param name="FilePath"></param>
        private void SetDirectoryandFilename(string FilePath)
        {
            try
            {

                char slash = '\\';

                Sfolder = SDevice;

                if (FilePath.IndexOf(slash) == -1)
                {

                    Debug.WriteLine("File path -> " + FilePath);
                    Fname = FilePath;

                }

                else
                {
                    string[] str = FilePath.Split(slash);

                    for (int i = 0; i < str.Length - 1; i++)
                    {

                        Sfolder = Sfolder.CreateFolder(str[i], CreationCollisionOption.ReplaceExisting);

                        Debug.WriteLine("Successfully created folder: " + Sfolder.Path);

                    }

                    Fname = str[str.Length - 1];

                    Debug.WriteLine("File name -> " + Fname);

                }

            }
            catch (Exception)
            {
                Debug.WriteLine("Directories are not supported for internal storage");

            }

        }

        /// <summary>
        /// Sets the private varable Sfolder to the current directory 
        /// Example: If StartingDirectory = folder1\\folder2
        /// Directories under folder2 are found
        /// </summary>
        private void SetDirectory(string StartingDirectory)
        {
            try
            {

                char slash = '\\';

                Sfolder = SDevice;

                if (StartingDirectory.IndexOf(slash) == -1)
                {

                    if (StartingDirectory != "Root")
                    {
                        Sfolder = Sfolder.CreateFolder(StartingDirectory, CreationCollisionOption.ReplaceExisting);

                        Debug.WriteLine("Successfully created folder: " + Sfolder.Path);
                    }

                }

                else
                {
                    string[] str = StartingDirectory.Split(slash);

                    for (int i = 0; i < str.Length; i++)
                    {

                        Sfolder = Sfolder.CreateFolder(str[i], CreationCollisionOption.ReplaceExisting);

                        Debug.WriteLine("Successfully created folder: " + Sfolder.Path);

                    }

                }

            }
            catch (Exception)
            {
                Debug.WriteLine("Directories are not supported for internal storage");

            }

        }

        /// <summary>
        /// Write text to file
        /// Default is to append if file exists
        /// </summary>
        public void WriteText(string FilePath, string Text, Boolean Append = true)
        {

            try
            {
                //Not capitalized in internal storage
                if (IsInternalStorage == false)
                    FilePath = FilePath.ToUpper();


                SetDirectoryandFilename(FilePath);

                if (Append)
                {

                    string st = ReadText(FilePath);

                    var File = Sfolder.CreateFile(Fname, CreationCollisionOption.ReplaceExisting);

                    st = st + Text;

                    FileIO.WriteText(File, st);

                    Debug.WriteLine("Wrote " + st.Length + " bytes to " + FilePath + " for append");

                }

                else
                {
                    var File = Sfolder.CreateFile(Fname, CreationCollisionOption.ReplaceExisting);

                    FileIO.WriteText(File, Text);

                    Debug.WriteLine("Wrote " + Text.Length + " bytes to " + FilePath);
                }

            }

            catch (Exception ex)
            {
                Debug.WriteLine("Error writing text: " + ex.Message);

            }
        }

        /// <summary>
        /// Delete file if exists
        /// </summary>
        public void DeleteFile(string FilePath)
        {
            //Not capitalized in internal storage
            if (IsInternalStorage == false)
                FilePath = FilePath.ToUpper();

            SetDirectoryandFilename(FilePath);

            var Files = Sfolder.GetFiles();

            foreach (var File in Files)
            {

                if (Fname == File.Name)
                {
                    File.Delete();

                    Debug.WriteLine(File.Name + " was deleted");
                }

            }
        }

        /// <summary>
        /// Delete directory from the starting directory
        /// </summary>
        public void DeleteDirectory(string DirectoryToDelete, string StartingDirectory = "Root")
        {

            try
            {

                DirectoryToDelete = DirectoryToDelete.ToUpper();

                if (StartingDirectory == "Root")
                {
                    Sfolder = SDevice;
                }
                else
                {
                    StartingDirectory = StartingDirectory.ToUpper();

                    SetDirectory(StartingDirectory);

                }

                Boolean found = false;

                var Folders = Sfolder.GetFolders();

                foreach (var Folder in Folders)
                {
                    Debug.WriteLine("Folder Name -> " + Folder.Name);

                    if (DirectoryToDelete == Folder.Name)
                    {


                        Folder.Delete();

                        Debug.WriteLine(Folder.Path + " deleted");

                        found = true;

                    }

                }

                if (found == false)
                    Debug.WriteLine(DirectoryToDelete + " not found");

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Error deleting directory the directory must be empty: " + ex.Message);

            }


        }

        /// <summary>
        /// Rename file if FilePath exists and NewFilename does not exist
        /// </summary>
        public void RenameFile(string FilePath, string NewFilename)
        {
            //Not capitalized in internal storage
            if (IsInternalStorage == false)
                FilePath = FilePath.ToUpper();

            SetDirectoryandFilename(FilePath);

            Boolean found = false;

            var Files = Sfolder.GetFiles();

            Debug.WriteLine("Fname " + Fname);

            foreach (var File in Files)
            {

                Debug.WriteLine("Filename " + File.Name);

                if (Fname == File.Name)
                {
                    Debug.WriteLine("Found " + File.Name);
                    if (FileExists(NewFilename))
                    {
                        Debug.WriteLine(FilePath + " can't be renamed " + NewFilename + " exists");

                    }

                    else
                    {
                        File.Rename(NewFilename);

                        Debug.WriteLine(File.Name + " renamed " + NewFilename);

                        found = true;

                    }

                }

            }

            if (found == false)
                Debug.WriteLine(FilePath + " not found");

        }

        /// <summary>
        /// Returns true if FilePath exists
        /// </summary>
        public Boolean FileExists(string FilePath)
        {
            // Internal storage doesn't capitalize files
            if (IsInternalStorage == false)
                FilePath = FilePath.ToUpper();

            SetDirectoryandFilename(FilePath);

            try
            {
                var Files = Sfolder.GetFiles();

                foreach (var File in Files)
                {

                    if (Fname == File.Name)
                    {
                        Debug.WriteLine(Fname + " exists");

                        return true;

                    }

                }

                Debug.WriteLine(Fname + " not found");

                return false;

            }


            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);

                return false;
            }

        }

        /// <summary>
        /// If SDcard is mounted Unmount the SDCard
        /// </summary>
        public void SDCardUnmount()
        {
            //  Currently the mount card class only allows for 1 device to be mounted
            if (SDCard.IsMounted)
            {
                SDCard.Unmount();
                Debug.WriteLine("SDCard successfully unmounted");
            }
        }

    }

}

```
