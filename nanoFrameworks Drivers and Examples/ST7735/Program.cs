using System;
using System.Threading;
using STM32F4.Pins;
using nanoFramework.Driver;


namespace NFApp5
{
    public class Program
    {
         public static void Main()
         {
            

            ST7735 display = new ST7735(
               NUCLEOF411.Gpio.D8,                 //Reset
               NUCLEOF411.Gpio.D7,                 //BackLight 
               NUCLEOF411.Gpio.D9,                 //A0 (DC) Control Pin / Data Command
               NUCLEOF411.SpiDevice.Sp2.Name,      //SPI SCK/MOSI 
               NUCLEOF411.Gpio.D10                 //chipSelect
              );                          

            display.TurnOn();

            short i = 0;

            display.DrawCircle(20, 20, 20, Color.Red);
            display.DrawRectangle(40, 40, 40, 40, Color.Cyan);
            display.DrawFilledRectangle(80, 80, 40, 40, Color.Blue);
            display.DrawText(10, 30, "Hello nanoFramework", Color.Green);
            display.DrawText(30, 60, "from ST7735 SPI", Color.Green);

            while (true)
            {
                i++;
                display.DrawText(10, 10, i.ToString(),Color.Green);

                Thread.Sleep(500);
            }
        }
        
    }
}
