using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UK101Library;

namespace UK101Console
{
    public class Tape
    {
        #region Fields

        // Tape mechanism
        // Add a file to the Tape
        // allow the ACIA to hook into the Tape
        // rather than the program line option
        // First port the file mechnism to here
        // then consider refactoring

        private Timer _timer;
        MemoryStream _memoryStream;
        BinaryWriter _binaryWriter;
        readonly object _lockObject = new Object();
        TapeMode _mode = TapeMode.Stopped;
        IPeripheralIO _peripheralIO;

        [Flags]
        public enum TapeMode : byte
        {
            Stopped = 0,
            Playing  = 1,
            Recording = 2
        }

        #endregion
        #region Constructors

        public Tape(IPeripheralIO peripheralIO)
        {
            _peripheralIO = peripheralIO;
            _timer = new Timer(Timer_Tick, null, Timeout.Infinite, 10); // Create the Timer, delay starting     
        }

        #endregion
        #region Propertes

        public TapeMode Mode
        {
            get
            {
                return (_mode);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return ((_mode & TapeMode.Playing) == TapeMode.Playing);
            }
        }

        public bool IsRecoding
        {
            get
            {
                return ((_mode & TapeMode.Recording) == TapeMode.Recording);
            }
        }

        public bool IsStopped
        {
            get
            {
                return ((_mode & TapeMode.Stopped) == TapeMode.Stopped);
            }
        }

        #endregion
        #region Methods

        public void Open()
        {
            _timer.Change(0, 10);
        }

        public void Close()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;
        }

        public void Stop(string filename)
        {
            // If recording then when the tape is stopped
            // save the data

            if (_mode == TapeMode.Recording)
            {
                if (File.Exists(filename) == true)
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch
                    {
                        throw new Exception("File exists");
                    }
                }

                _memoryStream.Seek(0, SeekOrigin.Begin);
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    _memoryStream.CopyTo(fs);
                    fs.Flush();
                }
                
                // Was transmitting to tape
                // Not sure how well this needs closing and disposing of
                _peripheralIO.Transmit.Close();
                _peripheralIO.Transmit.Dispose();
                _peripheralIO.Transmit = null;
                _memoryStream.Close();
                _memoryStream.Dispose();
                _memoryStream = null;
                _mode = TapeMode.Stopped;
            }
            else if (_mode == TapeMode.Playing)
            {
                // Was recieving from tape
                // Not sure how well this needs closing and disposing of
                _peripheralIO.Receive.Close();
                _peripheralIO.Receive.Dispose();
                _peripheralIO.Receive = null;
                _memoryStream.Close();
                _memoryStream.Dispose();
                _memoryStream = null;
                _mode = TapeMode.Stopped;
            }
        }

        public void Record()
        {
            // Create a file stream
            // How do we detect when the stream has stopped being
            // written to by the ACIA. The answer is you cannot so
            // its a manual stop. The UK101 logic appeears to be
            // use LOAD command to stop the ACIA

            if (_mode == TapeMode.Stopped)
            {
                _mode = TapeMode.Recording;
                _memoryStream = new MemoryStream();
                _memoryStream.Seek(0, SeekOrigin.Begin);
                _peripheralIO.Transmit = _memoryStream;     // Transmit to tape
            }
            else
            {
                Debug.WriteLine("Tape is not stopped");
            }
        }

        public void Play(string filename)
        {
            if (_mode == TapeMode.Stopped)
            {
                if (File.Exists(filename) == true)
                {
                    _mode = TapeMode.Playing;
                    _memoryStream = new MemoryStream();
                    FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);

                    file.CopyTo(_memoryStream);
                    _memoryStream.Seek(0, SeekOrigin.Begin);   
                                                             
                    _peripheralIO.Receive = _memoryStream;  // Recieve from tape
                }
                else
                {
                    Debug.WriteLine("File missing");
                }
            }
            else
            {
                Debug.WriteLine("Tape is not stopped");
            }
        }

        #endregion
        #region Private

        private void Timer_Tick(object sender)
        {
            byte b;
            int data;
            lock (_lockObject)
            {
                do
                {
                    data = _memoryStream.ReadByte();
                    if (data != -1)
                    {
                        b = (byte)data;

                        // Slightly different approach was to 
                        // write out the bytes as they were added to the 
                        // memory stream by the ACIA. Currently have chosen
                        // a stop method and do the write at the end

                        _binaryWriter.Write(b);
                        _binaryWriter.Flush();
                    }
                }
                while (1 == 1);
            }
        }
        #endregion
    }
}
