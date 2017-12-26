//---------------------------------------------------------------------------
//<copyright file="Dht11Sensor.cs">
//
// Copyright 2011 Stanislav "CW" Simicek
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Modified from Valon Hoti
//</copyright>
//---------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace DHT
{
    /// <summary>
    /// Represents an instance of the DHT11 sensor.
    /// </summary>
    /// <remarks>
    /// Humidity measurement range 20 - 90%, accuracy ±4% at 25°C, ±5% at full range.
    /// Temperature measurement range 0 - 50°C, accuracy ±1-2°C.
    /// </remarks>
    public class Dht11Sensor : DhtSensor
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Dht11Sensor"/> class.
        /// </summary>
        /// <param name="pin1">The identifier for the sensor's data bus port.</param>
        /// <param name="pin2">The identifier for the sensor's data bus port.</param>
        /// <param name="pullUp">The pull-up resistor type.</param>
        /// <remarks>
        /// The ports identified by <paramref name="pin1"/> and <paramref name="pin2"/>
        /// must be wired together.
        /// </remarks>
        public Dht11Sensor(Cpu.Pin pin1, Cpu.Pin pin2, Port.ResistorMode pullUp)
            : base(pin1, pin2, pullUp)
        {
            // This constructor is intentionally left blank.
        }

        protected override int StartDelay
        {
            get
            {
                return 18;  // At least 18 ms
            }
        }

        protected override void Convert(byte[] data)
        {
            Debug.Assert(data != null);
            Debug.Assert(data.Length == 4);
            // DHT11 has 8-bit resolution, so the decimal part is always zero.
            Debug.Assert(data[1] == 0, "Humidity decimal part should be zero.");
            Debug.Assert(data[3] == 0, "Temperature decimal part should be zero.");

            Humidity = (float)data[0];
            Temperature = (float)data[2];
        }
    }

    public class Dht22Sensor : DhtSensor
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Dht22Sensor"/> class.
        /// </summary>
        /// <param name="pin1">The identifier for the sensor's data bus port.</param>
        /// <param name="pin2">The identifier for the sensor's data bus port.</param>
        /// <param name="pullUp">The pull-up resistor type.</param>
        /// <remarks>
        /// The ports identified by <paramref name="pin1"/> and <paramref name="pin2"/>
        /// must be wired together.
        /// </remarks>
        public Dht22Sensor(Cpu.Pin pin1, Cpu.Pin pin2, Port.ResistorMode pullUp)
            : base(pin1, pin2, pullUp)
        {
            // This constructor is intentionally left blank.
        }

        protected override int StartDelay
        {
            get
            {
                return 2; // At least 1 ms
            }
        }

        protected override void Convert(byte[] data)
        {
            Debug.Assert(data != null);
            Debug.Assert(data.Length == 4);

            // The first byte is integral, the second decimal part
            Humidity = ((data[0] << 8) | data[1]) * 0.1F;

            var temp = (((data[2] & 0x7F) << 8) | data[3]) * 0.1F;
            Temperature = (data[2] & 0x80) == 0 ? temp : -temp; // MSB = 1 means negative
        }
    }


    public abstract class DhtSensor : IDisposable
    {
        private bool disposed;

        private InterruptPort portIn;
        private TristatePort portOut;

        private float rhum; // Relative Humidity
        private float temp; // Temperature
       

        private long data;
        private long bitMask;
        private long lastTicks;
        private byte[] bytes = new byte[4];

        private AutoResetEvent dataReceived = new AutoResetEvent(false);

        // Instantiated via derived class
        protected DhtSensor(Cpu.Pin pin1, Cpu.Pin pin2, Port.ResistorMode pullUp)
        {
            var resistorMode = (Port.ResistorMode)pullUp;

            portIn = new InterruptPort(pin2, false, resistorMode, Port.InterruptMode.InterruptEdgeLow);
            portIn.OnInterrupt += new NativeEventHandler(portIn_OnInterrupt);
            portIn.DisableInterrupt();  // Enabled automatically in the previous call

            portOut = new TristatePort(pin1, true, false, resistorMode);

            if (!CheckPins())
            {
                //throw new InvalidOperationException("DHT sensor pins are not connected together.");
                Debug.Print("DHT sensor pins are not connected together.\n");
            }
        }

        /// <summary>
        /// Deletes an instance of the <see cref="DhtSensor"/> class.
        /// </summary>
        ~DhtSensor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources used by this <see cref="DhtSensor"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources associated with the <see cref="DhtSensor"/> object.
        /// </summary>
        /// <param name="disposing">
        /// <b>true</b> to release both managed and unmanaged resources;
        /// <b>false</b> to release only unmanaged resources.
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    portIn.Dispose();
                    portOut.Dispose();
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        /// <summary>
        /// Gets the measured temperature value.
        /// </summary>
        public float Temperature
        {
            get
            {
                return temp;
            }
            protected set
            {
                temp = value;
            }
        }

        public float TemperatureKelvin
        {
           get
            {
            return (float)(this.temp + 273.15);
            }       
        }

        public float TemperatureFarenheit
        {
            get
            {
               return (this.temp * 9 / 5 + 32);
            }         
        }
        /// <summary>
        /// Gets the measured relative humidity value.
        /// </summary>
        public float Humidity
        {
            get
            {
                return rhum;
            }
            protected set
            {
                rhum = value;
            }
        }

     public float CalcDewPoint
     {
         get
          {
            float celsius = this.temp;
            float humidity = this.rhum;
            float a = (float)17.271;
            float b = (float)237.7;
            float temp = (float)((a * celsius) / (b + celsius) + System.Math.Log(humidity / 100));
            float Td = (b * temp) / (a - temp);
            return Td;
         }         
     }


        

        /// <summary>
        /// Gets the start delay, in milliseconds.
        /// </summary>
        protected abstract int StartDelay
        {
            get;
        }

        /// <summary>
        /// Converts raw sensor data.
        /// </summary>
        /// <param name="data">The sensor raw data, excluding the checksum.</param>
        /// <remarks>
        /// If the checksum verification fails, this method is not called.
        /// </remarks>
        protected abstract void Convert(byte[] data);

        /// <summary>
        /// Retrieves measured data from the sensor.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the operation succeeds and the data is valid, otherwise <c>false</c>.
        /// </returns>
        public bool Read()
        {
            if (disposed)
            {
                throw new ObjectDisposedException();
            }
            // The 'bitMask' also serves as edge counter: data bit edges plus
            // extra ones at the beginning of the communication (presence pulse).
            bitMask = 1L << 41;

            data = 0;
            // lastTicks = 0; // This is not really needed, we measure duration
            // between edges and the first three values are ignored anyway.

            // Initiate communication
            portOut.Active = true;
            portOut.Write(false);       // Pull bus low
            Thread.Sleep(StartDelay);
            portIn.EnableInterrupt();   // Turn on the receiver
            portOut.Active = false;     // Release bus

            bool dataValid = false;

            // Now the interrupt handler is getting called on each falling edge.
            // The communication takes up to 5 ms, but the interrupt handler managed
            // code takes longer to execute than is the duration of sensor pulse
            // (interrupts are queued), so we must wait for the last one to finish
            // and signal completion. 20 ms should be enough, 50 ms is safe.
            if (dataReceived.WaitOne(50, false))
            {
                // TODO: Use two short-s ?
                bytes[0] = (byte)((data >> 32) & 0xFF);
                bytes[1] = (byte)((data >> 24) & 0xFF);
                bytes[2] = (byte)((data >> 16) & 0xFF);
                bytes[3] = (byte)((data >> 8) & 0xFF);

                byte checksum = (byte)(bytes[0] + bytes[1] + bytes[2] + bytes[3]);
                if (checksum == (byte)(data & 0xFF))
                {
                    dataValid = true;

                    if (bytes[0] == 0)
                        portIn.DisableInterrupt();
                    else
                        Convert(bytes);

 
                }
                else
                {
                    Debug.Print("DHT sensor data has invalid checksum.");
                }
            }
            else
            {
                portIn.DisableInterrupt();  // Stop receiver
                Debug.Print("DHT sensor data timeout.");  // TODO: TimeoutException?
            }
            return dataValid;
        }

        // If the received data has invalid checksum too often, adjust this value
        // based on the actual sensor pulse durations. It may be a little bit
        // tricky, because the resolution of system clock is only 21.33 µs.
        private const long BitThreshold = 1050;

        private void portIn_OnInterrupt(uint pin, uint state, DateTime time)
        {
            var ticks = time.Ticks;
            if ((ticks - lastTicks) > BitThreshold)
            {
                // If the time between edges exceeds threshold, it is bit '1'
                data |= bitMask;
            }
            if ((bitMask >>= 1) == 0)
            {
                // Received the last edge, stop and signal completion
                portIn.DisableInterrupt();
                dataReceived.Set();
            }
            lastTicks = ticks;
        }

        // Returns true if the ports are wired together, otherwise false.
        private bool CheckPins()
        {
            Debug.Assert(portIn != null, "Input port should not be null.");
            Debug.Assert(portOut != null, "Output port should not be null.");
            Debug.Assert(!portOut.Active, "Output port should not be active.");

            portOut.Active = true;  // Switch to output
            portOut.Write(false);
            var expectedFalse = portIn.Read();
            portOut.Active = false; // Switch to input
            var expectedTrue = portIn.Read();
            return (expectedTrue && !expectedFalse);
        }
    }
}
