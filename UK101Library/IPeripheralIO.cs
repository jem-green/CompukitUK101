using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UK101Library
{
    public interface IPeripheralIO
    {
        #region Peroperties
		
        int Width { get; set; }
        int Height { get; set; }
        int Left { get; set; }
        int Top { get; set; }
		int CursorLeft { get; }
        int CursorTop { get; }
        int Zone { get; set; }
        int Compact { get; set; }
        string Input { set; }
        string Output { get; }

        byte[] KeyStates { get; }
        MemoryStream Transmit { get; set; }
        MemoryStream Receive { get; set; }

        #endregion
        #region Methods

        void Out(int row, int column, byte character);
        string In();
		void Error(string theErr);
        void Reset();

        void PressKey(byte row, byte col);
        void ReleaseKey(byte row, byte col);	
        void WriteData(byte data);
        byte ReadData();

        #endregion
        #region Events

        event EventHandler<TextEventArgs> TextReceived;

        #endregion
    }
}
