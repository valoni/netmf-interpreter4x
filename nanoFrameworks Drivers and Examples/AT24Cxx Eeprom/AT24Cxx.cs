using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.I2c;

/*
   AT24Cxx 
   this is modified version of driver taken from there :

   https://github.com/WildernessLabs/Netduino.Foundation/tree/master/Source/Peripheral_Libs/ICs.EEPROM.AT24Cxx

    22/06/2020  - Prishtine / Kosovo
    Valon Hoti
   
*/

namespace EEprom
{
    public class AT24Cxx
    {
        /*
           Random word addressing requires a 12/13 bit data word address
           AT24C32  = 256,16 = 4096 Kbyte
           AT24C64  = 256,32 = 8192 Kbyte
           -------------------------------------------------------------
           Random word addressing requires a 14/15-bit data word addres
           AT24C128 = 256,64 = 16384 Kbyte
           AT24C256 = 512,64 = 32768 Kbyte
          */

        /// <summary>
        ///     Eeprom device.
        /// </summary>
        private I2cDevice EEprom;

        /// <summary>
        ///     Number of bytes in a page.
        /// </summary>
        private ushort _pageSize;

        /// <summary>
        ///     Number of bytes in the EEPROM module.
        /// </summary>
        private readonly int _memorySize;

 
        public AT24Cxx(string device, byte DeviceAddress , ushort pagesize , ushort bytesperpageblock)
        {
            
            _pageSize = pagesize;  
            _memorySize = pagesize* bytesperpageblock;

            EEprom = I2cDevice.FromId(
                device, 
                new I2cConnectionSettings(DeviceAddress)
                {
                    BusSpeed = I2cBusSpeed.FastMode,
                    SharingMode = I2cSharingMode.Exclusive
                }
             );
        }



        /// <summary>
        ///     Check the startAddress and the amount of data being accessed to make sure that the
        ///     addresss and the startAddress plus the amount remain within the bounds of the memory chip.
        /// </summary>
        /// <param name="address">Start startAddress for the memory activity.</param>
        /// <param name="amount">Amunt of data to be accessed.</param>
        private void CheckAddress(ushort address, ushort amount)
        {
            if (address >= _memorySize)
            {
                throw new ArgumentOutOfRangeException(
                    "address", "startAddress should be less than the amount of memory in the module");
            }
            if ((address + amount) >= _memorySize)
            {
                throw new ArgumentOutOfRangeException(
                    "address", "startAddress + amount should be less than the amount of memory in the module");
            }
        }


        /// <summary>
        ///     Force the Eeprom to make a reading and update the relevanyt properties.
        /// </summary>
        /// <param name="startAddress">Start address for the read operation.</param>
        /// <param name="amount">Amount of data to read from the EEPROM.</param>
        public byte[] read(ushort startAddress, ushort amount)
        {
            CheckAddress(startAddress, amount);

            byte[] Data = new byte[amount];
            var address = new byte[2];

            address[0] = (byte)((startAddress >> 8) & 0xff);
            address[1] = (byte)(startAddress & 0xff);
            EEprom.WriteRead(address,Data);
           
           return Data;
        }

        /// <summary>
        ///     Write a number of bytes to the EEPROM.
        /// </summary>
        /// <param name="startAddress">Address of he first byte to be written.</param>
        /// <param name="data">Data to be written to the EEPROM.</param>
        public void write(ushort startAddress, byte[] data)
        {
            CheckAddress(startAddress, (ushort)data.Length);
            //
            //  TODO: Convert to use page writes where possible.
            //
            for (ushort index = 0; index < data.Length; index++)
            {
                var address = (ushort)(startAddress + index);
                var addressAndData = new byte[3];

                addressAndData[0] = (byte)((address >> 8) & 0xff);
                addressAndData[1] = (byte)(address & 0xff);
                addressAndData[2] = data[index];

                EEprom.Write(addressAndData);
                Thread.Sleep(5);
            }

        }
    }
}
