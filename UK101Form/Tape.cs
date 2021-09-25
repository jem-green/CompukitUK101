using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UK101Library;

namespace UK101Form
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

        MainPage _mainPage;
        private Timer _timer;
        MemoryStream _memoryStream;
        BinaryWriter binaryWriter;
        readonly object _lockObject = new Object();

        #endregion
        #region Constructors

        public Tape(MainPage mainPage)
        {
            _mainPage = mainPage;
            basicProg = new BasicProg();
            Lines = null;
            _timer = new Timer(Timer_Tick, null, Timeout.Infinite, 10); // Create the Timer delay starting
            
        }

        #endregion
        #region Propertes

        public BasicProg basicProg { get; set; }
        public String[] Lines { get; set; }
        public Int16 line { get; set; }

        #endregion


        public void Load()
        {

        }

        public void Stop(string filename)
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
        }

        public void Record()
        {
            // Create a file stream
            // How do we detect when the stream has stopped being
            // written to by the ACIA

            _memoryStream = new MemoryStream();
            _memoryStream.Seek(0, SeekOrigin.Begin);
            _mainPage.CSignetic6502.MemoryBus.ACIA.FileOutputStream = _memoryStream;

        }

        public void Play(string filename)
        {
            if (File.Exists(filename) == true)
            {
                _memoryStream = new MemoryStream();
                FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);

                file.CopyTo(_memoryStream);
                _memoryStream.Seek(0, SeekOrigin.Begin);    // Move to the beginning of the file
                _mainPage.CSignetic6502.MemoryBus.ACIA.FileInputStream = _memoryStream;
            }
            else
            {
                throw new Exception("File missing");
            }
        }


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
                        binaryWriter.Write(b);
                        binaryWriter.Flush();
                    }
                }
                while (1 == 1);
            }
        }
    }
}
