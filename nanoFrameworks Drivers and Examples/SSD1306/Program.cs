using System;
using System.Threading;
using nanoFramework.Hardware.Esp32;

using nanoFramework.Driver.SSD1306;

namespace ESP32_SSD1306
{
    public class Program
    {
        private static SSD1306 oled;

        public static void Main()
        {

            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            oled = new SSD1306("I2C1", 128, 64, 0x3C);
            oled.Init();

            Console.WriteLine("---------------------");
            Console.WriteLine("Start SSD1306 Display");
            Console.WriteLine("---------------------");
            oled.Display();
            Thread.Sleep(500);

            oled.Clear();
            Thread.Sleep(500);

            while (true)
            {
                SSD1306Demo();
            }
        }

        public static void SSD1306Demo()
        {
             Console.WriteLine("Two Circle");
            //two circles
            oled.DrawCircle(31, 31, 30);
            oled.DrawFilledCircle(97, 31, 30);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();

            Console.WriteLine("Archery Target");
            //archery target
            oled.DrawFilledCircle(63, 31, 30);
            oled.DrawCircle(63, 31, 25, false);
            oled.DrawCircle(63, 31, 20, false);
            oled.DrawCircle(63, 31, 15, false);
            oled.DrawCircle(63, 31, 10, false);
            oled.DrawCircle(63, 31, 5, false);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();

            Console.WriteLine("Invert");
            oled.SetInverseDisplay(true);
            Thread.Sleep(3000);
            oled.SetInverseDisplay(false);

            Console.WriteLine("Rectangles");
            oled.DrawRectangle(5, 20, 40, 40);
            oled.DrawFilledRectangle(50, 20, 50, 40);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();

            Console.WriteLine("Rounded Rect");
            oled.DrawRoundRect(5, 20, 40, 40, 10);
            oled.DrawRoundFilledRect(50, 20, 50, 40, 10);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();

            Console.WriteLine("Scrolling");
            oled.StartScrollHorizontally(true, 0, 0xff);
            Thread.Sleep(3000);

            oled.StartScrollHorizontally(false, 0, 0xff);
            Thread.Sleep(3000);
            oled.StartScrollVerticallyHorizontally(true, 0, 0xff, 0x02);
            Thread.Sleep(3000);
            oled.StartScrollVerticallyHorizontally(true, 0, 0xff, 0x0A);
            Thread.Sleep(3000);
            oled.DeactivateScroll();

            oled.Clear();

            Console.WriteLine("Triangle and Line");
            oled.DrawLine(10, 10, oled.Width - 10, oled.Height - 10);
            oled.DrawTriangle(5, 20, 5, 60, 63, 60);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();


            Console.WriteLine("Text smaller");
            oled.DrawText(0, 0, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), 1);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();

            Console.WriteLine("Text bigger");
            oled.DrawText(0, 0, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), 2);

            oled.Display();
            Thread.Sleep(3000);
            oled.Clear();

            Console.WriteLine("---------------------");

        }

    }
}
