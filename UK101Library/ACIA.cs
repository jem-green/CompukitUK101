using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Windows.Threading;

/*
 * Adress
 * 
 * 0xF000 - Seems to be Status or Command
 * 0xF001 - Seems to be Data Tx or Rx 
 * 
 * A0 = RS
 * A0 = 0 Read = Read status,  Write = Write command.
 * A0 = 1 Read = Read received data, Write = Send data
 *
 *                      6850
 *                +------\/------+
 *             1 -|Vss       !CTS|- 24
 *             2 -|RxD       !DCD|- 23
 *             3 -|RxClk       D0|- 22
 *             4 -|TxClk       D1|- 21
 *             5 -|!RTS        D2|- 20
 *             6 -|TxD         D3|- 19
 *             7 -|!IRQ        D4|- 18
 *             8 -|CS0         D5|- 17
 *             9 -|!CS2        D6|- 16
 *            10 -|CS1         D7|- 15
 *            11 -|RS           E|- 14
 *            12 -|Vcc        R/W|- 13
 *                +--------------+ 
 *
 * address 0 read
 * --------------------------------
 * 
 * Data bits DB0 - DB07
 * 
 * address 0 write
 * --------------------------------
 * 
 * Data bits DB0 - DB07
 *
 * address 1 write
 * ---------------
 * 
 * DB0 = divide sel 0      00 = /1	 10 = /64
 * DB1 = divide sel 1	  01 = /16	 11 = master reset
 * DB2 = word sel 0		 000 = 7e2	011 = 7o1		110 = 8e1
 * DB3 = word sel 1		 001 = 7o2	100 = 8n2		111 = 8o1
 * DB4 = word sel 2		 010 = 7e1	101 = 8n1
 * DB5 = transmit ctrl 0 00 = -RTS low xmit irq disable
 * DB6 = transmit ctrl 1 01 = -RTS low xmit irq enable
 *                       10 = -RTS high xmit irq disable
 *                       11 = -RTS low, xmit a break lvl, irq disable
 * DB7 = receive irq enable
 *
 * address 1 read
 * --------------
 * 
 * DB0 = RX buff full
 * DB1 = TX buff empty
 * DB2 = -DCD			(1 when DCD not present)
 * DB3 = -CTS			(1 when transmit is inhibited)
 * DB4 = Framing error	(1 when error, lack of start/stop bits or break condition)
 * DB5 = RX overrun
 * DB6 = Parity Error    (1 when # of 1 bits != selected parity)
 * DB7 = IRQ             (1 when IRQ present)  cleared by read of rx or write to tx
 *
 * rxIRQ is set when Receive Data Register is full, overrun, or there is a low to high 
 * transition on the DCD signal line.
 *
 * txIRQ is set when the Transmit Data Register is empty,
 *
 */


namespace UK101Library
{
    /// <summary>
    /// ACIA is detailed as a 6850
    /// </summary>
    public class ACIA : MemoryBusDevice
    {
        #region Fields

        // I/O modes:
        public const byte IO_MODE_6820_NONE = 0;    // No mode
        public const byte IO_MODE_6820_FILE = 1;    // Use filesystem
        public const byte IO_MODE_6820_MIDI = 2;    // Use a MIDI interface
        public const byte IO_MODE_6820_TAPE = 4;    // Use internal classes
        public const byte IO_MODE_6820_SERIAL = 8;  // Use serial interface

        // Status register flags:
        const byte ACIA_STATUS_IRQ = 0x80;          // ACIA wishes to interrupt processor
        const byte ACIA_STATUS_PE = 0x40;           // A parity error has occurred
        const byte ACIA_STATUS_OVRN = 0x20;         // Receiver overrun, character not read before next came in
        const byte ACIA_STATUS_FE = 0x10;           // A framing error has occurred
        const byte ACIA_STATUS_CTS = 0x08;          // Must be LOW for clear to send. A high also forces a low TDRE
        const byte ACIA_STATUS_DCD = 0x04;          // Is LOW when clear to send. Does not reset until CPU read status AND data.
        const byte ACIA_STATUS_TDRE = 0x02;         // Is HIGH when a transmit is finished. Goes low when writing new data to send.
        const byte ACIA_STATUS_RDRF = 0x01;         // Is HIGH when a character has been read in and is ready to fetch.

        // Control register:
        const byte ACIA_CONTROL_ENABLE_IRQ = 0x80;              // A HIGH enables receive interrupt
        const byte ACIA_CONTROL_TRANSMIT_CONTROL_MASK = 0x60;   // To mask out transmit control
        const byte ACIA_CONTROL_TC_LOW_DISABLE = 0x00;          // RTS low, disable interrupts
        const byte ACIA_CONTROL_TC_LOW_ENABLE = 0x20;           // RTS low, enable interrupts
        const byte ACIA_CONTROL_TC_HIGH_DISABLE = 0x40;         // RTS high, disable interrupts
        const byte ACIA_CONTROL_TC_LOW_DISABLE_BREAK = 0x60;    // RTS low, disable interrupts
        const byte ACIA_CONTROL_PROTOCOL_MASK = 0x1C;           // Protocol mask, Bits, parity and stop bits
        const byte ACIA_CONTROL_PM_7E2 = 0x00;                  // protocol 7 bits, even parity, 2 stop bits
        const byte ACIA_CONTROL_PM_7O2 = 0x04;                  // protocol 7 bits, odd parity, 2 stop bits
        const byte ACIA_CONTROL_PM_7E1 = 0x08;                  // protocol 7 bits, even parity, 1 stop bit
        const byte ACIA_CONTROL_PM_7O1 = 0x0C;                  // protocol 7 bits, odd parity, 1 stop bit
        const byte ACIA_CONTROL_PM_8N2 = 0x10;                  // protocol 8 bits, no parity, 2 stop bits
        const byte ACIA_CONTROL_PM_8N1 = 0x14;                  // protocol 8 bits, no parity, 1 stop bit
        const byte ACIA_CONTROL_PM_8E1 = 0x18;                  // protocol 8 bits, even parity, 1 stop bit
        const byte ACIA_CONTROL_PM_8O1 = 0x1C;                  // protocol 8 bits, odd parity, 1 stop bit
        const byte ACIA_CONTROL_DIVISION_MASK = 0x03;           // Counter division bits.
        const byte ACIA_CONTROL_DM_ONE = 0x00;                  // Division 1
        const byte ACIA_CONTROL_DM_SIXTEEN = 0x01;              // Division sixteen
        const byte ACIA_CONTROL_DM_SIXTY_FOUR = 0x02;           // Division sixty four
        const byte ACIA_CONTROL_MASTER_RESET = 0x03;            // Master reset command

        private int _counterDivider = 1;

        // Protocol 

        private int _dataBits = 7;
        private Parity _parity = Parity.Even;
        private StopBits _stopBits = StopBits.Two;

        // Modem control

        private bool _cts;
        private bool _rts;
        private bool _dcd;

        // Read write control

        private bool _irq;
        private bool _rs;
        private bool _enable;
        private bool _readWrite;

        // Internal store within the ACIA

        private byte _tdr;  // Internal transmit data register
        private byte _rdr;  // Internal receive data register

        // InData -> _tdr
        // return data <- _rdr


        //public MemoryStream inStream;
        //public MemoryStream outStream;

        //private DispatcherTimer timer;
        private Timer _timer;
        
        private byte ACIAStatus;
        private byte ACIACommand;

        private IPeripheralIO _peripheralIO;
        private byte mode;

        readonly object _lockObject = new Object();

        #endregion
        #region Constructors

        public ACIA(IPeripheralIO peripheralIO)
        {
            this._peripheralIO = peripheralIO;
            ReadOnly = false;
            ACIAStatus = 0x00;

            // 12/09/2021 JPG change the timer

            //timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 10);// 33);
            //timer.Tick += Timer_Tick;

            _timer = new Timer(Timer_Tick, null, Timeout.Infinite, 10); // Create the Timer delay starting

            // Ensure that the Receive Data Refister Full is set to indicate that there is data available.
            // This seems odd but need to see what the master reset does

            SetFlag(ACIA_STATUS_RDRF);
        }

        #endregion
        #region Properties

        public bool Enable
        {
            set
            {
                _enable = value;
            }
        }

        public bool ReadWrite
        {
            set
            {
                _readWrite = value;
            }
        }

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
                        //outStream = new MemoryStream();
                        SetFlag(ACIA_STATUS_TDRE);   // Will be set all time
                        SetFlag(ACIA_STATUS_RDRF);   // Will be set all time
                        break;
                    case IO_MODE_6820_SERIAL:
                        //outStream = new MemoryStream();
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

            // Not sure how the Address gets set it seems to be memory mapped so
            // the MPU must write to this address to indicate that the status will be
            // checked rather than the reading of data

            if ((Address & 0x0001) == 0x0001)
            {
                // Read received data:
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        {
                            _timer.Change(0, 10);           // Start the timer

                            _rdr = 0x00;
                            if (_peripheralIO.Receive != null)  // Must be recieving from the tape
                            {
                                if (_peripheralIO.Receive.Position < _peripheralIO.Receive.Length) // Nothing left to recieve
                                {
                                    Byte = (byte)_peripheralIO.Receive.ReadByte();
                                    if (Byte > -1)  // Somthing to read
                                    {
                                        _rdr = (byte)Byte;
                                        if ((Byte == 0x0d) || (Byte == 0x0a))
                                        {
                                            GC.Collect();
                                            _rdr = 0x0d;
                                        }
                                        ResetFlag(ACIA_STATUS_RDRF);    // Read data register full - Data available
                                    }
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Tape not playing");
                            }
                            return (_rdr);
                        }
                    case IO_MODE_6820_SERIAL:
                        {
                            //if (inStream != null)
                            //{
                            //    Byte = inStream.ReadByte();
                            //    if (Byte > -1)
                            //    {
                            //        return (byte)Byte;
                            //    }
                            //    else
                            //    {
                            //        inStream.Close();
                            //        inStream = null;
                            //    }
                            //}
                            return 0x00;
                        }
                    case IO_MODE_6820_FILE:
                        {
                            return 0x00;
                        }
                    default:
                        {
                            return 0xff;
                        }
                }
            }
            else
            {
                // Read status:
                //Debug.WriteLine("Status=" + Convert.ToString(ACIAStatus, 2).PadLeft(8, '0'));
                switch (mode)
                {
                    case IO_MODE_6820_TAPE:
                        {
                            // Chalenge here is dont know if playing or receiveing
                            // Suspect i need another flag from the Periperal to get the
                            // status

                            if (_peripheralIO.Transmit != null)
                            {
                                SetFlag(ACIA_STATUS_TDRE);        // Transmit Data Register Empty - Data can be written
                            }
                            else if (_peripheralIO.Receive != null)
                            {
                                if (_peripheralIO.Receive.Position < _peripheralIO.Receive.Length)
                                {
                                   SetFlag(ACIA_STATUS_RDRF);       // Recieve Data Register Full - data available
                                }
                                else
                                {
                                    ResetFlag(ACIA_STATUS_RDRF);    // Recieve Data Register Full - no data available
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Tape not recording or playing");
                            }
                            return(ACIAStatus);
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
                        {
                            if (_peripheralIO.Transmit != null) // Must be transmitting to the tape
                            {
                                ResetFlag(ACIA.ACIA_STATUS_TDRE);   // Set TDRE low to indicate that ACIA is transmitting
                                _tdr = InData;
                                _peripheralIO.Transmit.WriteByte(_tdr);
                            }
                            else
                            {
                                Debug.WriteLine("Tape not recording");
                            }
                            SetFlag(ACIA.ACIA_STATUS_TDRE);   // Set TDRE high to indicate that ACIA has finished transmitting
                            break;
                        }
                    case IO_MODE_6820_SERIAL:
                        {
                            break;
                        }
                    case IO_MODE_6820_FILE:
                        {
                            break;
                        }
                }
            }
            else
            {
                // Accept a command
                ACIACommand = InData;
                Debug.WriteLine("Command=" + Convert.ToString(InData, 2).PadLeft(8, '0'));

                // Counter divide

                if ((ACIACommand & ACIA_CONTROL_DIVISION_MASK) == ACIA_CONTROL_DM_SIXTEEN)
                {
                    _counterDivider = 16;
                    Debug.WriteLine("Counter Devide=" + _counterDivider);
                }
                else if ((ACIACommand & ACIA_CONTROL_DIVISION_MASK) == ACIA_CONTROL_DM_SIXTY_FOUR)
                {
                    _counterDivider = 64;
                    Debug.WriteLine("Counter Devide=" + _counterDivider);
                }
                else if ((ACIACommand & ACIA_CONTROL_DIVISION_MASK) == ACIA_CONTROL_MASTER_RESET)
                {
                    Debug.WriteLine("Reset the status register");
                    _rts = true;
                    ACIAStatus = 0x00;
                    ResetFlag(ACIA_STATUS_IRQ);
                }

                // Protocol

                if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_7E2)
                {
                    Debug.WriteLine("Packet format 7 data, even party, 2 stop bits");
                    _dataBits = 7;
                    _parity = Parity.Even;
                    _stopBits = StopBits.Two;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_7O2)
                {
                    Debug.WriteLine("Packet format 7 data, odd party, 2 stop bits");
                    _dataBits = 7;
                    _parity = Parity.Odd;
                    _stopBits = StopBits.Two;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_7E1)
                {
                    Debug.WriteLine("Packet format 7 data, even party, 1 stop bit");
                    _dataBits = 7;
                    _parity = Parity.Even;
                    _stopBits = StopBits.One;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_7O1)
                {
                    Debug.WriteLine("Packet format 7 data, odd party, 1 stop bit");
                    _dataBits = 7;
                    _parity = Parity.Odd;
                    _stopBits = StopBits.One;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_8N2)
                {
                    Debug.WriteLine("Packet format 8 data, no party, 2 stop bits");
                    _dataBits = 8;
                    _parity = Parity.None;
                    _stopBits = StopBits.Two;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_8N1)
                {
                    Debug.WriteLine("Packet format 8 data, no party, 1 stop bit");
                    _dataBits = 8;
                    _parity = Parity.None;
                    _stopBits = StopBits.One;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_8E1)
                {
                    Debug.WriteLine("Packet format 8 data, even party, 1 stop bit");
                    _dataBits = 8;
                    _parity = Parity.None;
                    _stopBits = StopBits.One;
                }
                else if ((ACIACommand & ACIA_CONTROL_PROTOCOL_MASK) == ACIA_CONTROL_PM_8O1)
                {
                    Debug.WriteLine("Packet format 8 data, odd party, 1 stop bit");
                    _dataBits = 8;
                    _parity = Parity.Odd;
                    _stopBits = StopBits.One;
                }

                // Reset IRQ if disabled by current command:
                if ((ACIACommand & ACIA_CONTROL_ENABLE_IRQ) == 0x00
                    || (ACIACommand & ACIA_CONTROL_TRANSMIT_CONTROL_MASK) != ACIA_CONTROL_TC_LOW_ENABLE
                    || (ACIACommand & ACIA_CONTROL_DIVISION_MASK) == ACIA_CONTROL_MASTER_RESET)
                {
                    // Reset because Rx or Tx IRQ disabled or MasterReset issued:
                    ResetFlag(ACIA_STATUS_IRQ);
                    _timer.Change(Timeout.Infinite, 10);    // Disable the timer
                }

                // Set RTS (not implemented on communications side yet).
                if ((ACIACommand & ACIA_CONTROL_TRANSMIT_CONTROL_MASK) == ACIA_CONTROL_TC_HIGH_DISABLE)
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
                        SetFlag(ACIA_STATUS_RDRF);  // Indicate that the Receive Data Register is full - Has data
                        SetFlag(ACIA_STATUS_TDRE);  // Indicat that the Transmit Data Register is empty - Data sent

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

        ///////////////////////////////////////////////////////////////////////////////////
        // Helpers:
        ///////////////////////////////////////////////////////////////////////////////////

        // Set a flag:
        private void SetFlag(byte Flag)
        {
            ACIAStatus = (byte)(ACIAStatus | Flag);
        }

        // Reset a flag:
        private void ResetFlag(byte Flag)
        {
            ACIAStatus = (byte)(ACIAStatus & ~Flag);
        }

        #endregion

    }

    //public class OneByteBuffer : IBuffer
    //{
    //    public uint Capacity { get { return capacity; } set { capacity = value; } }

    //    public uint Length { get { return capacity; } set { capacity = value; } }

    //    uint capacity;
    //}
}
