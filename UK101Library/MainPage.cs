using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UK101Library
{
    public class MainPage
    {
        public CSignetic6502 CSignetic6502 { get; set; }
        public PictureBox pictureBox { get; set; }
        public MIDI Midi { get; set; }
    }
}
