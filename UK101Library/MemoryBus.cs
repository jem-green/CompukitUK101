using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    public class MemoryBus
    {
        #region Fields

        private IPeripheralIO _peripheralIO;
        private const Int32 DEVICES_MAX = 12;
        private byte DeviceIndex;
        private MemoryBusDevice[] _device = new MemoryBusDevice[DEVICES_MAX];
        private ushort _address;

        // Reference all the devices

        public MemoryBusDevice Monitor;

        public BASIC1 Basic1;
        public BASIC2 Basic2;
        public BASIC3 Basic3;
        public BASIC4 Basic4;
        public RAM RAM;

        // Want to reference a different VDU here

        public VDU VDU;
        public Keyboard Keyboard;
        public ACIA ACIA;
        public ROM8000 ROM8000;
        public NoDevice NoDevice;

        private MemoryMap _memoryMap;

        #endregion
        #region Constructors

        public MemoryBus(IPeripheralIO peripheralIO)
        {
            _peripheralIO = peripheralIO;
            _memoryMap = new MemoryMap();

            // Clear all device pointers:
            int i;
            for (i = 0; i < DEVICES_MAX; i++)
            {
                _device[i] = null;
            }

            //Monitor = new MON01(Address);
            //Monitor = new MON02(Address);
            Monitor = new CEGMON(Address);
            //Monitor = new WEMON(Address);
            //Monitor = new EXMON(Address);
            Monitor.StartsAt = 0xF800;
            Monitor.EndsAt = 0xFFFF;
            _device[0] = Monitor;

            ACIA = new ACIA(_peripheralIO);             // ACAI
            ACIA.StartsAt = 0xF000;
            ACIA.EndsAt = 0xF0FF;
            _device[1] = ACIA;

            Keyboard = new Keyboard(_peripheralIO);     // Keyboard
            Keyboard.StartsAt = 0xDF00;
            Keyboard.EndsAt = 0xDF00;
            _device[2] = Keyboard;

            VDU = new VDU(_peripheralIO);               // Video
            VDU.StartsAt = 0xD000;
            VDU.EndsAt = 0xD7FF;
            _device[3] = VDU;

            Basic4 = new BASIC4(Address);               // Basic 4
            Basic4.StartsAt = 0xB800;
            Basic4.EndsAt = 0xBFFF;
            _device[4] = Basic4;

            Basic3 = new BASIC3(Address);               // Basic 3
            Basic3.StartsAt = 0xB000;
            Basic3.EndsAt = 0xB7FF;
            _device[5] = Basic3;

            Basic2 = new BASIC2(Address);               // Basic 2
            Basic2.StartsAt = 0xA800;
            Basic2.EndsAt = 0xAFFF;
            _device[6] = Basic2;

            Basic1 = new BASIC1(Address);               // Basic 1
            Basic1.StartsAt = 0xA000;
            Basic1.EndsAt = 0xA7FF;
            _device[7] = Basic1;

            ROM8000 = new ROM8000(Address);             // ROM8000
            ROM8000.StartsAt = 0x8000;
            ROM8000.EndsAt = 0x8FFF;
            _device[8] = ROM8000;

            NoDevice = new NoDevice();                  // Was MIDI
            _device[9] = NoDevice;

            RAM = new RAM();                            // RAM 8K = 0x2000
            RAM.StartsAt = 0x0000;
            RAM.EndsAt = 0x1FFF;
            RAM.SetSize(0x2000);
            _device[10] = RAM;

            NoDevice = new NoDevice();
            _device[11] = NoDevice;
        }

        #endregion
        #region Properties

        public ushort Address
        {
            get
            {
                return (_address);
            }
            set
            {
                _address = value;
            }
        }

        #endregion

        #region Methods

        public void SetAddress(UInt16 Address)
        {
            DeviceIndex = _memoryMap.Map[Address];
            _device[DeviceIndex].SetAddress(Address);
        }

        public void Write(byte Data)
        {
            _device[DeviceIndex].Write(Data);
        }

        public byte Read()
        {
            return _device[DeviceIndex].Read();
        }

        #endregion Methods

    }
}
