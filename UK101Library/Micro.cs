using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UK101Library
{
    public class Micro : IMicro, IDisposable
    {
        #region Fields

        private readonly IPeripheralIO _peripheralIO;
        private Signetic6502 _signetic6502;
        private Clock _clock;
        private AddressBus _addressBus;
        private DataBus _dataBus;
        private bool disposedValue;
        private const Int32 DEVICES_MAX = 12;
        private MemoryBusDevice[] _devices = new MemoryBusDevice[DEVICES_MAX];
        private ushort _address = 0;
        private int _lines = 32;

        #endregion
        #region Constructor
        public Micro(IPeripheralIO peripheralIO)
        {
            _peripheralIO = peripheralIO;
        }
        #endregion
        #region Properties

        public AddressBus AddressBus
        {
            set
            {
                _addressBus = value;
            }
            get
            {
                return (_addressBus);
            }
        }

        public MemoryBusDevice[] Devices
        {
            set
            {
                _devices = value;
            }
            get
            {
                return (_devices);
            }
        }

        public int Height
        {
            set
            {
                _lines = value;
            }
            get
            {
                return (_lines);
            }
        }

        /// <summary>
        /// Get the device by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MemoryBusDevice this[byte index]
        {
            get
            {
                return(_devices[index]);
            }
        }

        /// <summary>
        /// Get the device by Name
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MemoryBusDevice this[string name]
        {
            get
            {
                MemoryBusDevice device = null;
                for (int i = 0; i < _devices.Length; i++)
                {
                    if (_devices[i].Name == name)
                    {
                        device = _devices[i];
                        break;
                    }
                }
                return (device);
            }
        }

        #endregion
        #region Methods

        public void Init(int lines)
        {
            Debug.WriteLine("In Init()");

            byte deviceCount = 0;

            CEGMON Monitor = new CEGMON(_address);               // Monitor
            Monitor.StartsAt = 0xF800;
            Monitor.EndsAt = 0xFFFF;
            _devices[0] = Monitor;
            //_devices[deviceCount++] = Monitor;

            ACIA ACIA = new ACIA(_peripheralIO);                // ACAI
            ACIA.StartsAt = 0xF000;
            ACIA.EndsAt = 0xF0FF;
            _devices[1] = ACIA;
            //_devices[deviceCount++] = ACIA;

            Keyboard keyboard = new Keyboard(_peripheralIO);    // Keyboard
            keyboard.StartsAt = 0xDF00;
            keyboard.EndsAt = 0xDF00;
            _devices[2] = keyboard;
            //_devices[deviceCount++] = keyboard;

            VDU VDU = new VDU(_peripheralIO);                   // Video
            VDU.StartsAt = 0xD000;
            VDU.EndsAt = 0xD7FF;
            _devices[3] = VDU;
            //_devices[deviceCount++] = VDU;

            BASIC4 Basic4 = new BASIC4(_address);               // Basic 4
            Basic4.StartsAt = 0xB800;
            Basic4.EndsAt = 0xBFFF;
            _devices[4] = Basic4;
            //_devices[deviceCount++] = Basic4;

            BASIC3 Basic3 = new BASIC3(_address);               // Basic 3
            Basic3.StartsAt = 0xB000;
            Basic3.EndsAt = 0xB7FF;
            _devices[5] = Basic3;
            //_devices[deviceCount++] = Basic3;

            BASIC2 Basic2 = new BASIC2(_address);               // Basic 2
            Basic2.StartsAt = 0xA800;
            Basic2.EndsAt = 0xAFFF;
            _devices[6] = Basic2;
            //_devices[deviceCount++] = Basic2;

            BASIC1 Basic1 = new BASIC1(_address);               // Basic 1
            Basic1.StartsAt = 0xA000;
            Basic1.EndsAt = 0xA7FF;
            _devices[7] = Basic1;
            //_devices[deviceCount++] = Basic1;

            ROM8000 ROM8000 = new ROM8000(_address);            // ROM8000
            ROM8000.StartsAt = 0x8000;
            ROM8000.EndsAt = 0x8FFF;
            _devices[8] = ROM8000;
            //_devices[deviceCount++] = ROM8000;

            NoDevice noDevice = new NoDevice();                 // Was MIDI
            _devices[9] = noDevice;
            //_devices[deviceCount++] = noDevice;

            RAM RAM = new RAM();                                // RAM 8K = 0x2000
            RAM.StartsAt= 0x0000;
            RAM.EndsAt = 0x1FFF;
            RAM.SetSize(0x2000);
            _devices[10] = RAM;
            //_devices[deviceCount++] = RAM;

            _devices[11] = noDevice;                            // Not sure what this is for
            //_devices[deviceCount++] = noDevice;

            _dataBus = new DataBus(_devices);
            _addressBus = new AddressBus(_devices, _dataBus);
            _addressBus.Init();

            _signetic6502 = new Signetic6502(_addressBus,_dataBus);
            _clock = new Clock(_signetic6502);

            VDU.Init();
			
			_lines = lines;
            if (_lines == 16)
            {
                Monitor.Data[0x3bc] = 0x2f;
                Monitor.Data[0x3bd] = 0x4c;
                Monitor.Data[0x3be] = 0xd0;
                Monitor.Data[0x3bf] = 0x8c;
                Monitor.Data[0x3c0] = 0xd3;
                Monitor.Data[0x0222] = 0x47;
                RAM.Data[0x0223] = 0x0c;
                RAM.Data[0x0224] = 0xd0;
                RAM.Data[0x0225] = 0xcc;
                RAM.Data[0x0226] = 0xd1;
            }
            else
            {
                Monitor.Data[0x3bc] = 0x2f;
                Monitor.Data[0x3bd] = 0x4c;
                Monitor.Data[0x3be] = 0xd0;
                Monitor.Data[0x3bf] = 0x8c;
                Monitor.Data[0x3c0] = 0xd7;
                RAM.Data[0x0222] = 0x47;
                RAM.Data[0x0223] = 0x0c;
                RAM.Data[0x0224] = 0xd0;
                RAM.Data[0x0226] = 0xd3;
            }

            Debug.WriteLine("Out Init()");
        }

        public void SetLines(int lines)
        {
            // I think this is CEGMON specific

            CEGMON monitor = (CEGMON)this["CEGMON"];
            RAM RAM = (RAM)this["RAM"];

            _lines = lines;
            if (_lines == 16)
            {
                monitor.Data[0x3bc] = 0x2f;
                monitor.Data[0x3bd] = 0x4c;
                monitor.Data[0x3be] = 0xd0;
                monitor.Data[0x3bf] = 0x8c;
                monitor.Data[0x3c0] = 0xd3;
                RAM.Data[0x0222] = 0x47;
                RAM.Data[0x0223] = 0x0c;
                RAM.Data[0x0224] = 0xd0;
                RAM.Data[0x0225] = 0xcc;
                RAM.Data[0x0226] = 0xd1;
            }
            else
            {
                monitor.Data[0x3bc] = 0x2f;
                monitor.Data[0x3bd] = 0x4c;
                monitor.Data[0x3be] = 0xd0;
                monitor.Data[0x3bf] = 0x8c;
                monitor.Data[0x3c0] = 0xd7;
                RAM.Data[0x0222] = 0x47;
                RAM.Data[0x0223] = 0x0c;
                RAM.Data[0x0224] = 0xd0;
                RAM.Data[0x0226] = 0xd3;
            }
        }

        public void Run()
        {
            Debug.WriteLine("In Run()");

            _signetic6502.Reset();
            _clock.Hold = false;
            _clock.Start();

            Debug.WriteLine("Out Run()");
        }

        public void Reset()
        {
            Debug.WriteLine("In Reset()");

            _signetic6502.Reset();

            Debug.WriteLine("Out Reset()");
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
        #region Private

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Micro()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        #endregion
    }
}
