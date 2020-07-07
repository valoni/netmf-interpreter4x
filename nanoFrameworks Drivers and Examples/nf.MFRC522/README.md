### Wiring
![alt text](https://github.com/valoni/netmf-interpreter4x/blob/master/nanoFrameworks%20Drivers%20and%20Examples/nf.MFRC522/ESP32_MFRC522.png "MFRC522")

### Fork from
https://github.com/bauland/TinyClrLib/tree/master/Modules/Others/MfRc522

### How to driver / Program.cs
```csharp
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
```

### Driver / MfRc522.cs
```csharp
using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace Driver.MFRC522
{
     /// <summary>
    /// Address of register
    /// </summary>
    public enum Register
    {
        // Dummy Register for tests
        Test = 0x2b << 1,
        // Command and status
        Command = 0x01 << 1,
        ComIrq = 0x04 << 1,
        DivIrq = 0x05 << 1,
        Error = 0x06 << 1,
        Status1 = 0x07 << 1,
        Status2 = 0x08 << 1,
        FifoData = 0x09 << 1,
        FifoLevel = 0x0A << 1,
        Control = 0x0C << 1,
        BitFraming = 0x0D << 1,
        Coll = 0x0E << 1,

        Mode = 0x11 << 1,
        TxMode = 0x12 << 1,
        RxMode = 0x13 << 1,
        TxControl = 0x14 << 1,
        TxAsk = 0x15 << 1,
        Version = 0x37 << 1,

        CrcResultHigh = 0x21 << 1,
        CrcResultLow = 0x22 << 1,
        ModeWith = 0x24 << 1,
        TimerMode = 0x2A << 1,
        TimerPrescaler = 0x2B << 1,
        TimerReloadHigh = 0x2C << 1,
        TimerReloadLow = 0x2D << 1,
    }

    /// <summary>
    /// Return code of some functions
    /// </summary>
    public enum StatusCode
    {
        Ok,
        Collision,
        Error,
        Timeout,
        NoRoom,
        CrcError
    }

    /// <summary>
    /// Command to send to picc (card)
    /// </summary>
    public enum PiccCommand
    {
        ReqA = 0x26,
        MifareRead = 0x30,
        HaltA = 0x50,
        AuthenticateKeyA = 0x60,
        AuthenticateKeyB = 0x61,
        SelCl1 = 0x93,
        SelCl2 = 0x95,
        SelCl3 = 0x97,
    }

    /// <summary>
    /// Command of reader
    /// </summary>
    public enum PcdCommand
    {
        Idle = 0x00,
        CalculateCrc = 0x03,
        Transceive = 0x0c,
        MfAuthenticate = 0xe,
    }

    /// <summary>
    /// Length of uid
    /// </summary>
    public enum UidType
    {
        T4 = 4,
        T7 = 7,
        T10 = 10
    }

    /// <summary>
    /// Type of card
    /// </summary>
    public enum PiccType
    {
        Unknown,
        Mifare1K,
        MifareUltralight
    }

    /// <summary>
    /// 
    /// </summary>
    public class Uid
    {
        /// <summary>
        /// Lentgh of uid (can be 4, 7 or 10 bytes length)
        /// </summary>
        public UidType UidType { get; set; }

        /// <summary>
        /// Contain uid of card (can be 4, 7 or 10 bytes length)
        /// </summary>
        public byte[] UidBytes { get; set; }

        /// <summary>
        /// Sak which contains usefull informations
        /// </summary>
        public byte Sak { get; set; }

        /// <summary>
        /// Get type of card
        /// </summary>
        /// <returns>Type of card</returns>
        public PiccType GetPiccType()
        {
            var sak = Sak & 0x7f;
            switch (sak)
            {
                case 0x08:
                    return PiccType.Mifare1K;
                case 0x00:
                    return PiccType.MifareUltralight;
                default:
                    return PiccType.Unknown;
            }
        }
    }

    /// <summary>
    /// MfRc522 module
    /// </summary>
    public class MFRC522
    {
        private readonly GpioPin _resetPin;
        //private readonly GpioPin _irqPin;
        private readonly SpiDevice _spi;
        private readonly byte[] _registerWriteBuffer;
        private readonly byte[] _dummyBuffer2;

        /// <summary>
        /// Constructor of MfRc522 module
        /// </summary>
        /// <param name="spiBus">Spi bus</param>
        /// <param name="resetPin">Reset Pin (RST)</param>
        /// <param name="csPin">ChipSelect Pin(SDA)</param>
        public MFRC522(string spiBus, int resetPin, int csPin)
        {
            _dummyBuffer2 = new byte[2];
            _registerWriteBuffer = new byte[2];

            var gpioCtl = GpioController.GetDefault();

            _resetPin = gpioCtl.OpenPin(resetPin);
            _resetPin.SetDriveMode(GpioPinDriveMode.Output);
            _resetPin.Write(GpioPinValue.High);

            //if (irqPin != -1)
            //{
            //    _irqPin = gpioCtl.OpenPin(irqPin);
            //    _irqPin.SetDriveMode(GpioPinDriveMode.Input);
            //    _irqPin.ValueChanged += _irqPin_ValueChanged;
            //}

            var settings = new SpiConnectionSettings(csPin)
            {
                // If necessary
                //BitOrder = DataBitOrder.LSB,
                //ChipSelectActiveState = false,
                ChipSelectLine = csPin,
                //ChipSelectType = SpiChipSelectType.Gpio,
                ClockFrequency = 7_000_000, // Was 10_000_000 droppped due instabiities and bit rotate issues
                DataBitLength = 8,
                Mode = SpiMode.Mode0,
            };
            
            _spi = SpiDevice.FromId(spiBus, settings);
            
            HardReset();
            SetDefaultValues();
        }

        public void ResetItAndShoot()
        {
            HardReset();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            // Set Timer for Timeout
            WriteRegister(Register.TimerMode, 0x80);
            WriteRegister(Register.TimerPrescaler, 0xA9);
            WriteRegister(Register.TimerReloadHigh, 0x06);
            WriteRegister(Register.TimerReloadLow, 0xE8);

            // Force 100% Modulation
            WriteRegister(Register.TxAsk, 0x40);

            // Set CRC to 0x6363 (iso 14443-3 6.1.6)
            WriteRegister(Register.Mode, 0x3D);

            EnableAntennaOn();
        }

        private void EnableAntennaOn()
        {
            SetRegisterBit(Register.TxControl, 0x03);
        }

        public void Test()
        {
            for (int i = 0; i < 50; i++)
            {
                WriteRegister(Register.FifoData, 0xAA);
                var txOn = ReadRegister(Register.FifoData);
                Debug.WriteLine("Transmit On Reg. = " + txOn);
                Thread.Sleep(100);

                WriteRegister(Register.FifoData, 0x55);
                var txOff = ReadRegister(Register.FifoData);
                Debug.WriteLine("Transmit Off Reg. = " + txOff);
                Thread.Sleep(100);

                var xmit = ReadRegister(Register.TxControl);
                Debug.WriteLine("Transmit Register = " + xmit);
                Thread.Sleep(100);

                Debug.WriteLine("");

            }
        }

        /// <summary>
        /// Check if a new card is present
        /// </summary>
        /// <param name="bufferAtqa"> return a buffer of 2 bytes with ATQA answer</param>
        /// <returns>true if there is a new card, else false</returns>
        public bool IsNewCardPresent(byte[] bufferAtqa)
        {
            if (bufferAtqa == null || bufferAtqa.Length != 2) throw new ArgumentException("bufferAtqa must be initialized and its size must be 2.", nameof(bufferAtqa));
            StatusCode sc = PiccRequestA(bufferAtqa);
            if (sc == StatusCode.Collision || sc == StatusCode.Ok) return true;
            return false;
        }

        /// <summary>
        /// Get serial of card
        /// </summary>
        /// <returns>Get Uid of card (which contains type)</returns>
        public Uid PiccReadCardSerial()
        {
            StatusCode sc = PiccSelect(out Uid uid);
            if (sc == StatusCode.Ok)
                return uid;
            return null;
        }

        private StatusCode PiccSelect(out Uid uid)
        {
            uid = new Uid();
            bool selectDone = false;
            int bitKnown = 0;
            var uidKnown = new byte[4];
            var tempUid = new byte[10];
            ClearRegisterBit(Register.Coll, 0x80);
            int selectCascadeLevel = 1;
            while (!selectDone)
            {
                var bufferLength = bitKnown == 0 ? 2 : 9;
                var buffer = new byte[bufferLength];
                var bufferBack = bitKnown == 0 ? new byte[5] : new byte[3];
                byte nvb = (byte)(bitKnown == 0 ? 0x20 : 0x70);
                int destinationIndex;
                switch (selectCascadeLevel)
                {
                    case 1:
                        buffer[0] = (byte)PiccCommand.SelCl1;
                        uid.UidType = UidType.T4;
                        destinationIndex = 0;
                        break;
                    case 2:
                        buffer[0] = (byte)PiccCommand.SelCl2;
                        uid.UidType = UidType.T7;
                        destinationIndex = 3;
                        break;
                    case 3:
                        buffer[0] = (byte)PiccCommand.SelCl3;
                        uid.UidType = UidType.T10;
                        destinationIndex = 6;
                        break;
                    default:
                        return StatusCode.Error;
                }
                //buffer[0] = (byte)PiccCommand.SelCl1;
                buffer[1] = nvb;
                if (bitKnown != 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        buffer[i + 2] = uidKnown[i];
                    }

                    buffer[6] = (byte)(buffer[2] ^ buffer[3] ^ buffer[4] ^ buffer[5]);
                    var crcStatus = CalculateCrc(buffer, 7, buffer, 7);
                    if (crcStatus != StatusCode.Ok) return crcStatus;
                }

                DisplayBuffer(buffer);
                byte validbits = 0;
                WriteRegister(Register.BitFraming, 0);

                StatusCode sc = TransceiveData(buffer, bufferBack, ref validbits);
                if (sc != StatusCode.Ok)
                {
                    return sc;
                }

                if (sc == StatusCode.Ok)
                {
                    if (bitKnown >= 32)
                    // We know all bits
                    {

                        if (buffer[2] == 0x88) // Cascade Tag
                        {
                            // check CascadeTag with SAK
                            var check = bufferBack[0] & 0x04;
                            if (check != 0x04) return StatusCode.Error;
                            // backup uid for CascadeLevel
                            Array.Copy(buffer, 3, tempUid, destinationIndex, 3);
                            selectCascadeLevel++;

                            // Clear bit know and redo REQ with next CascadeLevel
                            bitKnown = 0;
                        }
                        else
                        {
                            selectDone = true;
                            Array.Copy(buffer, 2, tempUid, destinationIndex, 4);
                            uid.Sak = bufferBack[0];
                            var check = bufferBack[0] & 0x04;
                            if (check == 0x04) return StatusCode.Error;
                        }
                    }
                    else
                    {
                        // All bit are known, redo loop to do SELECT
                        bitKnown = 32;
                        // Save
                        for (int i = 0; i < 4; i++) // 5 is BCC
                        {
                            uidKnown[i] = bufferBack[i];
                        }
                    }
                }
            }
            // Create Uid with collected infos
            uid.UidBytes = new byte[(int)uid.UidType];
            Array.Copy(tempUid, uid.UidBytes, uid.UidBytes.Length);
            return StatusCode.Ok;
        }

       [ConditionalAttribute("MYDEBUG")]
        private void DisplayBuffer(byte[] buffer)
        {
           Debug.WriteLine("#Data:");
           Debug.WriteLine($"Length: {buffer.Length}");
            var str = "";
            for (int i = 0; i < buffer.Length; i++)
                str += $"{buffer[i]:X2} ";
           Debug.WriteLine($"{str}");
        }

        private StatusCode PiccRequestA(byte[] bufferAtqa)
        {
            return ShortFrame(PiccCommand.ReqA, bufferAtqa);
        }

        private StatusCode ShortFrame(PiccCommand cmd, byte[] bufferAtqa)
        {
            ClearRegisterBit(Register.Coll, 0x80);
            byte validBits = 7;
            StatusCode sc = TransceiveData(new[] { (byte)cmd }, bufferAtqa, ref validBits);
            if (sc != StatusCode.Ok) return sc;
            if (validBits != 0)
                return StatusCode.Error;
            return StatusCode.Ok;
        }

        private StatusCode TransceiveData(byte[] buffer, byte[] bufferBack, ref byte validBits)
        {
            byte waitIrq = 0x30;
            return CommunicateWithPicc(PcdCommand.Transceive, waitIrq, buffer, bufferBack, ref validBits);
        }

        private StatusCode CommunicateWithPicc(PcdCommand cmd, byte waitIrq, byte[] sendData, byte[] backData, ref byte validBits/*, byte rxAlign = 0, bool crcCheck = false*/)
        {

            byte txLastBits = validBits;
            byte bitFraming = txLastBits;

            WriteRegister(Register.Command, (byte)PcdCommand.Idle);
            WriteRegister(Register.ComIrq, 0x7f);
            WriteRegister(Register.FifoLevel, 0x80);
            WriteRegister(Register.FifoData, sendData);
            WriteRegister(Register.BitFraming, bitFraming);
            WriteRegister(Register.Command, (byte)cmd);

            if (cmd == PcdCommand.Transceive)
            {
                SetRegisterBit(Register.BitFraming, 0x80);
            }

            StatusCode sc = WaitForCommandComplete(waitIrq);
            if (sc == StatusCode.Timeout)
            {
                return sc;
            }

            // Stop if BufferOverflow, Parity or Protocol error
            byte error = ReadRegister(Register.Error);
            if ((byte)(error & 0x13) != 0x00) return StatusCode.Error;

            // Get data back from Mfrc522
            if (backData != null)
            {
                byte n = ReadRegister(Register.FifoLevel);
                if (n > backData.Length) return StatusCode.NoRoom;
                // if (n < backData.Length) return StatusCode.Error;
                ReadRegister(Register.FifoData, backData);

                DisplayBuffer(backData);

                validBits = (byte)(ReadRegister(Register.Control) & 0x07);
            }

            // Check collision
            if ((byte)(error & 0x08) == 0x08) return StatusCode.Collision;

            return StatusCode.Ok;
        }

        /// <summary>
        /// Read a page or a sector of a card
        /// </summary>
        /// <param name="uid"> uid of card to read</param>
        /// <param name="pageOrSector"> number of page or sector to read</param>
        /// <param name="key">key to authenticate (not used with Ultralight)</param>
        /// <param name="authenticateType">type of authentication (not used with Ultralight): A (default) or B</param>
        /// <returns></returns>
        public byte[][] GetSector(Uid uid, byte pageOrSector, byte[] key, PiccCommand authenticateType = PiccCommand.AuthenticateKeyA)
        {
            if (key == null || key.Length != 6) throw new ArgumentException("Key must be a byte[] of length 6.", nameof(key));
            switch (uid.GetPiccType())
            {
                case PiccType.Mifare1K:
                    return GetMifare1KSector(uid, pageOrSector, key, authenticateType);
                case PiccType.MifareUltralight:
                    return GetMifareUltraLight(pageOrSector);
                default:
                    return null;
            }
        }

        private byte[][] GetMifareUltraLight(byte page)
        {

            byte[] buffer = new byte[18];
            byte[][] resultBuffer = new byte[4][];
            for (int i = 0; i < 4; i++)
                resultBuffer[i] = new byte[4];
            var sc = MifareRead((byte)(page * 4), buffer);
            if (sc != StatusCode.Ok) throw new Exception($"MifareRead() failed:{sc}");
            for (int j = 0; j < 4; j++)
                Array.Copy(buffer, j * 4, resultBuffer[j], 0, 4);
            return resultBuffer;
        }

        private byte[][] GetMifare1KSector(Uid uid, byte sector, byte[] key, PiccCommand cmd = PiccCommand.AuthenticateKeyA)
        {
            if (sector > 15) throw new ArgumentOutOfRangeException(nameof(sector), "Sector must be between 0 and 16.");
            byte numberOfBlocks = 4;
            var firstblock = sector * numberOfBlocks;
            var isTrailerBlock = true;
            byte[] buffer = new byte[18];
            byte[][] returnBuffer = new byte[4][];
            for (int i = 0; i < 4; i++)
            {
                returnBuffer[i] = new byte[16];
            }

            for (int i = numberOfBlocks - 1; i >= 0; i--)
            {
                var blockAddr = (byte)(firstblock + i);
                StatusCode sc;
                if (isTrailerBlock)
                {
                    sc = Authenticate(uid, key, blockAddr, cmd);
                   // if (sc != StatusCode.Ok) throw new Exception($"Authenticate() failed:{sc}");
                }
                // Read block
                sc = MifareRead(blockAddr, buffer);
               // if (sc != StatusCode.Ok) throw new Exception($"MifareRead() failed:{sc}");
                if (isTrailerBlock)
                {
                    isTrailerBlock = false;
                }
                Array.Copy(buffer, returnBuffer[i], 16);
            }
            return returnBuffer;
        }

        private StatusCode MifareRead(byte blockAddr, byte[] buffer)
        {
            byte[] cmdBuffer = new byte[4];
            if (buffer == null || buffer.Length != 18) return StatusCode.NoRoom;
            cmdBuffer[0] = (byte)PiccCommand.MifareRead;
            cmdBuffer[1] = blockAddr;
            var sc = CalculateCrc(cmdBuffer, 2, cmdBuffer, 2);
            if (sc != StatusCode.Ok) return sc;
            byte validBits = 0;

            sc = TransceiveData(cmdBuffer, buffer, ref validBits);
            if (sc != StatusCode.Ok) return sc;

            // Check CRC
            byte[] crc = new byte[2];
            sc = CalculateCrc(buffer, 16, crc, 0);
            if (sc != StatusCode.Ok) return sc;
            if (buffer[16] == crc[0] && buffer[17] == crc[1]) return StatusCode.Ok;
            return StatusCode.CrcError;
        }

        /// <summary>
        /// Get access bytes from sector of 1k card
        /// </summary>
        /// <param name="sector">byte array contains sector informations (must be a 4 * 16 bytes array)</param>
        /// <returns></returns>
        public byte[] GetAccessRights(byte[][] sector)
        {
            byte[] c = new byte[3];
            if (sector.Length != 4) throw new ArgumentOutOfRangeException(nameof(sector), "Must content 4 blocks.");
            c[0] = (byte)(sector[3][7] >> 4);
            c[1] = (byte)(sector[3][8] & 0x0f);
            c[2] = (byte)(sector[3][8] >> 4);

            return c;
        }

        /// <summary>
        /// Get version of reader
        /// </summary>
        /// <returns>Number of version</returns>
        public byte GetVersion()
        {
            return ReadRegister(Register.Version);
        }

        /// <summary>
        /// Stop to communicate with a card
        /// </summary>
        /// <returns>Status code</returns>
        public StatusCode Halt()
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)PiccCommand.HaltA;
            buffer[1] = 0;
            byte validBits = 0;
            StatusCode sc = CalculateCrc(buffer, 2, buffer, 2);
            if (sc != StatusCode.Ok) return sc;
            sc = TransceiveData(buffer, null, ref validBits);
            if (sc == StatusCode.Timeout) return StatusCode.Ok;
            if (sc == StatusCode.Ok) return StatusCode.Error;
            return sc;
        }

        /// <summary>
        /// Stop to use crypto1 with communication. Must be called after Halt() when sector have been authenticate.
        /// </summary>
        public void StopCrypto()
        {
            ClearRegisterBit(Register.Status2, 0x08);
        }

        /// <summary>
        /// Authenticate to access sector
        /// </summary>
        /// <param name="uid">Uid of the card to access</param>
        /// <param name="key">Key to access (must be a array of 6 bytes)</param>
        /// <param name="blockAddress">Address of block (not sector !)</param>
        /// <param name="cmd">Type of authentication (Must be AuthenticateKeyA or AuthenticateKeyB)</param>
        /// <returns></returns>
        public StatusCode Authenticate(Uid uid, byte[] key, byte blockAddress, PiccCommand cmd)
        {
            if (cmd != PiccCommand.AuthenticateKeyA && cmd != PiccCommand.AuthenticateKeyB)
                throw new ArgumentException("Must be AuthenticateA or AuthenticateB only");

            if (key.Length != 6) throw new ArgumentException("Key must have a length of 6.", nameof(key));
            byte waitIrq = 0x10;
            byte validBits = 0;
            byte[] buffer = new byte[12];
            buffer[0] = (byte)cmd;
            buffer[1] = blockAddress;
            // set key
            for (int i = 0; i < 6; i++)
            {
                buffer[i + 2] = key[i];
            }
            // set uid
            for (int i = 0; i < 4; i++)
            {
                buffer[i + 8] = uid.UidBytes[uid.UidType == UidType.T4 ? i : i + 3];
            }

            return CommunicateWithPicc(PcdCommand.MfAuthenticate, waitIrq, buffer, null, ref validBits);
        }

        /// <summary>
        /// Calculate Crc of a buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="lengthBuffer"></param>
        /// <param name="bufferBack"></param>
        /// <param name="indexBufferBack"></param>
        /// <returns></returns>
        private StatusCode CalculateCrc(byte[] buffer, int lengthBuffer, byte[] bufferBack, int indexBufferBack)
        {
            byte[] shortBuffer = new byte[lengthBuffer];
            Array.Copy(buffer, shortBuffer, lengthBuffer);
            WriteRegister(Register.Command, (byte)PcdCommand.Idle);
            WriteRegister(Register.DivIrq, 0x04);
            WriteRegister(Register.FifoLevel, 0x80);
            WriteRegister(Register.FifoData, shortBuffer);
            WriteRegister(Register.Command, (byte)PcdCommand.CalculateCrc);
            for (int i = 500; i > 0; i--)
            {
                byte n = ReadRegister(Register.DivIrq);
                if ((n & 0x04) == 0x04)
                {
                    WriteRegister(Register.Command, (byte)PcdCommand.Idle);
                    bufferBack[indexBufferBack] = ReadRegister(Register.CrcResultLow);
                    bufferBack[indexBufferBack + 1] = ReadRegister(Register.CrcResultHigh);
                    return StatusCode.Ok;
                }
            }

            return StatusCode.Timeout;
        }

        private StatusCode WaitForCommandComplete(byte waitIrq)
        {
            for (int i = 200; i > 0; i--)
            {
                byte n = ReadRegister(Register.ComIrq);

                if ((n & waitIrq) != 0)
                    return StatusCode.Ok;

                if ((n & 0x01) == 0x01)
                {
                    return StatusCode.Timeout;
                }
            }

            return StatusCode.Timeout;
        }

        private void HardReset()
        {
            _resetPin.Write(GpioPinValue.Low);
            Thread.Sleep(20);
            _resetPin.Write(GpioPinValue.High);
            Thread.Sleep(20);
        }

        #region Basic Communication functions

        private void WriteRegister(Register register, byte data)
        {
            _registerWriteBuffer[0] = (byte)register;
            _registerWriteBuffer[1] = data;
            _spi.TransferFullDuplex(_registerWriteBuffer, _dummyBuffer2);
        }

        private void WriteRegister(Register register, byte[] data)
        {
            foreach (var b in data)
            {
                WriteRegister(register, b);
            }
        }

        private byte ReadRegister(Register register)
        {
            _registerWriteBuffer[0] = (byte)((byte)register | 0x80);
            _registerWriteBuffer[1] = 0x00;
            _spi.TransferFullDuplex(_registerWriteBuffer, _dummyBuffer2);
            Byte _tempByte = _dummyBuffer2[1];
           //var  tempByte = (int) _tempByte << 4;
            return _dummyBuffer2[1];
        }

        private void ReadRegister(Register register, byte[] backData)
        {
            if (backData == null || backData.Length == 0) return;
            byte address = (byte)((byte)register | 0x80);
            byte[] writeBuffer = new byte[backData.Length + 1];
            byte[] readBuffer = new byte[backData.Length + 1];

            for (int i = 0; i < backData.Length; i++) writeBuffer[i] = address;
            _spi.TransferFullDuplex(writeBuffer, readBuffer);
            Array.Copy(readBuffer, 1, backData, 0, backData.Length);
        }

        private void SetRegisterBit(Register register, byte mask)
        {
            var tmp = ReadRegister(register);
            WriteRegister(register, (byte)(tmp | mask));
        }

        private void ClearRegisterBit(Register register, byte mask)
        {
            var tmp = ReadRegister(register);
            WriteRegister(register, (byte)(tmp & ~mask));
        }
        #endregion

    }
}

```
