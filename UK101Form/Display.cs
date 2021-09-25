using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UK101Library;

namespace UK101Form
{
    public class Display
    {
        // This class needs to do the acutal conversion of
        // VDU RAM into the display. It needs to be outside the core
        // library as it will depend on the client view form/console/app

        // The UK1010 block digram indicates that
        // VdURAM -> CHARGEN -> PISO -> SYNC -> UHF MOD - TV

        // 

        // PISO - parallel in serial out

        // So we can cover
        // start offset - simulate the elements that cannot be viewed by the TV
        // end offset - 
        // lines - simulate a 16 line or 32 line display depending on the country
        // scaling to make readable on the PC.

        CHARGEN _chargen;
        int _width = 64;
        int _height = 32;
        int Horizontal = 8;
        int Vertical = 8;
        MainPage _mainPage;

        public Display(MainPage mainPage)
        {
            _mainPage = mainPage;
        }

        public Bitmap Generate()
        {
            // Need to get the scaling factor sorted

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
                    byte character = _mainPage.CSignetic6502.MemoryBus.VDU.pData[column + row * columns];
                    if (character != 32)
                    {
                        for (int i = 0; i < vbits; i++) // rows
                        {
                            byte data = _chargen.pData[character * vbits + i];
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


    }
}
