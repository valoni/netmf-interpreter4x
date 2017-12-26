using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace AnalogPins
{
    public class Program
    {
        public static void Main()
        {
            // {0,1,4,5,6,7,16,32,33,34} 
            // PA0,PA1,PA4,PA5,PA6,PA7,PB0,PC0,PC1,PC2
            using (var analogInput = new AnalogInput(Cpu.AnalogChannel.ANALOG_1))
            {
                for (;;)
                {
                    double readVal = analogInput.Read();
                    int rawVal = analogInput.ReadRaw();
                    Debug.Print("readVal: " + readVal.ToString() + " ( rawVal " + rawVal.ToString() + ")");

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
