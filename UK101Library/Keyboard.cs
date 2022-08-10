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

    public class Keyboard : MemoryBusDevice
    {
        #region Variables

        private byte lastInData = 0xff;
        private IPeripheralIO _peripheralIO;
        public Boolean loadResetIsNeeded { get; set; }

        #endregion
        #region Constructors

        public Keyboard(IPeripheralIO peripheralIO)
        {
            _peripheralIO = peripheralIO;
            loadResetIsNeeded = false;
            Reset();
        }

        #endregion
        #region Methods

        public void Reset()
        {
            // Initiate Keystates:
            _peripheralIO.KeyStates[0] = 0xFF;
            _peripheralIO.KeyStates[1] = 0xFF;
            _peripheralIO.KeyStates[2] = 0xFF;
            _peripheralIO.KeyStates[3] = 0xFF;
            _peripheralIO.KeyStates[4] = 0xFF;
            _peripheralIO.KeyStates[5] = 0xFF;
            _peripheralIO.KeyStates[6] = 0xFF;
            _peripheralIO.KeyStates[7] = 0xFF;
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
            // zero bit moves from low to high
            //
            // Row 0 = 254 (1111 1110)
            // row 1 = 253 (1111 1101)
            // row 2 = 251 (1111 1011)
            // row 3 = 247 (1111 0111)
            // row 4 = 239 (1110 1111)
            // row 5 = 223 (1101 1111)
            // row 6 = 191 (1011 1111)
            // row 7 = 127 (0111 1111)
            //
            // Loop through the bits and test selected rows.
            // The selected row is stored in data field.
            // Reset corresponding bits in outdata:

            byte OutData = 0xFF;
            for (int i = 0; i < 8; i++)
            {
                // Is the row selected? (Bit at location 'i' is zero.)
                // 1101 1011 & 0000 1000 = 0000 1000 (no)
                // 1101 1011 & 0000 0100 = 0000 0000 (yes)
                if ((Data & (0x80 >> i)) == 0)
                {
                    // Mask 0's from corresponding Keystate onto OutData:
                    OutData = (byte)(OutData & _peripheralIO.KeyStates[i]);
                }
            }

            return OutData;
        }

        public override void Write(byte InData)
        {
            // Inverted bits for row selection.
            // zero bit moves from low to high
            // The 
            //
            // Row 0 = 254 (1111 1110)
            // row 1 = 253 (1111 1101)
            // row 2 = 251 (1111 1011)
            // row 3 = 247 (1111 0111)
            // row 4 = 239 (1110 1111)
            // row 5 = 223 (1101 1111)
            // row 6 = 191 (1011 1111)
            // row 7 = 127 (0111 1111)
            //
            // We need to store which row is selected (only one can be selected at a time).
            // The selected row is stored in Data
            //
            Data = InData;
        }
        #endregion
    }
}