using Windows.Devices.Gpio;
using System;
using System.Threading;

//using nanoFramework.Hardware.Esp32;
//use to configure pins 

namespace Esp32nF
{
    public class Program
    {

        public static void Main()
        {
            // How to set Alternate pins for Devices ( COM1/2/3, SPI1/2, I2C, PWM ) then open device as normal
            //UART1
            //Configuration.SetPinFunction(9, DeviceFunction.COM1_RX);
            //Configuration.SetPinFunction(10, DeviceFunction.COM1_TX);
            //I2C
            //Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK); //I2C1 SCL
            //Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);  //I2C1 SDA
            //SPI1
            //Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
            //Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISI);
            //Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);
            //PWM
            //Configuration.SetPinFunction(36, DeviceFunction.PWM1);

            //======================================================//
            // 2 is a valid LED GPIO pin in ESP32S NodeMCU v1.1 DoIt
            //======================================================//

            GpioPin led = GpioController.GetDefault().OpenPin(2);
            
            led.SetDriveMode(GpioPinDriveMode.Output);
            
            int cnt = 0;

            while (true)
            {
                cnt++;

                led.Write(GpioPinValue.High);

                Thread.Sleep(300);
                led.Toggle();

                Thread.Sleep(100);
                led.Toggle();

                Thread.Sleep(300);
                led.Toggle();

                Thread.Sleep(300);
                Console.WriteLine("Counter = " + cnt.ToString());
            }
        }

        static int PinNumber(char port, byte pin)
        {
            if (port < 'A' || port > 'J')
                throw new ArgumentException();

            return ((port - 'A') * 16) + pin;
        }
    }
}