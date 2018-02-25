/*
 * https://github.com/kodfilemon/kodfilemon.blogspot.com/tree/master/STM32F4Discovery/Demo/DemoHardwareInfo/Helpers 
 */

using System;
using System.IO.Ports;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Hardware.UsbClient;

using STM32F411RE.Hardware;

namespace HelloWorld
{
    
    public class Program
    {
        public static void Main()
        {
            Debug.Print("\n\n");
            Debug.Print("Hello world - i'm trying to find what are hidden to this firmware ");
            Debug.Print("\n\n");

            HardwareProvider provider = HardwareProvider.HwProvider;

            var cnt = provider.GetAnalogChannelsCount();
            for (int i = 0; i < cnt; i++)
            {
                var channel = (Cpu.AnalogChannel)i;
                Cpu.Pin pin = provider.GetAnalogPinForChannel(channel);
                int[] precisions = provider.GetAvailablePrecisionInBitsForChannel(channel);

                Debug.Print("AnalogChannel" + channel + ": pin=" + pin.ToString() + "("+STM32F411RE.Hardware.Pin.GetPinName(pin)+"): precisions=" + precisions[0].ToString()+" bit");
            }
            Debug.Print("\n\n");

            cnt = 0;
            cnt = provider.GetAnalogOutputChannelsCount();
            for (int i = 0; i < cnt; i++)
            {
                var channel = (Cpu.AnalogOutputChannel)i;
                Cpu.Pin pin = provider.GetAnalogOutputPinForChannel(channel);
                int[] precisions = provider.GetAvailableAnalogOutputPrecisionInBitsForChannel(channel);

                Debug.Print("AnalogOutputChannel" + ": pin=" + pin.ToString() + "(" + STM32F411RE.Hardware.Pin.GetPinName(pin) + "): precisions=" + precisions[0].ToString() + " bit");
            }
            Debug.Print("\n\n");

            /* find pwm */
            cnt = 0;
            cnt = provider.GetPWMChannelsCount();
            for (int i = 0; i < cnt; i++)
            {
                var channel = (Cpu.PWMChannel)i;
                Cpu.Pin pin = provider.GetPwmPinForChannel(channel);
                Debug.Print("PWMChannel" + channel + ": pin=" + pin.ToString() +"("+STM32F411RE.Hardware.Pin.GetPinName(pin)+")");
            }
            Debug.Print("\n\n");

            /*find uart */
            cnt = 0;
            cnt = provider.GetSerialPortsCount();
            for (int i = 0; i < cnt; i++)
            {
              
                string comPort = Serial.COM1.Substring(0, 3) + (i + 1);
                Cpu.Pin rxPin, txPin, ctsPin, rtsPin;
                provider.GetSerialPins(comPort, out rxPin, out txPin, out ctsPin, out rtsPin);
                uint max, min;
                provider.GetBaudRateBoundary(i, out max, out min);

                Debug.Print(comPort + ": (rx, tx, cts, rts)=(" +
                            STM32F411RE.Hardware.Pin.GetPinName(rxPin) + ", " +
                            STM32F411RE.Hardware.Pin.GetPinName(txPin) + ", " +
                            STM32F411RE.Hardware.Pin.GetPinName(ctsPin) + ", " +
                            STM32F411RE.Hardware.Pin.GetPinName(rtsPin) + ")" +
                            " baud=" + min + "..." + max);
            }
            Debug.Print("\n\n");
            /* find i2c */
            Cpu.Pin i2cscl, i2csda;
            provider.GetI2CPins(out i2cscl, out i2csda);
            Debug.Print("I2C module:(scl value="+ i2cscl.ToString()+"("+STM32F411RE.Hardware.Pin.GetPinName(i2cscl) + "),sda="+ i2csda.ToString()+"("+ STM32F411RE.Hardware.Pin.GetPinName(i2csda) + "));");
            Debug.Print("\n\n");

            /* find SPI */
            var spicnt = provider.GetSpiPortsCount();
            for (int i = 0; i < spicnt; i++)
            {
                var module = (SPI.SPI_module)i;

                Cpu.Pin msk, miso, mosi;
                provider.GetSpiPins(module, out msk, out miso, out mosi);
                Debug.Print("SPI_module" + (i + 1) + ": (msk=" + msk.ToString() + "("+ STM32F411RE.Hardware.Pin.GetPinName(msk) + "), miso=" + miso.ToString() + "("+ STM32F411RE.Hardware.Pin.GetPinName(miso) + "), mosi=" + mosi.ToString() + "("+ STM32F411RE.Hardware.Pin.GetPinName(mosi) + "));");
            }
            Debug.Print("\n\n");

            UsbController[] controllers = UsbController.GetControllers();
            for (int i = 0; i < controllers.Length; i++)
            {
                Debug.Print("USB" + i + ": " + Convert(controllers[i].Status));
                Thread.Sleep(500);
            }

            Debug.Print("\n\n");
            Debug.Print("Finished check of hardware ");
        }

        private static string Convert(UsbController.PortState status)
        {
            if (status == UsbController.PortState.Stopped)
                return "stopped";

            if (status == UsbController.PortState.Running)
                return "running";

            return status.ToString();
        }
    }
}
