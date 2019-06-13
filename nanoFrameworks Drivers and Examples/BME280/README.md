### Wiring
![alt text](https://github.com/valoni/netmf-interpreter4x/blob/master/nanoFrameworks%20Drivers%20and%20Examples/BME280/BME280_ESP32.jpg "ESP32 wiring with BME280")

### How to use driver / Program.cs
```csharp
using System;
using System.Threading;
using Windows.Devices.I2c;
using nanoFramework.Hardware.Esp32;

/*
   forked driver from  
   https://github.com/monoculture/Monoculture.TinyCLR.Drivers.BME280
   to work on nanoFramework
   
   TO DO :
   to fix crc calc
*/

using nanoFramework.Drivers.BME280;

namespace BEM280
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("nanoFramework BME280 Sample!");

            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            var settings = BME280Driver.GetI2CConnectionSettings(BME280Address.Secondary);         

            var device = I2cDevice.FromId("I2C1", settings);

            var driver = new BME280Driver(device);

            driver.Initialize();

            driver.ChangeSettings(
                BME280SensorMode.Forced,
                BME280OverSample.X1,
                BME280OverSample.X1,
                BME280OverSample.X1,
                BME280Filter.Off);

            Console.WriteLine(" ---------------- ");

            while (true)
            {
                driver.Read();

                Console.WriteLine("Pressure: "  + driver.Pressure.ToString("N2"));
                Console.WriteLine("Humidity: " + driver.Humidity.ToString("N2"));
                Console.WriteLine("Temperature:" + driver.Temperature.ToString("N2"));
                Console.WriteLine(" ---------------- ");

                Thread.Sleep(1000);
            }
        }
    }
}

```


### How to use driver / BME280BusWrapper.cs
```csharp

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

```

### How to use driver / BME280Driver.cs
```csharp
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
using System.Threading;
using Windows.Devices.I2c;


namespace nanoFramework.Drivers.BME280
{
    public class BME280Driver
    {
        private double _tFine;
        private int _rawHumidity;
        private int _rawPressure;
        private int _rawTemperature;
        private BME280CFData _calibration;
        private readonly BME280BusWrapper _device;

        public BME280Driver(I2cDevice device)
        {
            if(device == null)
                throw new ArgumentNullException(nameof(device));

            _device = new BME280BusWrapper(device);
        }

  

        public static I2cConnectionSettings GetI2CConnectionSettings(BME280Address address)
        {
            var settings = new I2cConnectionSettings((int) address)
            {
                BusSpeed = I2cBusSpeed.FastMode,
               // SharingMode = I2cSharingMode.Exclusive 
            };

            return settings;
        }


        public bool IsInitialized { get; private set; }

        public BME280BusType BusType => _device.BusType;

        public BME280Filter Filter { get; private set; } = BME280Filter.Off;

        public BME280SensorMode SensorMode { get; private set; } = BME280SensorMode.Normal;

        public BME280OverSample OsrTemperature { get; private set; } = BME280OverSample.X1;

        public BME280OverSample OsrPressure { get; private set; } = BME280OverSample.X1;

        public BME280OverSample OsrHumidity { get; private set; } = BME280OverSample.X1;

        public BME280StandbyTime StandbyDuration { get; private set; } = BME280StandbyTime.Ms05;


        public void Initialize()
        {
            Reset();

            ChipId();

            LoadCalibration();

            WriteSettings();

            IsInitialized = true;
        }

        private void Reset()
        {
            _device.WriteRegister(Constants.BME280_REGISTER_SOFTRESET, 0xB6); 

            Thread.Sleep(300);
        }

        private void ChipId()
        {
            byte chipId = _device.ReadRegister(Constants.BME280_REGISTER_CHIPID);

            if (chipId != 0x60)
                throw new ApplicationException("Unrecognized chip");
        }

        private void LoadCalibration()
        {
            byte crc = _device.ReadRegister(0xE8); 

            var calibrationBuffer = new byte[33];

            var x1 = _device.ReadRegion(0x88, 27);
            var x2 = _device.ReadRegion(0xE1, 8);

            Array.Copy(x1, 0, calibrationBuffer, 0, 26);

            Array.Copy(x2, 0, calibrationBuffer, 26, 7);
                
             _calibration = new BME280CFData
            {
                T1 = BitConverter.ToUInt16(calibrationBuffer, 0),
                T2 = BitConverter.ToInt16(calibrationBuffer, 2),
                T3 = BitConverter.ToInt16(calibrationBuffer, 4),
                P1 = BitConverter.ToUInt16(calibrationBuffer, 6),
                P2 = BitConverter.ToInt16(calibrationBuffer, 8),
                P3 = BitConverter.ToInt16(calibrationBuffer, 10),
                P4 = BitConverter.ToInt16(calibrationBuffer, 12),
                P5 = BitConverter.ToInt16(calibrationBuffer, 14),
                P6 = BitConverter.ToInt16(calibrationBuffer, 16),
                P7 = BitConverter.ToInt16(calibrationBuffer, 18),
                P8 = BitConverter.ToInt16(calibrationBuffer, 20),
                P9 = BitConverter.ToInt16(calibrationBuffer, 22),
                H1 = calibrationBuffer[25],
                H2 = BitConverter.ToInt16(calibrationBuffer, 26),
                H3 = calibrationBuffer[28],
                H4 = (short)((calibrationBuffer[29] << 4) | (calibrationBuffer[30] & 0xF)),
                H5 = (short)((calibrationBuffer[31] << 4) | (calibrationBuffer[30] >> 4)),
                H6 = (sbyte)calibrationBuffer[32]
            };

           // if (crc != CalculateCrc(calibrationBuffer))
                //throw new ApplicationException("CRC error loading configuration.");
        }

        private static byte CalculateCrc(byte[] buffer)
        {
            uint crcReg = 0xFF;

            const byte polynomial = 0x1D;

            for (var index = 0; index < buffer.Length; index++)
            {
                for (byte bitNo = 0; bitNo < 8; bitNo++)
                {
                    byte din;

                    if (((crcReg & 0x80) > 0) ^ ((buffer[index] & 0x80) > 0))
                        din = 1;
                    else
                        din = 0;

                    crcReg = (ushort)((crcReg & 0x7F) << 1);

                    buffer[index] = (byte)((buffer[index] & 0x7F) << 1);

                    crcReg = (ushort)(crcReg ^ (polynomial * din));
                }
            }

            return (byte)(crcReg ^ 0xFF);
        }

        public void ChangeSettings(
            BME280SensorMode sensorMode = BME280SensorMode.Normal,
            BME280OverSample osrTemperature = BME280OverSample.X16,
            BME280OverSample osrPressure = BME280OverSample.X16,
            BME280OverSample osrHumidity = BME280OverSample.X16,
            BME280Filter filter = BME280Filter.Off,
            BME280StandbyTime standbyDuration = BME280StandbyTime.Ms05)
        {
            Filter = filter;
            SensorMode = sensorMode;
            OsrPressure = osrPressure;
            OsrHumidity = osrHumidity;
            OsrTemperature = osrTemperature;
            StandbyDuration = standbyDuration;

            WriteSettings();
        }

        private void WriteSettings()
        {
            var humiReg = (byte) OsrHumidity;

            var measReg = (byte)(((byte)OsrTemperature << 5) |
                                 ((byte)OsrPressure << 3) |
                                 (byte)SensorMode);

            var confReg = (byte)((byte)StandbyDuration << 5 | (byte)Filter << 3 | 0); 

            _device.WriteRegister(0xF2, humiReg);
            _device.WriteRegister(0xF5, confReg); 
            _device.WriteRegister(0xF4, measReg);
        }

        private void TakeForcedReading()
        {
            var measReg = (byte)(((byte)OsrTemperature << 5) |
                                 ((byte)OsrPressure << 3) |
                                 (byte)SensorMode);

            
            _device.WriteRegister(Constants.BME280_REGISTER_CONTROL, measReg); 

            Thread.Sleep(100);
        }

        public void Read()
        {
            if (SensorMode == BME280SensorMode.Forced)
            {
                TakeForcedReading();
            }

            var buffer = _device.ReadRegion(Constants.BME280_REGISTER_PRESSUREDATA, 8);

            _rawHumidity = buffer[7] | buffer[6] << 8;

            _rawPressure = buffer[0] << 12 | buffer[1] << 4 | buffer[2] >> 4;

            _rawTemperature = buffer[3] << 12 | buffer[4] << 4 | buffer[5] >> 4;

            var var1 = _rawTemperature / 16384.0 - _calibration.T1 / 1024.0;

            var1 = var1 * _calibration.T2;

            var var2 = _rawTemperature / 131072.0 - _calibration.T1 / 8192.0;

            var2 = var2 * var2 * _calibration.T3;

            _tFine = var1 + var2;
        }


        public double Temperature
        {
            get
            {
                const double temperatureMin = -40;
                const double temperatureMax = 85;

                var temperature = _tFine / 5120.0;

                if (temperature < temperatureMin)
                {
                    temperature = temperatureMin;
                }
                else if (temperature > temperatureMax)
                {
                    temperature = temperatureMax;
                }

                return temperature;
            }
        }

        public double Pressure
        {
            get
            {
                double pressure;

                const double pressureMin = 30000.0;
                const double pressureMax = 110000.0;

                var var1 = _tFine / 2.0 - 64000.0;

                var var2 = var1 * var1 * _calibration.P6 / 32768.0;

                var2 = var2 + var1 * _calibration.P5 * 2.0;

                var2 = var2 / 4.0 + _calibration.P4 * 65536.0;

                var var3 = _calibration.P3 * var1 * var1 / 524288.0;

                var1 = (var3 + _calibration.P2 * var1) / 524288.0;

                var1 = (1.0 + var1 / 32768.0) * _calibration.P1;

                if (var1 != 0)
                {
                    pressure = 1048576.0 - _rawPressure;

                    pressure = (pressure - var2 / 4096.0) * 6250.0 / var1;

                    var1 = _calibration.P9 * pressure * pressure / 2147483648.0;

                    var2 = pressure * _calibration.P8 / 2768.0;

                    pressure = pressure + (var1 + var2 + _calibration.P7) / 16.0;

                    if (pressure < pressureMin)
                    {
                        pressure = pressureMin;
                    }
                    else if (pressure > pressureMax)
                    {
                        pressure = pressureMax;
                    }
                }
                else
                {
                    pressure = pressureMin;
                }

                return pressure;
            }
        }

        public double Humidity
        {
            get
            {
                const double humidityMin = 0.0;
                const double humidityMax = 100.0;

                var var1 = _tFine - 76800.0;

                var var2 = _calibration.H4 * 64.0 + _calibration.H5 / 16384.0 * var1;

                var var3 = _rawHumidity - var2;

                var var4 = _calibration.H2 / 65536.0;

                var var5 = 1.0 + _calibration.H3 / 67108864.0 * var1;

                var var6 = 1.0 + _calibration.H6 / 67108864.0 * var1 * var5;

                var6 = var3 * var4 * (var5 * var6);

                var humidity = var6 * (1.0 - _calibration.H1 * var6 / 524288.0);

                if (humidity > humidityMax)
                {
                    humidity = humidityMax;
                }
                else if (humidity < humidityMin)
                {
                    humidity = humidityMin;
                }

                return humidity;
            }
        }
    }
}


```
