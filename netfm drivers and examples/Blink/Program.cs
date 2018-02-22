using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using STM32F411RE.Hardware;

namespace HelloWorld
{
    public class Program
    {
        public static void Main()
        {
            int cnt = 0;
            int PauzaNeMiliSekonda = 50;

			/* Pin.Led = PA5 = D13 on Nucleo */
            OutputPort LED = new OutputPort(Pin.Led, true);

            while (true)
            {
                LED.Write(!LED.Read());                       // Rezultati inverz

                Thread.Sleep(PauzaNeMiliSekonda);             // Prit pake ...

                Debug.Print("cnt = " + cnt.ToString());
                cnt++;
                if ((cnt % 2) == 0)
                {
                    Debug.Print("Div with 0");
                }
                else
                {
                    Debug.Print("No dived with 0");
                }
                 
            }
        }
    }
}
