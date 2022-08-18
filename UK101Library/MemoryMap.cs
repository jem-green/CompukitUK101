using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UK101Library
{
    public class MemoryMap
    {
        /*
         * 0x0000-0x9fff	40KB RAM
         * 0xa000-0xbfff	8KB BASIC in ROM
         * 0xc000-0xcfff	External SRAM (not viewable by BASIC)
         * 0xd000-0xd3ff	1KB Display RAM
         * 0xdc00-0xdfff	Polled Keyboard
         * 0xf000-0xf001	ACIA Serial port
         * 0xf800-0xffff	2KB MONITOR ROM
         */

        #region Fields

        private byte[] _map = new byte[0x10000];

        #endregion
        #region Constructors

        public MemoryMap()
        {
            for (Int32 address = 0; address < 0x10000; address++)
            {
                if (address >= 0xf800)
                {
                    Map[address] = 0;
                }
                else if (address >= 0xf000 && address <= 0xf0ff)    // ACAI Device[1]
                {
                    Map[address] = 1;
                }
                else if (address == 0xdf00)                         // KEYBOARD Device[2]
                {
                    Map[address] = 2;
                }
                else if (address >= 0xd000 && address <= 0xd7ff)    // VDU Device[3]
                {
                    Map[address] = 3;
                }
                else if (address >= 0xb800 && address <= 0xbfff)    // BASIC4 Device[4]
                {
                    Map[address] = 4;
                }
                else if (address >= 0xb000 && address <= 0xb7ff)    // BASIC3 Device[5]
                {
                    Map[address] = 5;
                }
                else if (address >= 0xa800 && address <= 0xafff)    // BASIC2 Device[6]
                {
                    Map[address] = 6;
                }
                else if (address >= 0xa000 && address <= 0xa7ff)    // BASIC1 Device[7]
                {
                    Map[address] = 7;
                }
                else if (address >= 0x8000 && address <= 0x8fff)    // ROM8000 Device[8]
                {
                    Map[address] = 8;
                }
                else if (address == 0x6001)
                {
                    Map[address] = 9;
                }
                else if (address < 0x2000)                          // RAM Device[10]
                {
                    Map[address] = 10;
                }
                else
                {
                    Map[address] = 11;
                }

            }
        }

        #endregion
        #region Properties

        public byte[] Map
        {
            set
            {
                _map = value;
            }
            get
            {
                return (_map);
            }
        }

        #endregion
    }
}
