using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UK101Form
{

    public class KeyboardMatrix
    {

        /*
        * * The keyboard matrix has the following layout:
        *
        *       C0    C1    C2    C3    C4    C5    C6    C7
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R0 |   ! |   " |   # |   $ |   % |   & |   ' |     |
        *     |   1 |   2 |   3 |   4 |   5 |   6 |   7 |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R1 |   ( |   ) | (1) |   * |   = | RUB |     |     |
        *     |   8 |   9 |   0 |   : |   - | OUT |     |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R2 |   > |   \ |     | (2) |     |     |     |     |
        *     |   . |   L |   O |   ^ |  CR |     |     |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R3 |     |     |     |     |     |     |     |     |
        *     |   W |   E |   R |   T |   Y |   U |   I |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R4 |     |     |     |     |     |  LF |   [ |     |
        *     |   S |   D |   F |   G |   H |   J |   K |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R5 |     | ETX |     |     |     |   ] |   < |     |
        *     |   X |   C |   V |   B |   N |   M |   , |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R6 |     |     |     |     |   ? |   + |   @ |     |
        *     |   Q |   A |   Z |space|   / |   ; |   P |     |
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  R7 | (3) |     | (4) |     |     | left|right|SHIFT|
        *     |     | CTRL|     |     |     |SHIFT|SHIFT| LOCK|
        *     +-----+-----+-----+-----+-----+-----+-----+-----+
        *  
        *  (1) Both MONUK02 and CEGMON decode shift-0 as @
        *  
        * Notes for Ohio Superboard II keyboard:
        *  (2) This key is labelled LINE FEED
        *  (3) This position is the REPEAT key
        *  (4) This position is the ESC key
        */

        public struct Key
        {
            Keys _keycode;
            byte _row;
            byte _column;
            bool _shift;

            public Key(Keys keycode, byte row, byte column, bool shift)
            {
                _keycode = keycode;
                _row = row;
                _column = column;
                _shift = shift;
            }

            public Keys KeyCode
            {
                get
                {
                    return (_keycode);
                }
            }

            public byte Row
            {
                get
                {
                    return (_row);
                }
            }

            public byte Column
            {
                get
                {
                    return (_column);
                }
            }

            public bool Shift
            {
                get
                {
                    return (_shift);
                }
            }

        }

        private Dictionary<Keys, Key> _normal;
        private Dictionary<Keys, Key> _shift;   // Fix issues with key mapping

        public KeyboardMatrix()
        {
            _normal = new Dictionary<Keys, Key>();
            _shift = new Dictionary<Keys, Key>();

            _normal.Add(Keys.D1, new Key(Keys.D1, 0, 0, false));                // 1
            _normal.Add(Keys.D2, new Key(Keys.D2, 0, 1, false));                // 2
            _normal.Add(Keys.D3, new Key(Keys.D3, 0, 2, false));                // 3
            _normal.Add(Keys.D4, new Key(Keys.D4, 0, 3, false));                // 4
            _normal.Add(Keys.D5, new Key(Keys.D5, 0, 4, false));                // 5
            _normal.Add(Keys.D6, new Key(Keys.D6, 0, 5, false));                // 6
            _normal.Add(Keys.D7, new Key(Keys.D7, 0, 6, false));                // 7

            _normal.Add(Keys.D8, new Key(Keys.D8, 1, 0, false));                // 8
            _normal.Add(Keys.D9, new Key(Keys.D9, 1, 1, false));                // 9
            _normal.Add(Keys.D0, new Key(Keys.D0, 1, 2, false));                // 0

            _normal.Add(Keys.OemMinus, new Key(Keys.OemMinus, 1, 4, false));    // -
            _normal.Add(Keys.Back, new Key(Keys.Delete, 1, 5, false));          // RUB OUT

            _normal.Add(Keys.OemPeriod, new Key(Keys.OemPeriod, 2, 0, false));  // .
            _normal.Add(Keys.L, new Key(Keys.L, 2, 1, false));                  // l
            _normal.Add(Keys.O, new Key(Keys.O, 2, 2, false));                  // o
            _normal.Add(Keys.Up, new Key(Keys.Up, 2, 3, false));                // ^
            _normal.Add(Keys.Enter, new Key(Keys.Enter, 2, 4, false));          // CR

            _normal.Add(Keys.W, new Key(Keys.W, 3, 0, false));                  // w
            _normal.Add(Keys.E, new Key(Keys.E, 3, 1, false));                  // e
            _normal.Add(Keys.R, new Key(Keys.R, 3, 2, false));                  // r
            _normal.Add(Keys.T, new Key(Keys.T, 3, 3, false));                  // t
            _normal.Add(Keys.Y, new Key(Keys.Y, 3, 4, false));                  // y
            _normal.Add(Keys.U, new Key(Keys.U, 3, 5, false));                  // u
            _normal.Add(Keys.I, new Key(Keys.I, 3, 6, false));                  // i

            _normal.Add(Keys.S, new Key(Keys.S, 4, 0, false));                  // s
            _normal.Add(Keys.D, new Key(Keys.D, 4, 1, false));                  // d
            _normal.Add(Keys.F, new Key(Keys.F, 4, 2, false));                  // f
            _normal.Add(Keys.G, new Key(Keys.G, 4, 3, false));                  // g
            _normal.Add(Keys.H, new Key(Keys.H, 4, 4, false));                  // h
            _normal.Add(Keys.J, new Key(Keys.J, 4, 5, false));                  // j
            _normal.Add(Keys.K, new Key(Keys.K, 4, 6, false));                  // k

            _normal.Add(Keys.X, new Key(Keys.X, 5, 0, false));                  // x
            _normal.Add(Keys.C, new Key(Keys.C, 5, 1, false));                  // c
            _normal.Add(Keys.V, new Key(Keys.V, 5, 2, false));                  // v
            _normal.Add(Keys.B, new Key(Keys.B, 5, 3, false));                  // b
            _normal.Add(Keys.N, new Key(Keys.N, 5, 4, false));                  // n
            _normal.Add(Keys.M, new Key(Keys.M, 5, 5, false));                  // m
            _normal.Add(Keys.Oemcomma, new Key(Keys.Oemcomma, 5, 6, false));    // ,

            _normal.Add(Keys.Q, new Key(Keys.Q, 6, 0, false));                  // q
            _normal.Add(Keys.A, new Key(Keys.A, 6, 1, false));                  // a
            _normal.Add(Keys.Z, new Key(Keys.Z, 6, 2, false));                  // z
            _normal.Add(Keys.Space, new Key(Keys.Space, 6, 3, false));          // space
            _normal.Add(Keys.Oem2, new Key(Keys.Oem2, 6, 4, false));            // /
            _normal.Add(Keys.Oem1, new Key(Keys.Oem1, 6, 5, false));            // ;
            _normal.Add(Keys.P, new Key(Keys.P, 6, 6, false));                  // p

            _normal.Add(Keys.ControlKey, new Key(Keys.ControlKey, 7, 1, false));  // CTRL
            _normal.Add(Keys.LShiftKey, new Key(Keys.LShiftKey, 7, 5, false));    // Left SHIFT
            _normal.Add(Keys.RShiftKey, new Key(Keys.RShiftKey, 7, 6, false));    // Right SHIFT
            _normal.Add(Keys.Capital, new Key(Keys.Capital, 7, 7, false));        // SHIFT-LOCK

            // Special keys

            _normal.Add(Keys.Oem7, new Key(Keys.Oem7, 0, 2, true));                 // #
            _normal.Add(Keys.Oemplus, new Key(Keys.Oemplus, 1, 4, true));           // =
            _normal.Add(Keys.Oem6, new Key(Keys.Oem6, 5, 5, true));                 // ]
            _normal.Add(Keys.Oem3, new Key(Keys.Oem3, 0, 6, true));                 // '
            _normal.Add(Keys.Oem5, new Key(Keys.Oem5, 2, 1, true));                 // \
            _normal.Add(Keys.Oem8, new Key(Keys.Oem8, 4, 5, false));                // unknown

            // Map keyboard codes over to UK101 keyboard

            _shift.Add(Keys.D1, new Key(Keys.D1, 0, 0, true));                      // !
            _shift.Add(Keys.D2, new Key(Keys.D2, 0, 1, true));                      // "
            _shift.Add(Keys.D3, new Key(Keys.D3, 0, 2, true));                      // #
            _shift.Add(Keys.D4, new Key(Keys.D4, 0, 3, true));                      // $
            _shift.Add(Keys.D5, new Key(Keys.D5, 0, 4, true));                      // %
            // missing D%
            _shift.Add(Keys.D7, new Key(Keys.D7, 0, 5, true));                      // &

            _shift.Add(Keys.D8, new Key(Keys.D8, 1, 3, true));                      // *
            _shift.Add(Keys.D9, new Key(Keys.D9, 1, 0, true));                      // (
            _shift.Add(Keys.D0, new Key(Keys.D0, 1, 1, true));                      // )


            _shift.Add(Keys.Back, new Key(Keys.Delete, 1, 5, false));               // RUB OUT

            _shift.Add(Keys.OemPeriod, new Key(Keys.OemPeriod, 2, 0, true));        // >
            _shift.Add(Keys.L, new Key(Keys.L, 2, 1, true));                        // L
            _shift.Add(Keys.O, new Key(Keys.O, 2, 2, true));                        // O
            //_shift.Add(Keys.Up, new Key(Keys.Up, 2, 3, true));                    // ^
            _shift.Add(Keys.Enter, new Key(Keys.Enter, 2, 4, false));               // CR

            _shift.Add(Keys.W, new Key(Keys.W, 3, 0, true));                        // W
            _shift.Add(Keys.E, new Key(Keys.E, 3, 1, true));                        // E
            _shift.Add(Keys.R, new Key(Keys.R, 3, 2, true));                        // R
            _shift.Add(Keys.T, new Key(Keys.T, 3, 3, true));                        // T
            _shift.Add(Keys.Y, new Key(Keys.Y, 3, 4, true));                        // Y
            _shift.Add(Keys.U, new Key(Keys.U, 3, 5, true));                        // U
            _shift.Add(Keys.I, new Key(Keys.I, 3, 6, true));                        // I

            _shift.Add(Keys.S, new Key(Keys.S, 4, 0, true));                        // S
            _shift.Add(Keys.D, new Key(Keys.D, 4, 1, true));                        // D
            _shift.Add(Keys.F, new Key(Keys.F, 4, 2, true));                        // F
            _shift.Add(Keys.G, new Key(Keys.G, 4, 3, true));                        // G
            _shift.Add(Keys.H, new Key(Keys.H, 4, 4, true));                        // H
            _shift.Add(Keys.J, new Key(Keys.J, 4, 5, true));                        // J
            _shift.Add(Keys.K, new Key(Keys.K, 4, 6, true));                        // K

            _shift.Add(Keys.X, new Key(Keys.X, 5, 0, true));                        // X
            _shift.Add(Keys.C, new Key(Keys.C, 5, 1, true));                        // C
            _shift.Add(Keys.V, new Key(Keys.V, 5, 2, true));                        // V
            _shift.Add(Keys.B, new Key(Keys.B, 5, 3, true));                        // B
            _shift.Add(Keys.N, new Key(Keys.N, 5, 4, true));                        // N
            _shift.Add(Keys.M, new Key(Keys.M, 5, 5, true));                        // M
            _shift.Add(Keys.Oemcomma, new Key(Keys.Oemcomma, 5, 6, true));          // <

            _shift.Add(Keys.Q, new Key(Keys.Q, 6, 0, true));                        // Q
            _shift.Add(Keys.A, new Key(Keys.A, 6, 1, true));                        // A
            _shift.Add(Keys.Z, new Key(Keys.Z, 6, 2, true));                        // Z
            _shift.Add(Keys.Space, new Key(Keys.Space, 6, 3, false));               // space
            _shift.Add(Keys.OemQuestion, new Key(Keys.OemQuestion, 6, 4, true));    // ?
            _shift.Add(Keys.Oemplus, new Key(Keys.Oemplus, 6, 5, true));            // +
            _shift.Add(Keys.P, new Key(Keys.P, 6, 6, true));                        // P
            

            _shift.Add(Keys.ControlKey, new Key(Keys.ControlKey, 7, 1, false));     // CTRL
            _shift.Add(Keys.LShiftKey, new Key(Keys.LShiftKey, 7, 5, false));       // Left SHIFT
            _shift.Add(Keys.RShiftKey, new Key(Keys.RShiftKey, 7, 6, false));       // Right SHIFT
            _shift.Add(Keys.Capital, new Key(Keys.Capital, 7, 7, false));           // SHIFT-LOCK

            // Special keys


            _shift.Add(Keys.Oem1, new Key(Keys.Oem1, 1, 3, false));                 // :
            _shift.Add(Keys.OemMinus, new Key(Keys.OemMinus, 1, 4, false));         // -
            _shift.Add(Keys.D6, new Key(Keys.D6, 5, 4, true));                      // ^
            _shift.Add(Keys.Oem3, new Key(Keys.Oem3, 6, 6, true));                  // @

            _shift.Add(Keys.Oem8, new Key(Keys.Oem8, 4, 5, true));                  // unknown
        }

        public Key GetKey(Keys keycode, bool shift)
        {
            Key key = new Key(Keys.NoName, 0, 7,false);
            try
            {
                if (shift == true)
                {
                    key = _shift[keycode];
                }
                else
                {
                    key = _normal[keycode];
                }
            }
            catch { }
            return (key);
        }

    }
}
