using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;

namespace UK101Library
{
    /// <summary>
    /// This is the VDU RAM
    /// </summary>
    public class CVDU : CMemoryBusDevice
    {
        // There are some elements here that i would
        // like to separate out so that the display can
        // have specific settings that just read the VDU data
        // The advantage currently is that the display isnt polling
        // the memory but gets a notification when the data changes


        #region Variables

        public bool inScene;
        public byte[] pData;
        public byte pCharData;
        public bool Changed;
        public MainPage mainPage;
        private byte numberOfLines;
        private CHARGEN chargen;


        #endregion
        #region Constructor
        public CVDU()
        {
            inScene = false;
            pCharData = 0;
            RAMSize = 4096;
            pData = new byte[RAMSize];

            // Load

            chargen = new CHARGEN(0x0);

        }

        #endregion
        #region Properties

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
                        //Console.Clear();
                        //for (byte row = 0; row < 16; row++)
                        //{
                        //    for (byte col = 0; col < 64; col++)
                        //    {
                        //        AddChar(row, col);
                        //    }
                        //}
                    }
                    else
                    {
                        //for (byte row = 16; row < 32; row++)
                        //{
                        //    for (byte col = 0; col < 64; col++)
                        //    {
                        //        AddChar(row, col);
                        //    }
                        //}

                    }
                    SetScreenSize();
                }
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
        public void InitCVDU(MainPage mainPage)
        {

            numberOfLines = 32;
            this.mainPage = mainPage;
            Random random = new Random(43);
            byte[] garbage = new byte[32 * 64];
            random.NextBytes(garbage);

            // simulate the random data

            int rows = 32;
            int columns = 64;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    pData[column + row * columns] = garbage[column + row * columns];
                }
            }
            mainPage.pictureBox.Invalidate();
            Thread.Sleep(1000);
        }

        public void ClearScreen()
        {
            // May need to clear the screen
        }

        public Bitmap Generate()
        {
            // Need to get the scaling factor sorted

            int _width = 64;
            int _height = 32;
            int Horizontal = 8;
            int Vertical = 8;

            Bitmap bmp = new Bitmap(_width * Horizontal, _height * Vertical, PixelFormat.Format8bppIndexed);

            BitmapData bmpCanvas = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Get the address of the first line.

            IntPtr ptr = bmpCanvas.Scan0;

            // Declare an array to hold the bytes of the bitmap.

            int size = bmp.Width * bmp.Height;
            byte[] rgbValues = new byte[size];

            int rows = _height;
            int columns = _width;
            int hbits = 8;
            int vbits = 8;

            // work across character by character

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    byte character = pData[column + row * columns];
                    if (character != 32)
                    {
                        for (int i = 0; i < vbits; i++) // rows
                        {
                            byte data = chargen.pData[character * vbits + i];
                            for (int j = hbits - 1; j > -1; j--) // columns
                            {
                                byte val = (byte)(data & (byte)Math.Pow(2, 7 - j));
                                if (val > 0)
                                {
                                    rgbValues[(row * hbits + i) * columns * vbits + column * vbits + j] = 255;
                                }
                            }
                        }
                    }
                }
            }

            // Copy the 256 bit values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, size);

            bmp.UnlockBits(bmpCanvas);

            return (bmp);
        }

        public override void Write(byte InData)
        {
            pData[Address - StartsAt] = InData;
            mainPage.pictureBox.Invalidate();
        }

        public override byte Read()
        {
            return pData[Address - StartsAt];
        }

        #endregion
        #region Private
        private void AddChar(byte row, byte col)
        {
            Console.CursorLeft = col;
            Console.CursorTop = row;
            Int32 charNumber = pData[col + row * 64];
            Console.Write((char)charNumber);
        }

        #endregion
    }
}
