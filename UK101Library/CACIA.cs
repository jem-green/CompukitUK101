using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Windows.Threading;

/**
 * A0 = RS
 * A0 = 0 Read = Read status,  Write = Write command.
 * A0 = 1 Read = Read received data, Write = Send data
 */

namespace UK101Library
{
    /// <summary>
    /// ACIA is detailed as a 6850
    /// </summary>
    public class CACIA : CMemoryBusDevice
    {
        #region Fields

        // I/O modes:
        public const byte IO_MODE_6820_FILE = 1; // Use filesystem
        public const byte IO_MODE_6820_MIDI = 2; // Use a MIDI interface
        public const byte IO_MODE_6820_TAPE = 4; // Use internal classes
        public const byte IO_MODE_6820_SERIAL = 8; // Use serial interface

        // Status register flags:
        const byte ACIA_STATUS_IRQ = 0x80;          // ACIA wishes to interrupt processor
        const byte ACIA_STATUS_PE = 0x40;           // A parity error has occurred
        const byte ACIA_STATUS_OVRN = 0x20;         // Receiver overrun, character not read before next came in
        const byte ACIA_STATUS_FE = 0x10;           // A framing error has occurred
        const byte ACIA_STATUS_CTS = 0x08;          // Must be LOW for clear to send. A high also forces a low TDRE
        const byte ACIA_STATUS_DCD = 0x04;          // Is LOW when clear to send. Dos not reset until CPU read status AND data.
        const byte ACIA_STATUS_TDRE = 0x02;         // Is HIGH when a transmit is finished. Goes low when writing new data to send.
        public const byte ACIA_STATUS_RDRF = 0x01;  // Is HIGH when a character has been read in and is ready to fetch.

        // Control register:
        const byte ACIA_CONTROL_ENABLE_IRQ = 0x80;              // A HIGH enables receive interrupt
        const byte ACIA_CONTROL_TC_MASK = 0x60;                 // To mask out transmit control
        const byte ACIA_CONTROL_TC_LOW_DISABLE = 0x00;          // RTS low, disable interrupts
        const byte ACIA_CONTROL_TC_LOW_ENABLE = 0x20;           // RTS low, enable interrupts
        const byte ACIA_CONTROL_TC_HIGH_DISABLE = 0x40;         // RTS high, disable interrupts
        const byte ACIA_CONTROL_TC_LOW_DISABLE_BREAK = 0x60;    // RTS low, disable interrupts
        const byte ACIA_CONTROL_PROTOCOL_MASK = 0x1C;           // Protocol mask, Bits, parity and stop bits
        const byte ACIA_CONTROL_DIVISION_MASK = 0x03;           // Counter division bits.
        const byte ACIA_CONTROL_MASTER_RESET = 0x03;            // Master reset command.


        public string[] sourceCode = null;

        public MemoryStream inStream;
        public MemoryStream outStream;

        //private DispatcherTimer timer;
        private Timer _timer;
        private byte ACIAStatus;
        public String[] Lines { get; set; }
        public Int16 line { get; set; }
        private Int16 pos;
        private byte Command;
        private MainPage mainPage;
        private byte mode;

        // MIDI
        private byte[] midiBuffer;
        private byte inpointer;
        private byte outpointer;
        byte[] midiOutMessage = new byte[3];
        byte midiByteNumber = 0;

        readonly object _lockObject = new Object();

        #endregion
        #region Constructors

        public CACIA(MainPage mainPage)
        {
            this.mainPage = mainPage;
            basicProg = new BasicProg();
            Lines = null;
            ReadOnly = false;
            ACIAStatus = 0x00;
            line = 0;
            pos = 0;

            // 12/09/2021 JPG change the timer

            //timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 10);// 33);
            //timer.Tick += Timer_Tick;

            _timer = new Timer(Timer_Tick, null, Timeout.Infinite, 10); // Create the Timer delay starting

            SetFlag(ACIA_STATUS_RDRF);
            midiBuffer = new byte[256];
            inpointer = 0;
            outpointer = 0;
            FileInputStream = null;
            FileOutputStream = null;
        }

        #endregion
        #region Properties

        public BasicProg basicProg { get; set; }
        //public string[] CurrentFile { get; set; }
        public Stream FileInputStream { get; set; }
        public long FileInputStreamLength { get; set; }
        public Stream FileOutputStream { get; set; }

        #endregion
        #region Methods

        public byte Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        SetFlag(ACIA_STATUS_RDRF);
                        break;
                    case IO_MODE_6820_MIDI:
                        ResetFlag(ACIA_STATUS_RDRF); // Will set when MIDI comes in
                        SetFlag(ACIA_STATUS_TDRE);   // Will be kept low a few ms by timer after data sent
                        break;
                    case IO_MODE_6820_FILE:
                        outStream = new MemoryStream();
                        SetFlag(ACIA_STATUS_TDRE);   // Will be set all time
                        SetFlag(ACIA_STATUS_RDRF);   // Will be set all time
                        break;
                    case IO_MODE_6820_SERIAL:
                        outStream = new MemoryStream();
                        SetFlag(ACIA_STATUS_TDRE);   // Will be set all time
                        SetFlag(ACIA_STATUS_RDRF);   // Will be set all time
                        break;
                }
            }
        }

        // Processor wants to read data or status:
        public override byte Read()
        {
            byte b;
            Int32 Byte;

            if ((Address & 0x0001) == 0x0001)
            {
                // Read received data:
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        _timer.Change(0, 10);   // Start the timer
                        ResetFlag(ACIA_STATUS_RDRF);
                        if (line < Lines.Length)
                        {
                            if (pos > Lines[line].Length - 1)
                            {
                                if (Lines[line] == "RUN")
                                {
                                    // Special treatment.
                                    // People useD to put "RUN" at end of listing in order to run the app,
                                    // followed by on last line containing only one space.
                                    // The manual states procedure to use if "RUN" and pace is not included
                                    // in listing only, and the user is told to get back to normal (not load)
                                    // mode by pressing space and then enter.
                                    // However, this does not work here, so if a "RUN" line is encountered
                                    // we must tell keyboard input routine to reset the load:
                                    mainPage.CSignetic6502.MemoryBus.Keyboard.loadResetIsNeeded = true;
                                }
                                line++;
                                pos = 0;
                                GC.Collect();
                                return 0x0d;
                            }
                            else
                            {
                                b = (byte)Lines[line][pos++];
                                return b;
                            }
                        }
                        return 0x00;
                    case IO_MODE_6820_MIDI:
                        // If this is the last byte received for now, set flag to indicate 'no data available':
                        if ((byte)(outpointer + 1) == inpointer)
                        {
                            ResetFlag(ACIA_STATUS_RDRF);
                        }
                        return midiBuffer[outpointer++];
                    case IO_MODE_6820_SERIAL:
                        if (inStream != null)
                        {
                            Byte = inStream.ReadByte();
                            if (Byte > -1)
                            {
                                return (byte)Byte;
                            }
                            else
                            {
                                inStream.Close();
                                inStream = null;
                            }
                        }
                        return 0x00;
                    case IO_MODE_6820_FILE:
                        //while (CharNumber >= CurrentFile[LineNumber].Length)
                        //{
                        //    CharNumber = 0;
                        //    LineNumber++;
                        //}
                        //if (LineNumber < CurrentFile.Length)
                        //{
                        //    if (CurrentFile[LineNumber][CharNumber++] != 0x0a)
                        //    {
                        //        return (byte)CurrentFile[LineNumber][CharNumber];
                        //    }
                        //}
                        if (FileInputStream != null)
                        {
                            Byte = FileInputStream.ReadByte();
                            // BASIC uses only 0d for line feeds, remove 0a:
                            if (Byte == 10)
                            {
                                Byte = FileInputStream.ReadByte();
                            }
                            if (Byte > -1)
                            {
                                return (byte)Byte;
                            }
                            else
                            {
                                FileInputStream.Close();
                                FileInputStream = null;
                            }
                        }
                        return 0xff;
                    default:
                        return 0xff;
                }
            }
            else
            {
                // Read status:
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        if (line < Lines.Length)
                        {
                            return ACIAStatus;
                        }
                        else
                        {
                            return 0;
                        }
                    case IO_MODE_6820_MIDI:
                        if ((ACIAStatus & ACIA_STATUS_TDRE) == ACIA_STATUS_TDRE)
                        {
                            SetFlag(ACIA_STATUS_TDRE);
                            return ACIAStatus;
                        }
                        else
                        {
                            return ACIAStatus;
                        }
                    case IO_MODE_6820_FILE:
                        return ACIAStatus;
                    case IO_MODE_6820_SERIAL:
                        SetFlag(ACIA_STATUS_TDRE);
                        SetFlag(ACIA_STATUS_RDRF);
                        return ACIAStatus;
                    default:
                        return 0;
                }
            }
        }

        // Processor wants to send data or set a command
        public override void Write(byte InData)
        {
            if ((Address & 0x0001) == 0x0001)
            {
                // Send data
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        break;
                    case IO_MODE_6820_MIDI:
                        if ((InData & 0x80) == 0x80)
                        {
                            midiByteNumber = 0;
                        }
                        midiBuffer[midiByteNumber++] = InData;
                        if (midiByteNumber > 2)
                        {
                            if (midiBuffer[0] == 0x90 && midiBuffer[1] > 0)
                            {
                                //mainPage.Midi.NoteOn(0, midiBuffer[1], midiBuffer[2]);
                            }
                            midiByteNumber = 0;
                        }
                        //byte[] buffer = new byte[] { InData };
                        //IBuffer ibuffer = buffer.AsBuffer();
                        //mainPage.Midi.midiOutPort.SendBuffer(ibuffer);
                        break;
                    case IO_MODE_6820_SERIAL:
                        if (outStream != null)
                        {
                            outStream.WriteByte(InData);
                        }
                        break;
                    case IO_MODE_6820_FILE:
                        if (FileOutputStream != null)
                        {
                            FileOutputStream.WriteByte(InData);
                        }
                        break;
                }
            }
            else
            {
                // Accept a command
                Command = InData;

                // Reset IRQ if disabled by current command:
                if ((Command & ACIA_CONTROL_ENABLE_IRQ) == 0x00
                    || (Command & ACIA_CONTROL_TC_MASK) != ACIA_CONTROL_TC_LOW_ENABLE
                    || (Command & ACIA_CONTROL_DIVISION_MASK) == ACIA_CONTROL_MASTER_RESET)
                {
                    // Reset because Rx or Tx IRQ disabled or MasterReset issued:
                    ResetFlag(ACIA_STATUS_IRQ);
                    _timer.Change(Timeout.Infinite, 10);
                }

                // Set RTS (not implemented on communications side yet).
                if ((Command & ACIA_CONTROL_TC_MASK) == ACIA_CONTROL_TC_HIGH_DISABLE)
                {
                    // Set RTS:
                }
                else
                {
                    // Reset RTS:
                }
            }
        }

        #endregion
        #region Private

        // Clock for simulating speed and IRQ:
        // Todo: remove the IRQ part, processor can send commands to do that.
        private void Timer_Tick(object sender)
        {
            // Create a lock to prevent from being called multiple times simultaneously
            lock (_lockObject)
            {
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        _timer.Change(Timeout.Infinite, 10);    // Stop the timer
                        SetFlag(ACIA_STATUS_RDRF);
                        //if ((Command & ACIA_CONTROL_ENABLE_IRQ) != 0x00)
                        //{
                        //    SetFlag(ACIA_STATUS_IRQ);
                        //}
                        break;
                    case IO_MODE_6820_MIDI:
                        // MIDI in is 'clocked' by the incoming MIDI data itself.
                        _timer.Change(Timeout.Infinite, 10);
                        SetFlag(ACIA_STATUS_TDRE); // Transmit timing handled by Composer
                        break;
                    case IO_MODE_6820_FILE:
                        _timer.Change(Timeout.Infinite, 10);
                        SetFlag(ACIA_STATUS_TDRE); // Transmit timing handled by Composer
                        break;
                }
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////
        // Helpers:
        ///////////////////////////////////////////////////////////////////////////////////

        // Set a flag:
        public void SetFlag(byte Flag)
        {
            ACIAStatus = (byte)(ACIAStatus | Flag);
        }

        // Reset a flag:
        public void ResetFlag(byte Flag)
        {
            ACIAStatus = (byte)(ACIAStatus & ~Flag);
        }
    }

    //public class OneByteBuffer : IBuffer
    //{
    //    public uint Capacity { get { return capacity; } set { capacity = value; } }

    //    public uint Length { get { return capacity; } set { capacity = value; } }

    //    uint capacity;
    //}
}
