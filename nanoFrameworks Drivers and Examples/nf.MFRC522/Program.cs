using System;
using System.Diagnostics;
using System.Threading;

using Driver.MFRC522;
using nanoFramework.Hardware.Esp32;


namespace nf_MFRC522
{
    static class Program
    {
        private static MFRC522 _mfRc522;
        static void Main()
        {

            // Set pins
            //Configuration.SetPinFunction(GpioPin, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);

            Debug.WriteLine("###############################################");
            Debug.WriteLine("# SPI1");
            Debug.WriteLine("###############################################");
            Debug.WriteLine($"SPI1_CLOCK:" + Configuration.GetFunctionPin(DeviceFunction.SPI1_CLOCK));
            Debug.WriteLine($"SPI1_MISO :" + Configuration.GetFunctionPin(DeviceFunction.SPI1_MISO));
            Debug.WriteLine($"SPI1_MOSI :" + Configuration.GetFunctionPin(DeviceFunction.SPI1_MOSI));
            Debug.WriteLine("###############################################");

            _mfRc522 = new MFRC522("SPI1", 4, 5);

            Debug.WriteLine("");  
            Debug.WriteLine($"Version: 0x{_mfRc522.GetVersion():X}");
            Debug.WriteLine("");
            Debug.WriteLine("###############################################");

            byte[] bufferAtqa = new byte[2];
            byte[] defaultKey = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

            while (true)
            {
                //test
                //bufferAtqa[0] = 0x0;
                //bufferAtqa[1] = 0x0;

                if (_mfRc522.IsNewCardPresent(bufferAtqa))
                {
                   // Debug.WriteLine("Card detected...");
                   // Debug.WriteLine($"ATQA: 0x{bufferAtqa[1]:X2},0x{bufferAtqa[0]:X2}");

                    var uid = _mfRc522.PiccReadCardSerial();
                    if (uid != null)
                    {
                        DisplayUid(uid);

                        //try
                        //{
                        //    byte pageOrSector = (byte)(uid.GetPiccType() == PiccType.Mifare1K ? 16 : 4);
                        //    for (byte i = 0; i < pageOrSector; i++)
                        //    {
                        //        Debug.WriteLine($"{i}:");
                        //        var buffer = _mfRc522.GetSector(uid, i, defaultKey /*, PiccCommand.AuthenticateKeyA*/);
                        //        if (uid.GetPiccType() == PiccType.Mifare1K)
                        //        {
                        //            var c = _mfRc522.GetAccessRights(buffer);
                        //            Display1kBuffer(buffer, c);
                        //        }
                        //        else if (uid.GetPiccType() == PiccType.MifareUltralight)
                        //        {
                        //            DisplayUltralightBuffer(buffer);
                        //        }
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    Debug.WriteLine(ex.Message);
                        //}

                        _mfRc522.Halt();

                        _mfRc522.StopCrypto();

                  
                    }
                    else
                    {
                        _mfRc522.ResetItAndShoot();
                    }
                }

                Thread.Sleep(500);
            }
        }

         private static void DisplayUltralightBuffer(byte[][] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                var line = "";
                for (int j = 0; j < buffer[i].Length; j++)
                {
                    line += $"{buffer[i][j]:X2} ";
                }
               Debug.WriteLine(line);
            }
        }

        private static void Display1kBuffer(byte[][] buffer, byte[] accessRights)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                var line = "";
                for (int j = 0; j < buffer[i].Length; j++)
                {
                    line += $"{buffer[i][j]:X2} ";
                }
                line += $"[{(accessRights[0] >> i) & 0x01} {(accessRights[1] >> i) & 0x01} {(accessRights[2] >> i) & 0x01}]";
               Debug.WriteLine(line);
            }
        }

        private static void DisplayUid(Uid uid)
        {
            string msg = "Uid of card is: ";
            for (int i = 0; i < (int)uid.UidType; i++)
            {
                msg += $"{uid.UidBytes[i]:X2} ";
            }
            msg += $"SAK: {uid.Sak:X2}";
           Debug.WriteLine(msg);

            switch (uid.GetPiccType())
            {
                case PiccType.Mifare1K:
                     Debug.WriteLine("PICC type: MIFARE 1K");
                    //Debug.WriteLine("");
                    break;
                case PiccType.MifareUltralight:
                     Debug.WriteLine("PICC type: MIFARE Ultralight");
                    //Debug.WriteLine("");
                    break;
                default:
                     Debug.WriteLine("PICC type: Unknown");
                    //Debug.WriteLine("");
                    break;

                   
            }
        }

    }
}