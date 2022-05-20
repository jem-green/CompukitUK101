using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UK101Library;
using TracerLibrary;


namespace UK101Form
{
    public class FormIO : IPeripheralIO
    {
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

        // Formatting constraints

        private int _consoleHeight;
        private int _consoleWidth;
        private int _consoleLeft;
        private int _consoleTop;
        private int _zoneWidth = 15;
        private int _compactWidth = 3;

        public byte[] _keystates;    // Store the key states

        private Cursor _cursor;
        private string _input = "";
        private string _output = "";
        protected readonly object _lockObject = new Object();
        private MemoryStream _inStream;
        private MemoryStream _outStream;

        private Display _display;

        struct Cursor
        {
            int _left;
            int _top;

            public Cursor(int left, int top)
            {
                _left = left;
                _top = top;
            }

            public int Left
            {
                get
                {
                    return (_left);
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
                    return (_top);
                }
                set
                {
                    _top = value;
                }
            }
        }

        #endregion
        #region Constructors
        public FormIO(Display display)
        {
            _keystates = new byte[8];
            _display = display;     
        }

        #endregion
        #region Properties

        public int Width
        {
            get
            {
                return (_consoleWidth);
            }
            set
            {
                _consoleWidth = value;
            }
        }

        public int Height
        {
            get
            {
                return (_consoleHeight);
            }
            set
            {
                _consoleHeight = value;
            }
        }

        public int Left
        {
            get
            {
                return (_consoleLeft);
            }
            set
            {
                _consoleLeft = value;
            }
        }

        public int Top
        {
            get
            {
                return (_consoleTop);
            }
            set
            {
                _consoleTop = value;
            }
        }

        public MemoryStream Transmit
        {
            get
            {
                return (_inStream);
            }
            set
            {
                _inStream = value;
            }
        }

        public MemoryStream Receive
        {
            get
            {
                return (_outStream);
            }
            set
            {
                _outStream = value;
            }
        }

        /// <summary>
        /// Potentiall change this to a method rather than
        /// exposing the array of key states
        /// </summary>
        public byte[] KeyStates
        {
            get
            {
                return (_keystates);
            }
        }

        public string Input
        {
            set
            {
                // need to wait here while the input is being read
                lock (_lockObject)
                {
                    _input = _input + value;
                }
            }
        }

        public string Output
        {
            get
            {
                string temp;
                // need to wait here while the output is being written
                lock (_lockObject)
                {
                    temp = _output;
                    _output = "";
                }
                return (temp);
            }
        }

        public int CursorLeft
        {
            get
            {
                return (_cursor.Left);
            }
            set
            {
                _cursor.Left = value;
            }
        }

        public int CursorTop
        {
            get
            {
                return (_cursor.Top);
            }
            set
            {
                _cursor.Top = value;
            }
        }

        public int Zone
        {
            get
            {
                return (_zoneWidth);
            }
            set
            {
                _zoneWidth = value;
            }
        }

        public int Compact
        {
            get
            {
                return (_compactWidth);
            }
            set
            {
                _compactWidth = value;
            }
        }

        #endregion
        #region Methods

        public void Out(string s)
        {
            lock (_lockObject)
            {
                s = s.Replace("\n", "\r\n");
                _output = _output + s;
            }
            TextEventArgs args = new TextEventArgs(s);
            OnTextReceived(args);
        }

        /// <summary>
        /// Put character at a specific location
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="character"></param>
        public void Out(int row, int column, byte character)
        {
            int start = Environment.TickCount;
            lock (_lockObject)
            {
                _cursor.Left = column - _consoleLeft;
                _cursor.Top = row - _consoleTop;
                if ((_cursor.Left >= 0) && (_cursor.Top >= 0))
                {
                    if ((_cursor.Left < _consoleWidth) && (_cursor.Top < _consoleHeight))
                    {
                        _display.Write(_cursor.Left, _cursor.Top, character);

                        TextEventArgs args = null; //= new TextEventArgs("");
                        OnTextReceived(args);
                    }
                }
            }
        }

        /// <summary>
        /// Get a keyboard character
        /// </summary>
        /// <returns></returns>
        public string In()
        {
            string value = "";
            do
            {
                while (_input.Length == 0)
                {
                    System.Threading.Thread.Sleep(250); // Loop until input is entered.
                }
                int pos = 0;
                lock (_lockObject)
                {
                    pos = _input.IndexOf('\n');
                    if (pos < 0)
                    {
                        pos = _input.IndexOf('\r');
                    }
                    if (pos > -1)
                    {
                        // read the input to the first \n or \r then trim the remaining
                        value = _input.Substring(0, pos);
                        _input = _input.Substring(pos + 1, _input.Length - pos - 1);
                    }
                }
            }
            while (value.Length == 0);
            return (value);
        }

        public void PressKey(byte row, byte col)
        {
            // If col = 1
            // 1000 0000 >> 0100 0000
            // 1111 1111 xor 0100 0000 = 1011 1111
            //
            // if col 4 already set and then col 6 is set
            // 1110 1111 ^ 0000 0100 = 1110 1011
            _keystates[row] = (byte)(_keystates[row] ^ (0x80 >> (col)));
        }

        public void ReleaseKey(byte row, byte col)
        {
            // if col 4 and 6 are already set and then col 6 is reset
            // 1110 1011 | 0000 0100 = 1110 1111
            _keystates[row] = (byte)(_keystates[row] | (0x80 >> col));
        }

        public void Error(string e)
        {
            Debug.WriteLine(e);
        }

        public void Reset()
        {
            _input = "";
            _output = "";
        }

        public void WriteData(byte data)
        {

        }

        public byte ReadData()
        {
            byte data = 0;
            return (data);
        }

        #endregion
    }
}
