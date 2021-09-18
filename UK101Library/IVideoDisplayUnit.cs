using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UK101Library
{
    interface IVideoDisplayUnit
    {
        UInt16 RAMSize { get; set; }
        byte NumberOfLines { set; }
        void SetScreenSize();
        void InitCVDU(MainPage mainPage);
        void ClearScreen();
        void Write(byte InData);
        byte Read();

    }
}
