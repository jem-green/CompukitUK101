using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UK101Library
{
    public class UK101
    {
        // Aiming to restructure the objects so that the UK101
        // creates the processor, clock and the memory bus.
        // I think the keyboard matrix should probably be here.
        // Not sure about the display and tape as i think these are
        // part of the console interfaces

        #region Fields

        private readonly IPeripheralIO _peripheralIO;
        private Signetic6502 _signetic6502;
        private Clock _clock;
        private MemoryBus _memoryBus;

        #endregion
        #region Constructor
        public UK101(IPeripheralIO peripheralIO)
        {
            _peripheralIO = peripheralIO;
        }
        #endregion
        #region Properties

        public MemoryBus MemoryBus
        {
            set
            {
                _memoryBus = value;
            }
            get
            {
                return (_memoryBus);
            }
        }
        #endregion
        #region Methods

        public void Init()
        {
            Debug.WriteLine("In Init()");

            _memoryBus = new MemoryBus(_peripheralIO);
            _signetic6502 = new Signetic6502(_memoryBus);
            _clock = new Clock(_signetic6502);
            _signetic6502._memoryBus.VDU.Init();

            int lines = 32;
            if (lines == 16)
            {
                _signetic6502._memoryBus.Monitor.pData[0x3bc] = 0x2f;
                _signetic6502._memoryBus.Monitor.pData[0x3bd] = 0x4c;
                _signetic6502._memoryBus.Monitor.pData[0x3be] = 0xd0;
                _signetic6502._memoryBus.Monitor.pData[0x3bf] = 0x8c;
                _signetic6502._memoryBus.Monitor.pData[0x3c0] = 0xd3;
                _signetic6502._memoryBus.RAM.pData[0x0222] = 0x47;
                _signetic6502._memoryBus.RAM.pData[0x0223] = 0x0c;
                _signetic6502._memoryBus.RAM.pData[0x0224] = 0xd0;
                _signetic6502._memoryBus.RAM.pData[0x0225] = 0xcc;
                _signetic6502._memoryBus.RAM.pData[0x0226] = 0xd1;
            }
            else
            {
                _signetic6502._memoryBus.Monitor.pData[0x3bc] = 0x2f;
                _signetic6502._memoryBus.Monitor.pData[0x3bd] = 0x4c;
                _signetic6502._memoryBus.Monitor.pData[0x3be] = 0xd0;
                _signetic6502._memoryBus.Monitor.pData[0x3bf] = 0x8c;
                _signetic6502._memoryBus.Monitor.pData[0x3c0] = 0xd7;
                _signetic6502._memoryBus.RAM.pData[0x0222] = 0x47;
                _signetic6502._memoryBus.RAM.pData[0x0223] = 0x0c;
                _signetic6502._memoryBus.RAM.pData[0x0224] = 0xd0;
                _signetic6502._memoryBus.RAM.pData[0x0226] = 0xd3;
            }

            Debug.WriteLine("Out Init()");
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

        #endregion

    }
}
