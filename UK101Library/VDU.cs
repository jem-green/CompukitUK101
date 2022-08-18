using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace UK101Library
{
    /// <summary>
    /// This is the VDU RAM
    /// </summary>
    public class VDU : MemoryBusDevice
    {
        // There are some elements here that i would
        // like to separate out so that the display can
        // have specific settings that just read the VDU data
        // The advantage currently is that the display isnt polling
        // the memory but gets a notification when the data changes

        #region Fields

        public bool inScene;
        public byte pCharData;
        public bool Changed;
        private IPeripheralIO _peripheralIO;
        private UInt16 _ramSize;

        #endregion
        #region Constructor

        public VDU(IPeripheralIO peripheralIO)
        {
            _peripheralIO = peripheralIO;
            _ramSize = 4096;
            _data = new byte[RAMSize];
        }

        #endregion
        #region Properties

        public UInt16 RAMSize
        {
            get
            {
                return (_ramSize);
            }
            set
            {
                _ramSize = value;
            }
        }

        #endregion
        #region Methods

        // Window size in CEGMON is stored at 0x0222 - 0x0226 (546 - 550):
        // SWIDTH column width (-1)     0x0222 = 0x2f (47)
        // SLTOP low byte of top        0x0223 = 0x0c (12)
        // SHTOP high byte of top       0x0224 = 0xd0 (208)
        // SLBASE low byte of base      0x0225 = 0xcc (204)
        // SHBASE high byte of base     0x0226 = 0xd3 (211)
        // Number of lines in CEGMON is stored at 0X0223 (547) and 0x0225 (549)
        // 0x0c (12), 0xcc (204) = 32 lines
        // 0x0c (12), 0xa0 (160) = 16 lines
        // Cursor Y position in 0x0200 (512)
        // CEGMON will reinitialize thos at reset, so to make screen 16 lines 
        // we have to patch CEGMON:
        // 0xfbc0 (in CEGMON file byte 0x3c0 (960)) = 0xd7 for 32 lines and 0x37 for 16 lines.
        //
        // 16 rows
        // Monitor.pData[0x3bc] = 0x2f;
        // Monitor.pData[0x3bd] = 0x4c;
        // Monitor.pData[0x3be] = 0xd0;
        // Monitor.pData[0x3bf] = 0x8c;
        // Monitor.pData[0x3c0] = 0xd3;
        // RAM.pData[0x0222] = 0x47;
        // RAM.pData[0x0223] = 0x0c;
        // RAM.pData[0x0224] = 0xd0;
        // RAM.pData[0x0225] = 0xcc;
        // RAM.pData[0x0226] = 0xd1;
        //
        // Monitor.pData[0x3bc] = 0x2f;
        // Monitor.pData[0x3bd] = 0x4c;
        // Monitor.pData[0x3be] = 0xd0;
        // Monitor.pData[0x3bf] = 0x8c;
        // Monitor.pData[0x3c0] = 0xd7;
        // RAM.pData[0x0222] = 0x47;
        // RAM.pData[0x0223] = 0x0c;
        // RAM.pData[0x0224] = 0xd0;
        // RAM.pData[0x0225] = 0xcc;
        // RAM.pData[0x0226] = 0xd3;

        public void Init()
        {
            // simulate the random data

            Random random = new Random(43);
            random.NextBytes(_data);
            for (int i = 0; i < _ramSize; i++)
            {
                byte column = (byte)(i % 64);
                byte row = (byte)(i / 64);
                _peripheralIO.Out(row, column, _data[i]);
            }
        }

        public void ClearScreen()
        {
            // Clear the memory

            for (int i = 0; i < _ramSize; i++)
            {
                _data[i] = 32;
                byte column = (byte)(i % 64);
                byte row = (byte)(i / 64);
                _peripheralIO.Out(row, column, _data[i]);
            }
        }

        public override void Write(byte InData)
        {
            Int32 position = _address - _startsAt;
            _data[position] = InData;;
            byte column = (byte)(position % 64);
            byte row = (byte)(position / 64);
            _peripheralIO.Out(row, column, InData);
        }

        public override byte Read()
        {
            return _data[_address - StartsAt];
        }

        #endregion
        #region Private
        #endregion
    }
}
