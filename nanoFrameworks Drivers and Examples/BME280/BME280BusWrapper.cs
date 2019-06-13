/*
 * Author: Monoculture 2019
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

using Windows.Devices.I2c;


namespace nanoFramework.Drivers.BME280
{

    public enum BME280Address : byte
    {
        Primary = 0x77,
        Secondary = 0x76
    }

    public enum BME280BusType
    {
        I2C,
        SPI
    }

    internal class BME280CFData
    {
        public ushort T1 { get; set; }
        public short T2 { get; set; }
        public short T3 { get; set; }
        public ushort P1 { get; set; }
        public short P2 { get; set; }
        public short P3 { get; set; }
        public short P4 { get; set; }
        public short P5 { get; set; }
        public short P6 { get; set; }
        public short P7 { get; set; }
        public short P8 { get; set; }
        public short P9 { get; set; }
        public byte H1 { get; set; }
        public short H2 { get; set; }
        public byte H3 { get; set; }
        public short H4 { get; set; }
        public short H5 { get; set; }
        public sbyte H6 { get; set; }

    }

    public enum BME280Filter : byte
    {
        Off = 0b000,
        X2 = 0b001,
        X4 = 0b010,
        X8 = 0b011,
        X16 = 0b100
    }

    public enum BME280OverSample : byte
    {
        None = 0b000,
        X1 = 0b001,
        X2 = 0b010,
        X4 = 0b011,
        X8 = 0b100,
        X16 = 0b101
    }

    public enum BME280SensorMode : byte
    {
        Sleep = 0x00,
        Forced = 0x01,
        Normal = 0x11
    }

    public enum BME280StandbyTime : byte
    {
        Ms05 = 0b000,
        Ms10 = 0b110,
        Ms20 = 0b111,
        Ms625 = 0b001,
        Ms125 = 0b010,
        Ms250 = 0b011,
        Ms500 = 0b100,
        Ms1000 = 0b101
    }

    internal class Constants
    {
        public const byte BME280_REGISTER_CHIPID = 0xD0;
        public const byte BME280_REGISTER_CONTROL = 0xF4;
        public const byte BME280_REGISTER_SOFTRESET = 0xE0;
        public const byte BME280_REGISTER_PRESSUREDATA = 0xF7;
    }

    internal class BME280BusWrapper
    {
    
        public BME280BusWrapper(I2cDevice device)
        {
            I2CDevice = device;
        }

        private I2cDevice I2CDevice { get; }

        public BME280BusType BusType =>  BME280BusType.I2C;

        public byte ReadRegister(byte address)
        {
            return ReadRegion(address, 1)[0];
        }

        public byte[] ReadRegion(byte address, int length)
        {
           
                var readBuffer = new byte[length];

                var writeBuffer = new byte[] { address };

                I2CDevice.WriteRead(writeBuffer, readBuffer);

                return readBuffer;
          
        }

        public void WriteRegister(byte address, byte data)
        {
            WriteRegion(address, new byte[] { data });
        }

        public void WriteRegion(byte address, byte[] data)
        {
            if (BusType == BME280BusType.I2C)
            {
                var writeBuffer = new byte[data.Length + 1];

                writeBuffer[0] = address;

                Array.Copy(data, 0, writeBuffer, 1, data.Length);

                I2CDevice.Write(writeBuffer);
            }
            
        }
    }
}
