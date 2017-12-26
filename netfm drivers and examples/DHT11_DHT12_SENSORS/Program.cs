using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

using STM32F411RE.Hardware;

namespace DHT
{
    public class Program
    {
		/* library of this code was done from CW_2 */
        public static void Main()
        {

                                                   /*   D2     D3    */
            Dht11Sensor dhtSensor = new Dht11Sensor(Pin.PA10, Pin.PB3, Port.ResistorMode.PullUp);

            int cnt = 0; /* just a simple counter */
             Debug.Print("----------------------------------------");

            /* loopa e ngjarjeve dhe funksionet */
            while (true)
            {
              cnt++;

              if (dhtSensor.Read())
              {
                    Debug.Print(cnt.ToString()+":");
                    Debug.Print("----------------------------------------");
                   
			        Debug.Print("Temp Celsius   = " + dhtSensor.Temperature.ToString("F1") + "°C"); 
                    Debug.Print("Temp Kelvin    = " + dhtSensor.TemperatureKelvin.ToString("F1") + "°K");
                    Debug.Print("Temp Farenhein = " + dhtSensor.TemperatureFarenheit.ToString("F1") + "°F");
                    Debug.Print(String.Empty);

                    Debug.Print("Humidity       = " + dhtSensor.Humidity.ToString("F1")+ " %");
                    Debug.Print(String.Empty);
                   
                    Thread.Sleep(2000);
                
              }
              else
              {
                  /* first time always fail than it correct itself */
                 Debug.Print("DHT sensor Read() failed");
                  Thread.Sleep(2000);

              }
            }

        }
    }
}