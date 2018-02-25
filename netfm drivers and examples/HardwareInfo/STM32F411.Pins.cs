using Microsoft.SPOT.Hardware;
using System.IO.Ports;

namespace STM32F411RE.Hardware
{

    public class STM32F411RE
    {
        static STM32F411RE()
        {
            HardwareProvider.Register(new STM32F411REHardwareProvider());
        }

        private sealed class STM32F411REHardwareProvider : HardwareProvider
        {
        }
    }

    /// <summary>
    /// Pin definitions for the STM32F411RE board
    /// </summary>
    public class Pin
    {
        /// <summary>A value indicating that no GPIO pin is specified.</summary>
        public const Cpu.Pin GPIO_NONE = Cpu.Pin.GPIO_NONE;

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA0 = (Cpu.Pin)((0 * 16) + 0);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA1 = (Cpu.Pin)((0 * 16) + 1);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA2 = (Cpu.Pin)((0 * 16) + 2);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA3 = (Cpu.Pin)((0 * 16) + 3);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA4 = (Cpu.Pin)((0 * 16) + 4);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA5 = (Cpu.Pin)((0 * 16) + 5);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA6 = (Cpu.Pin)((0 * 16) + 6);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA7 = (Cpu.Pin)((0 * 16) + 7);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA8 = (Cpu.Pin)((0 * 16) + 8);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA9 = (Cpu.Pin)((0 * 16) + 9);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA10 = (Cpu.Pin)((0 * 16) + 10);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA11 = (Cpu.Pin)((0 * 16) + 11);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA12 = (Cpu.Pin)((0 * 16) + 12);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA13 = (Cpu.Pin)((0 * 16) + 13);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA14 = (Cpu.Pin)((0 * 16) + 14);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PA15 = (Cpu.Pin)((0 * 16) + 15);


        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB0 = (Cpu.Pin)((1 * 16) + 0);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB1 = (Cpu.Pin)((1 * 16) + 1);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB2 = (Cpu.Pin)((1 * 16) + 2);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB3 = (Cpu.Pin)((1 * 16) + 3);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB4 = (Cpu.Pin)((1 * 16) + 4);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB5 = (Cpu.Pin)((1 * 16) + 5);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB6 = (Cpu.Pin)((1 * 16) + 6);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB7 = (Cpu.Pin)((1 * 16) + 7);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB8 = (Cpu.Pin)((1 * 16) + 8);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB9 = (Cpu.Pin)((1 * 16) + 9);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB10 = (Cpu.Pin)((1 * 16) + 10);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB11 = (Cpu.Pin)((1 * 16) + 11);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB12 = (Cpu.Pin)((1 * 16) + 12);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB13 = (Cpu.Pin)((1 * 16) + 13);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB14 = (Cpu.Pin)((1 * 16) + 14);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PB15 = (Cpu.Pin)((1 * 16) + 15);



        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC0 = (Cpu.Pin)((2 * 16) + 0);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC1 = (Cpu.Pin)((2 * 16) + 1);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC2 = (Cpu.Pin)((2 * 16) + 2);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC3 = (Cpu.Pin)((2 * 16) + 3);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC4 = (Cpu.Pin)((2 * 16) + 4);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC5 = (Cpu.Pin)((2 * 16) + 5);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC6 = (Cpu.Pin)((2 * 16) + 6);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC7 = (Cpu.Pin)((2 * 16) + 7);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC8 = (Cpu.Pin)((2 * 16) + 8);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC9 = (Cpu.Pin)((2 * 16) + 9);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC10 = (Cpu.Pin)((2 * 16) + 10);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC11 = (Cpu.Pin)((2 * 16) + 11);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC12 = (Cpu.Pin)((2 * 16) + 12);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC13 = (Cpu.Pin)((2 * 16) + 13);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC14 = (Cpu.Pin)((2 * 16) + 14);

        /// <summary>Digital I/O.</summary>
        public const Cpu.Pin PC15 = (Cpu.Pin)((2 * 16) + 15);

        /* internal on firmware for analog */
        // PA0,PA1,PA2,PA3,PA4,PB0,PC0,PC1,PC2,PC3
        public const Cpu.Pin Analog1 = PA0;
		public const Cpu.Pin Analog2 = PA1;
		public const Cpu.Pin Analog3 = PA2;
		public const Cpu.Pin Analog4 = PA3;
		public const Cpu.Pin Analog5 = PA4;
		public const Cpu.Pin Analog6 = PB0;
		public const Cpu.Pin Analog7 = PC0;
		public const Cpu.Pin Analog8 = PC1;
		public const Cpu.Pin Analog9 = PC2;
		public const Cpu.Pin Analog10 = PC3;

        /* internal on firmware for PWM */
        //{0,1,5,6,8,9,10,11,15,19,20,21,22,23,24,25,26,38,39,40,41} 
        //A0,A1,A5,A8,A9,A10,A11,A15,B3,B4,B5,B6,B7,B8,B9,B10,C6,C7,C8,C9
        public const Cpu.Pin PWM1 = PA0;
		public const Cpu.Pin PWM2 = PA1;
		public const Cpu.Pin PWM3 = PA5;
		public const Cpu.Pin PWM4 = PA6;
		public const Cpu.Pin PWM5 = PA8;
		public const Cpu.Pin PWM6 = PA9;
		public const Cpu.Pin PWM7 = PA10;
        public const Cpu.Pin PWM8 = PA11;
        public const Cpu.Pin PWM9 = PA15;
        public const Cpu.Pin PWM10 = PB3; //19
        public const Cpu.Pin PWM11 = PB4; //20
        public const Cpu.Pin PWM12 = PB5; //21
        public const Cpu.Pin PWM13 = PB6; //22
        public const Cpu.Pin PWM14 = PB7; //23
        public const Cpu.Pin PWM15 = PB8; //24
        public const Cpu.Pin PWM16 = PB9; //25
      //  public const Cpu.Pin PWM17 = PB10; //26
      //  public const Cpu.Pin PWM18 = PC6; //38
      //  public const Cpu.Pin PWM19 = PC7; //39
      //  public const Cpu.Pin PWM20 = PC8; //40
      //  public const Cpu.Pin PWM21 = PC9; //41
        
        /* internal on firmware for SPI */
        public const Cpu.Pin SCLK1 = PA5;
		public const Cpu.Pin MISO1 = PA6;
		public const Cpu.Pin MOSI1 = PA7;
		public const Cpu.Pin SS1   = PB6;
		
		public const Cpu.Pin SCLK2 = PB15;
		public const Cpu.Pin MISO2 = PB14;
		public const Cpu.Pin MOSI2 = PB13;
		public const Cpu.Pin SS2   = PA10;
		
		/* internal on firmware for I2C */
		public const Cpu.Pin SCL   = PB8;
		public const Cpu.Pin SDA   = PB9;
		
		/* internal on firmware for UART */
		public const Cpu.Pin RX_UART1   = PA10;
		public const Cpu.Pin TX_UART1   = PA9;
		
		public const Cpu.Pin RX_UART2   = PA3;
		public const Cpu.Pin TX_UART2   = PA2;
		
		/* arduino headers */
		public const Cpu.Pin A0 = PA0;   //Analog
		public const Cpu.Pin A1 = PA1;   //Analog
		public const Cpu.Pin A2 = PA4;   //Analog
		public const Cpu.Pin A3 = PB0;   //Analog
		public const Cpu.Pin A4 = PC1;   //Analog
		public const Cpu.Pin A5 = PC0;   //Analog
	
		public const Cpu.Pin D0 = PA3;   //UART2 - RX
		public const Cpu.Pin D1 = PA2;   //UART2 - TX
		public const Cpu.Pin D2 = PA10;  //UART1 - RX
		public const Cpu.Pin D3 = PB3;
		public const Cpu.Pin D4 = PB5;
		public const Cpu.Pin D5 = PB4;
		public const Cpu.Pin D6 = PB10;
		public const Cpu.Pin D7 = PA8;
		public const Cpu.Pin D8 = PA9;   //UART1 - TX
		public const Cpu.Pin D9 = PC7;
		
		public const Cpu.Pin D10 = PB6;  //CS1
		public const Cpu.Pin D11 = PA7;  //MOSI1
		public const Cpu.Pin D12 = PA6;  //MISO1
		public const Cpu.Pin D13 = PA5;  //SCK1
		
		public const Cpu.Pin D14 = PB9;  //SDA
	    public const Cpu.Pin D15 = PB8;  //SCL
		
		public const Cpu.Pin UserButton = PC13;  
		public const Cpu.Pin Led = PA5;

        public static string GetPinName(Cpu.Pin pin)
        {
            if (pin == Cpu.Pin.GPIO_NONE)
                return "GPIO_NONE";

            var pinNumber = (int)pin;

            int port = pinNumber / 16;
            int num = pinNumber - 16 * port;
            string result = "P" + (char)('A' + port) + num;
            return result;
        }

        static int PinNumber(char port, byte pin)
        {
            if (port < 'A' || port > 'E') return 65535;

            return ((port - 'A') * 16) + pin;
        }

    }

   

    public static class SerialPorts
    {
        public const string COM1 = Serial.COM1;
        public const string COM2 = Serial.COM2;
        public const string COM3 = Serial.COM3;
    }
    public static class BaudRates
    {
        public const BaudRate BaudrateNONE = BaudRate.BaudrateNONE;
        public const BaudRate Baud75 = BaudRate.Baudrate75;
        public const BaudRate Baud150 = BaudRate.Baudrate150;
        public const BaudRate Baud300 = BaudRate.Baudrate300;
        public const BaudRate Baud600 = BaudRate.Baudrate600;
        public const BaudRate Baud1200 = BaudRate.Baudrate1200;
        public const BaudRate Baud2400 = BaudRate.Baudrate2400;
        public const BaudRate Baud4800 = BaudRate.Baudrate4800;
        public const BaudRate Baud9600 = BaudRate.Baudrate9600;
        public const BaudRate Baud19200 = BaudRate.Baudrate19200;
        public const BaudRate Baud38400 = BaudRate.Baudrate38400;
        public const BaudRate Baud57600 = BaudRate.Baudrate57600;
        public const BaudRate Baud115200 = BaudRate.Baudrate115200;
        public const BaudRate Baud230400 = BaudRate.Baudrate230400;
    }


    public class PwmOutput
    {
       // A0,A1,A5,A8,A9,A10,A11,A15,B3,B4,B5,B6,B7,B8,B9,B10,C6,C7,C8,C9
        public const Cpu.PWMChannel PA0 = Cpu.PWMChannel.PWM_0;
        public const Cpu.PWMChannel PA1 = Cpu.PWMChannel.PWM_1;
        public const Cpu.PWMChannel PA5 = Cpu.PWMChannel.PWM_2;
        public const Cpu.PWMChannel PA8 = Cpu.PWMChannel.PWM_3;
        public const Cpu.PWMChannel PA9 = Cpu.PWMChannel.PWM_4;
        public const Cpu.PWMChannel PA10 = Cpu.PWMChannel.PWM_5;
        public const Cpu.PWMChannel PA11 = Cpu.PWMChannel.PWM_6;
        public const Cpu.PWMChannel PA15 = Cpu.PWMChannel.PWM_7;
        public const Cpu.PWMChannel PB3 = Cpu.PWMChannel.PWM_8;
        public const Cpu.PWMChannel PB4 = Cpu.PWMChannel.PWM_9;
        public const Cpu.PWMChannel PB5 = Cpu.PWMChannel.PWM_10;
        public const Cpu.PWMChannel PB6 = Cpu.PWMChannel.PWM_11;
        public const Cpu.PWMChannel PB7 = Cpu.PWMChannel.PWM_12;
        public const Cpu.PWMChannel PB8 = Cpu.PWMChannel.PWM_13;
        public const Cpu.PWMChannel PB9 = Cpu.PWMChannel.PWM_14;
        public const Cpu.PWMChannel PB10 = Cpu.PWMChannel.PWM_15;

        /* CPU.PWMChannel max = 15 from Microsoft Definated */
       // public const Cpu.PWMChannel PC6 = Cpu.PWMChannel.PWM_15;
       // public const Cpu.PWMChannel PC7 = Cpu.PWMChannel.PWM_15;
       // public const Cpu.PWMChannel PC8 = Cpu.PWMChannel.PWM_15;
       // public const Cpu.PWMChannel PC9 = Cpu.PWMChannel.PWM_15;
    }
}