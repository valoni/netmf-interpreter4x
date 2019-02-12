//
// Copyright (c) 2019 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//
using System;

namespace STM32F4.Pins
{
    public static class Pinouts
    { 
    static int PinNumber(char port, byte pin)
    {
            if (port < 'A' || port > 'J')
                throw new ArgumentException();

            return ((port - 'A') * 16) + pin;
        }
    }

    public static class NUCLEOF411
    {
        /// <summary>
        /// Enumeration of ADC channels available on ST_NUCLEO64_F411RE_NF
        /// </summary>
        public static class AdcChannels
        {
            /// <summary>
            /// Channel 0, exposed as A0, connected to pin 1 on CN8 = PA0 (ADC1 - IN0)
            /// </summary>
            public const int Channel_0 = 0;

            /// <summary>
            /// Channel 1, exposed as A1, connected to pin 2 on CN8 = PA1 (ADC1 - IN1)
            /// </summary>
            public const int Channel_1 = 1;

            /// <summary>
            /// Channel 2, exposed as A2, connected to pin 3 on CN8 = PA4 (ADC1 - IN4)
            /// </summary>
            public const int Channel_2 = 2;

            /// <summary>
            /// Channel 3, exposed as A3, connected to pin 4 on CN8 = PB0 (ADC1 - IN8)
            /// </summary>
            public const int Channel_3 = 3;

            /// <summary>
            /// Channel 4, exposed as A4, connected to pin 5 on CN8 = PC1 (ADC1 - IN11)
            /// </summary>
            public const int Channel_4 = 4;

            /// <summary>
            /// Channel 5, exposed on A5, connected to pin 6 on CN8 = PC0 (ADC1 - IN10)
            /// </summary>
            public const int Channel_5 = 5;

            /// <summary>
            /// Channel 6, internal temperature sensor, connected to ADC1
            /// </summary>
            public const int Channel_TemperatureSensor = 6;

            /// <summary>
            /// Channel 7, internal voltage reference, connected to ADC1
            /// </summary>
            public const int Channel_VrefIn = 7;

            /// <summary>
            /// Channel 8, connected to VBatt pin, ADC1
            /// </summary>
            public const int Channel_Vbatt = 8;
        }

        /// <summary>
        ///  GPIO class represent mapped info and values on 
        ///  Arduino Headers as A0 ... A5 and D0 ... D15 and
        ///  Morpho Headers as PA0 .... PC15 ...
        /// </summary>

        public static class Gpio 
        {
            public const int PA0 = 0;   public const int A0 = 0;
            public const int PA1 = 1;   public const int A1 = 1;  
            public const int PA2 = 2;   public const int D1 = 2;
            public const int PA3 = 3;   public const int D0 = 3;
            public const int PA4 = 4;   public const int A2 = 4;
            public const int PA5 = 5;   public const int D13 = 5;  public const int SCK = 5;     public const int Led1 = 5;
            public const int PA6 = 6;   public const int D12 = 6;  public const int MISO = 6;
            public const int PA7 = 7;   public const int D11 = 7;  public const int MOSI = 7;
            public const int PA8 = 8;   public const int D7 = 8;
            public const int PA9 = 9;   public const int D8 = 9;
            public const int PA10 = 10; public const int D2 = 10;
            public const int PA11 = 11;
            public const int PA12 = 12;
            public const int PA13 = 13;
            public const int PA14 = 14;
            public const int PA15 = 15;

            public const int PB0 = 16;  public const int A3 = 16;
            public const int PB1 = 17; 
            public const int PB2 = 18; 
            public const int PB3 = 19;  public const int D3 = 19;
            public const int PB4 = 20;  public const int D5 = 20;
            public const int PB5 = 21;  public const int D4 = 21;
            public const int PB6 = 22;  public const int D10 = 22;
            public const int PB7 = 23; 
            public const int PB8 = 24;  public const int D15 = 24;  public const int SCL = 24;
            public const int PB9 = 25;  public const int D14 = 25;  public const int SDA = 25;
            public const int PB10 = 26; public const int D6 = 26;
            public const int PB11 = 27;
            public const int PB12 = 28;
            public const int PB13 = 28;
            public const int PB14 = 30;
            public const int PB15 = 31;

            public const int PC0 = 32;  public const int A5 = 32;
            public const int PC1 = 33;  public const int A4 = 22;
            public const int PC2 = 34;
            public const int PC3 = 35;
            public const int PC4 = 36;
            public const int PC5 = 37;
            public const int PC6 = 38;
            public const int PC7 = 39;  public const int D9 = 39;
            public const int PC8 = 40;
            public const int PC9 = 41;
            public const int PC10 = 42;
            public const int PC11 = 43;
            public const int PC12 = 44;
            public const int PC13 = 45; public const int Buttonn1 = 45;
            public const int PC14 = 46;
            public const int PC15 = 47;

        }

        /// <summary>
        ///  this class is represented SPI Device on Nucleo F411 
        /// </summary>
        public static class SpiDevice
        {
            public static class Sp2
            {
                public const string Name = "SPI2";
                public const int MOSI = Gpio.PB15;
                public const int MISO = Gpio.PB14;
                public const int SCK = Gpio.PB13;
            }
        }

        /// <summary>
        ///  this clcass is represented I2C Device on Nucleo F411
        /// </summary>
        public class I2CDevice
        {
            struct I2C1
            {
                public const string Name = "I2C1";
                public const int SCL = 24;
                public const int SDA = 25;
            }
        }

    }
}
