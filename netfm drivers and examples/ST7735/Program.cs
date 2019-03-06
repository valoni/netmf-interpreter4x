using System;
using Microsoft.SPOT;
using System.Threading;

using STM32F411RE;

using microFramework.Driver;

namespace Test
{
    public class Program
    {
        public static void Main()
        {
            ST7735 display = new ST7735(
                 STM32F411RE.Hardware.ArduinoPin.D8,                //Reset
                 STM32F411RE.Hardware.ArduinoPin.D7,                //BackLight 
                 STM32F411RE.Hardware.ArduinoPin.D9,                //A0 (DC) Control Pin / Data Command
                 STM32F411RE.Hardware.SpiDevices.SPI2,              //SPI SCK/MOSI 
                 STM32F411RE.Hardware.ArduinoPin.D10                //chipSelect
               );

            display.TurnOn();

            short i = 0;

            display.DrawCircle(20, 20, 20, Color.Red);
            display.DrawRectangle(40, 40, 40, 40, Color.Cyan);
            display.DrawFilledRectangle(80, 80, 40, 40, Color.Blue);
            display.DrawText(10, 30, "Hello .NET Microframework", Color.Green);
            display.DrawText(30, 60, "from ST7735 SPI", Color.Green);

            while (true)
            {
                i++;
                display.DrawText(10, 10, i.ToString(), Color.Green);

                Thread.Sleep(500);
            }
        }
    }
}
