using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using UK101Library;
using System.Diagnostics;
using TracerLibrary;

namespace UK101Form
{
    public class Display
    {
        // This class needs to do the acutal conversion of
        // VDU RAM into the display. It needs to be outside the core
        // library as it will depend on the client view form/console/app

        // The UK1010 block digram indicates that
        // VdURAM -> CHARGEN -> PISO -> SYNC -> UHF MOD - TV(display)

        // 

        // PISO - parallel in serial out
        // So we can cover
        // start offset - simulate the elements that cannot be viewed by the TV
        // end offset - 
        // lines - simulate a 16 line or 32 line display depending on the country
        // scaling to make readable on the PC.

        #region Event handling

        /// <summary>
        /// Occurs when the Zmachine recives a message.
        /// </summary>
        public event EventHandler<TextEventArgs> TextReceived;

        /// <summary>
        /// Handles the actual event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTextReceived(TextEventArgs e)
        {
            EventHandler<TextEventArgs> handler = TextReceived;
            if (handler != null)
                handler(this, e);
        }

        #endregion
        #region Fields

        private CHARGEN _chargen;

        private int _columns = 64;      // Full display width for memory calculation
        private int _rows = 32;         // Full display height for memory calculation

        private int _width = 64;        // width of visible display
        private int _height = 32;       // height of visible display
        private int _left = 0;          // Left offset of start of visible display
        private int _top = 0;           // top offset of start of visbible dsiplay

        private int _horizontal = 8;    // Number of pixels
        private int _vertical = 8;      // Number of pixels

        private int _scale = 1;         //
        private double _aspect = 1;     // Will need to round to a positive number

        private byte[] _store;

        #endregion
        #region Consructors

        // Not much like dependency injection

        public Display()
        {
            _store = new byte[_columns * _rows];
        }

        #endregion
        #region Properties

        public double Aspect
        {
            set
            {
                _aspect = value;
            }
            get
            {
                return (_aspect);
            }
        }

        public int Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }

        public int Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
            }
        }

        public int Width
        {
            get
            { 
                return _width;
            }
            set
            {
                _width = value;
            }
        }


        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public int Left
        {
            get
            { 
                return _left;
            }
            set
            { 
                _left = value;
            }
        }

        public int Top
        {
            get 
            {
                return _top;
            }
            set
            {
                _top = value;
            }
        }


        public CHARGEN CharacterGenerator
        {
            set
            {
                _chargen = value;
            }
        }

        #endregion
        #region Methods

        public void Write(int column, int row, byte character)
        {
            _store[column + row * _columns] = character;
        }

        public Bitmap Generate()
        {
            int hScale;
            int vscale;
            if (_aspect > 1)
            {
                hScale = (int)(_scale * _aspect);
                vscale = _scale;
            }
            else
            {
                hScale = _scale;
                vscale = (int)(_scale / _aspect);
            }

            // Need to get the scaling factor sorted

            Bitmap bitmap = new Bitmap(_width * _horizontal * hScale, _height * _vertical * vscale, PixelFormat.Format8bppIndexed);

            BitmapData bmpCanvas = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Get the address of the first line.

            IntPtr ptr = bmpCanvas.Scan0;

            // Declare an array to hold the bytes of the bitmap.

            int size = bitmap.Width * bitmap.Height;
            byte[] rgbValues = new byte[size];
			
            // might be quicker to fill the array with background in one go

            int hbits = _horizontal;
            int vbits = _vertical;


            int hbytes = (int)Math.Round((double)hbits / 8);
            // work across character by character

            for (int row = _top; row < _height; row++)
            {
                for (int column = _left; column < _width; column++)
                {
                    byte character = _store[column + row * _columns];

                    for (int r = 0; r < vbits; r++) // rows
                    {
                        byte value = _chargen.pData[character * hbytes * vbits + r];

                        for (int c = 0; c < hbits; c++) // columns
                        {
                            byte val = (byte)(value & (128 >> c));

                            if (val != 0)
                            {
                                if ((hScale == 1) && (vscale == 1) && (_aspect == 1))
                                {
                                    int pos = (row * vbits + r) * _width * hbits + column * hbits + c;
                                    rgbValues[pos] = 255;
                                }
                                else
                                {
                                    for (int i = 0; i < vscale; i++)
                                    {
                                        for (int j = 0; j < hScale; j++)
                                        {
                                            int pos = (row * vbits * vscale + r * vscale + i) * _width * hbits * hScale + column * hbits * hScale + c * hScale + j;
                                            rgbValues[pos] = 255;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((hScale == 1) && (vscale == 1) && (_aspect == 1))
                                {
                                    int pos = (row * vbits + r) * _width * hbits + column * hbits + c;
                                    rgbValues[pos] = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < vscale; i++)
                                    {
                                        for (int j = 0; j < hScale; j++)
                                        {
                                            int pos = (row * vbits * vscale + r * vscale + i) * _width * hbits * hScale + column * hbits * hScale + c * hScale + j;
                                            rgbValues[pos] = 0;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }

            // Copy the 256 bit values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, size);

            bitmap.UnlockBits(bmpCanvas);

            return (bitmap);
        }

        #endregion
    }
}
