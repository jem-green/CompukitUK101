using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    /*
* The keyboard matrix has the following layout:
*
*
*
*                         01111  0111  1011  1101  1110
*         127   191   223   239   247   251   253   254
*          C7    C6    C5    C4    C3    C2    C1    C0
*           |     |     |     |     |     |     |     |
*         ! |   " |   # |   $ |   % |   & |   ' |     |
*         1 |   2 |   3 |   4 |   5 |   6 |   7 |     |
*  R7 ------+-----+-----+-----+-----+-----+-----+-----+
*         ( |   ) |(1)  |   * |   = | RUB |     |     |
*         8 |   9 |   0 |   : |   - | OUT |     |     |
*  R6 ------+-----+-----+-----+-----+-----+-----+-----+
*         > |   \ |     |(2)  |     |     |     |     |
*         . |   L |   O |   ^ |  CR |     |     |     |
*  R5 ------+-----+-----+-----+-----+-----+-----+-----+
*           |     |     |     |     |     |     |     |
*         W |   E |   R |   T |   Y |   U |   I |     |
*  R4 ------+-----+-----+-----+-----+-----+-----+-----+
*           |     |     |     |     |  LF |   [ |     |
*         S |   D |   F |   G |   H |   J |   K |     |
*  R3 ------+-----+-----+-----+-----+-----+-----+-----+
*           | ETX |     |     |     |   ] |   < |     |
*         X |   C |   V |   B |   N |   M |   , |     |
*  R2 ------+-----+-----+-----+-----+-----+-----+-----+
*           |     |     |     |   ? |   + |   @ |     |
*         Q |   A |   Z |space|   / |   ; |   P |     |
*  R1 ------+-----+-----+-----+-----+-----+-----+-----+
*      (3)  |     |(4)  |     |     | left|right|SHIFT|
*           | CTRL|     |     |     |SHIFT|SHIFT| LOCK|
*  R0 ------+-----+-----+-----+-----+-----+-----+-----+
*  
*  (1) Both MONUK02 and CEGMON decode shift-0 as @
*  
* Notes for Ohio Superboard II keyboard:
*  (2) This key is labelled LINE FEED
*  (3) This position is the REPEAT key
*  (4) This position is the ESC key
*/


    public class CKeyboard : CMemoryBusDevice
    {

        public byte[] Keystates;
        private byte lastInData = 0xff;
        public Boolean loadResetIsNeeded { get; set; }

        public CKeyboard()
        {
            loadResetIsNeeded = false;
            Keystates = new byte[8];
            Reset();
        }

        public void Reset()
        {
            // Initiate Keystates:
            Keystates[0] = 0xFF;
            Keystates[1] = 0xFF;
            Keystates[2] = 0xFF;
            Keystates[3] = 0xFF;
            Keystates[4] = 0xFF;
            Keystates[5] = 0xFF;
            Keystates[6] = 0xFF;
            Keystates[7] = 0xFF;
        }

        public void PressKey(byte row, byte col)
        {
            Keystates[row] = (byte)(Keystates[row] ^ (0x80 >> (col))); // E.g. 1110 1111 ^ 0000 0100 = 1110 1011
        }

        public void ReleaseKey(byte row, byte col)
        {
            Keystates[row] = (byte)(Keystates[row] | (0x80 >> col)); // E.g. 1110 1011 | 0000 0100 = 1110 1111
        }

        public override byte Read()
        {
            if (loadResetIsNeeded && Data == 0xfd)
            {
                // Special treatment.
                // People use to put "RUN" at end of listing in order to run the app,
                // followed by on last line containing only one space.
                // The manual states procedure to use if "RUN" and pace is not inlcuded
                // in listing only, and the user is told to get back to normal (not load)
                // mode by pressing space and then enter.
                // However, this does not work here, so if a "RUN" line is encountered
                // we must tell keyboard input routine to reset the load by pressing
                // space:
                loadResetIsNeeded = false;
                return 0xef;
            }

            // Inverted bits for row selection.
            // Row 0 = 254 (1111 1110), row 1 = 253 (1111 1101)... row 7 = 127 (0111 1111).
            // Loop through the bits and test selected rows. 
            // Reset corresponding bits in outdata:
            byte OutData = 0xFF;
            for (int i = 0; i < 8; i++)
            {
                // Is the row selected? (Bit at location 'i' is zero.)
                if ((Data & (0x80 >> i)) == 0) // 1101 1011 & 0000 1000 = 0000 1000 (no), 1101 1011 & 0000 0100 = 0000 0000(yes)
                {
                    // Mask 0's from corresponding Keystate onto OutData:
                    OutData = (byte)(OutData & Keystates[i]);
                }
            }

            return OutData;
        }

        public override void Write(byte InData)
        {
            // Inverted bits for row selection.
            // Row 0 = 254 (1111 1110), row 1 = 253 (1111 1101)... row 7 = 127 (0111 1111).
            // We need to store which row is selected (only one can be selected at a time).
            Data = InData;
        }
    }
}

