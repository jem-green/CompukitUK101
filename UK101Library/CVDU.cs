using System;
using System.Collections.Generic;
using System.Text;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Media.Imaging;

namespace UK101Library
{
    public class CVDU : CMemoryBusDevice
    {
        public bool inScene;
        public UInt16 RAMSize { get; set; }
        public byte NumberOfLines
        {
            set
            {
                if (numberOfLines != value)
                {
                    numberOfLines = value;
                    if (numberOfLines == 16)
                    {
                        Console.Clear();
                        for (byte row = 0; row < 16; row++)
                        {
                            for (byte col = 0; col < 64; col++)
                            {
                                AddChar(row, col);
                            }
                        }
                    }
                    else
                    {
                        for (byte row = 16; row < 32; row++)
                        {
                            for (byte col = 0; col < 64; col++)
                            {
                                AddChar(row, col);
                            }
                        }

                    }
                    SetScreenSize();
                }
            }
        }

        private void AddChar(byte row, byte col)
        {
            Console.CursorLeft = col;
            Console.CursorTop = row;
            Int32 charNumber = pData[col + row * 64];
            Console.Write((char)charNumber);
        }

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
        public void SetScreenSize()
        {
            if (numberOfLines == 16)
            {
                //mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3bc] = 0x2f;
                //mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3bd] = 0x4c;
                //mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3be] = 0xd0;
                mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3bf] = 0x8c;
                mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3c0] = 0xd3;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0222] = 0x47;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0223] = 0x0c;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0224] = 0xd0;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0225] = 0xcc;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0226] = 0xd1;
            }
            else
            {
                //mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3bc] = 0x2f;
                //mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3bd] = 0x4c;
                //mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3be] = 0xd0;
                mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3bf] = 0x8c;
                mainPage.CSignetic6502.MemoryBus.Monitor.pData[0x3c0] = 0xd7;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0222] = 0x47;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0223] = 0x0c;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0224] = 0xd0;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0225] = 0xcc;
                //mainPage.CSignetic6502.MemoryBus.RAM.pData[0x0226] = 0xd3;
            }
        }

        public byte[] pData;
        public byte pCharData;
        public bool Changed;
        public MainPage mainPage;

        private byte numberOfLines;

        public CVDU()
        {
            inScene = false;
            pCharData = 0;
            RAMSize = 4096;
            pData = new byte[RAMSize];
        }

        public void InitCVDU(MainPage mainPage)
        {

            numberOfLines = 32;

            Random random = new Random(43);
            byte[] garbage = new byte[32 * 64];
            random.NextBytes(garbage);
            for (Int32 row = 0; row < 32; row++)
            {
                for (Int32 col = 0; col < 64; col++)
                {
                    Console.CursorLeft = col;
                    Console.CursorTop = row;
                    Int32 charNumber = garbage[row * 16 + col];
                    Console.Write((char)charNumber);
                }
            }
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public override void Write(byte InData)
        {
            pData[Address - StartsAt] = InData;
            Int32 position = Address - StartsAt;
            byte col = (byte)(position % 64);
            byte row = (byte)(position / 64);
            Console.CursorLeft = col;
            Console.CursorTop = row;
            Console.Write((char)InData);
        }

        public override byte Read()
        {
            return pData[Address - StartsAt];
        }
    }
}
