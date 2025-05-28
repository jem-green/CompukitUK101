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

        // Devices

        private CEGMON _monitor;        // CEGMON
        private ACIA _acia;             // ACAI
        private Keyboard _keyboard;     // Keyboard
        private VDU _vdu;               // Video
        private BASIC4 _basic4;         // Basic 4
        private BASIC3 _basic3;         // Basic 3
        private BASIC2 _basic2;         // Basic 2
        private BASIC1 _basic1;         // Basic 1
        private ROM8000 _rom8000;       // ROM8000
        private NoDevice _noDevice;     // Place holder
        private RAM _ram;               // RAM 8K = 0x2000

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

        // The idea would be to support alternative
        // devices and configure them from the uk101.xml

        public void Init(int lines)
        {
            Debug.WriteLine("In Init()");

            byte deviceCount = 0;

            _monitor = new CEGMON(_address);                 // Monitor
            _monitor.StartsAt = 0xF800;
            _monitor.EndsAt = 0xFFFF;
            //_devices[0] = _monitor;
            _devices[deviceCount++] = _monitor;

            _acia = new ACIA(_peripheralIO);                 // ACAI
            _acia.StartsAt = 0xF000;
            _acia.EndsAt = 0xF0FF;
            //_devices[1] = _acia;
            _devices[deviceCount++] = _acia;

            _keyboard = new Keyboard(_peripheralIO);         // Keyboard
            _keyboard.StartsAt = 0xDF00;
            _keyboard.EndsAt = 0xDF00;
            //_devices[2] = _keyboard;
            _devices[deviceCount++] = _keyboard;

            _vdu = new VDU(_peripheralIO);                   // Video
            _vdu.StartsAt = 0xD000;
            _vdu.EndsAt = 0xD7FF;
            //_devices[3] = _vdu;
            _devices[deviceCount++] = _vdu;

            _basic4 = new BASIC4(_address);                  // Basic 4
            _basic4.StartsAt = 0xB800;
            _basic4.EndsAt = 0xBFFF;
            //_devices[4] = _basic4;
            _devices[deviceCount++] = _basic4;

            _basic3 = new BASIC3(_address);                  // Basic 3
            _basic3.StartsAt = 0xB000;
            _basic3.EndsAt = 0xB7FF;
            //_devices[5] = _basic3;
            _devices[deviceCount++] = _basic3;

            _basic2 = new BASIC2(_address);                  // Basic 2
            _basic2.StartsAt = 0xA800;
            _basic2.EndsAt = 0xAFFF;
            //_devices[6] = _basic2;
            _devices[deviceCount++] = _basic2;

            _basic1 = new BASIC1(_address);                  // Basic 1
            _basic1.StartsAt = 0xA000;
            _basic1.EndsAt = 0xA7FF;
            //_devices[7] = _basic1;
            _devices[deviceCount++] = _basic1;

            _rom8000 = new ROM8000(_address);                // ROM8000
            _rom8000.StartsAt = 0x8000;
            _rom8000.EndsAt = 0x8FFF;
            //_devices[8] = _rom8000;
            _devices[deviceCount++] = _rom8000;

            _noDevice = new NoDevice();                      // Was MIDI
            //_devices[9] = _noDevice;
            _devices[deviceCount++] = _noDevice;

            _ram = new RAM();                               // RAM 8K = 0x2000
            _ram.StartsAt= 0x0000;
            _ram.EndsAt = 0x1FFF;
            _ram.SetSize(0x2000);
            //_devices[10] = _ram;
            _devices[deviceCount++] = _ram;

            //_devices[11] = _noDevice;                        // Not sure what this is for
            _devices[deviceCount++] = _noDevice;

            _dataBus = new DataBus(_devices);
            _addressBus = new AddressBus(_devices, _dataBus);
            _addressBus.Init();

            _signetic6502 = new Signetic6502(_addressBus,_dataBus);
            _clock = new Clock(_signetic6502);

            _vdu.Init();
			
			_lines = lines;
            if (_lines == 16)
            {
                _monitor.Data[0x3bc] = 0x2f;
                _monitor.Data[0x3bd] = 0x4c;
                _monitor.Data[0x3be] = 0xd0;
                _monitor.Data[0x3bf] = 0x8c;
                _monitor.Data[0x3c0] = 0xd3;
                _monitor.Data[0x0222] = 0x47;
                _ram.Data[0x0223] = 0x0c;
                _ram.Data[0x0224] = 0xd0;
                _ram.Data[0x0225] = 0xcc;
                _ram.Data[0x0226] = 0xd1;
            }
            else
            {
                _monitor.Data[0x3bc] = 0x2f;
                _monitor.Data[0x3bd] = 0x4c;
                _monitor.Data[0x3be] = 0xd0;
                _monitor.Data[0x3bf] = 0x8c;
                _monitor.Data[0x3c0] = 0xd7;
                _ram.Data[0x0222] = 0x47;
                _ram.Data[0x0223] = 0x0c;
                _ram.Data[0x0224] = 0xd0;
                _ram.Data[0x0226] = 0xd3;
            }

            Debug.WriteLine("Out Init()");
        }

        public void SetLines(int lines)
        {
            // I think this is CEGMON specific

            _lines = lines;
            if (_lines == 16)
            {
                _monitor.Data[0x3bc] = 0x2f;
                _monitor.Data[0x3bd] = 0x4c;
                _monitor.Data[0x3be] = 0xd0;
                _monitor.Data[0x3bf] = 0x8c;
                _monitor.Data[0x3c0] = 0xd3;
                _ram.Data[0x0222] = 0x47;
                _ram.Data[0x0223] = 0x0c;
                _ram.Data[0x0224] = 0xd0;
                _ram.Data[0x0225] = 0xcc;
                _ram.Data[0x0226] = 0xd1;
            }
            else
            {
                _monitor.Data[0x3bc] = 0x2f;
                _monitor.Data[0x3bd] = 0x4c;
                _monitor.Data[0x3be] = 0xd0;
                _monitor.Data[0x3bf] = 0x8c;
                _monitor.Data[0x3c0] = 0xd7;
                _ram.Data[0x0222] = 0x47;
                _ram.Data[0x0223] = 0x0c;
                _ram.Data[0x0224] = 0xd0;
                _ram.Data[0x0226] = 0xd3;
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

            _ram.SetSize(0x2000);   // Clear the ram
            _signetic6502.Reset();

            Debug.WriteLine("Out Reset()");
        }

        public void Dispose()
        {
            Debug.WriteLine("In Dispose()");

            Dispose(disposing: true);
            GC.SuppressFinalize(this);

            Debug.WriteLine("Out Dispose()");

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
