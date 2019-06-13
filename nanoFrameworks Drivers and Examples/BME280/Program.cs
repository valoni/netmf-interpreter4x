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
