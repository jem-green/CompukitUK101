using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UK101Console
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
            ConsoleKey _keycode;
            byte _row;
            byte _column;
            bool _shift;

            public Key(ConsoleKey keycode, byte row, byte column, bool shift)
            {
                _keycode = keycode;
                _row = row;
                _column = column;
                _shift = shift;
            }

            public ConsoleKey KeyCode
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

        private Dictionary<ConsoleKey, Key> _normal;
        private Dictionary<ConsoleKey, Key> _shift;   // Fix issues with key mapping

        public KeyboardMatrix()
        {
            _normal = new Dictionary<ConsoleKey, Key>();
            _shift = new Dictionary<ConsoleKey, Key>();

            _normal.Add(ConsoleKey.D1, new Key(ConsoleKey.D1, 0, 0, false));                // 1
            _normal.Add(ConsoleKey.D2, new Key(ConsoleKey.D2, 0, 1, false));                // 2
            _normal.Add(ConsoleKey.D3, new Key(ConsoleKey.D3, 0, 2, false));                // 3
            _normal.Add(ConsoleKey.D4, new Key(ConsoleKey.D4, 0, 3, false));                // 4
            _normal.Add(ConsoleKey.D5, new Key(ConsoleKey.D5, 0, 4, false));                // 5
            _normal.Add(ConsoleKey.D6, new Key(ConsoleKey.D6, 0, 5, false));                // 6
            _normal.Add(ConsoleKey.D7, new Key(ConsoleKey.D7, 0, 6, false));                // 7

            _normal.Add(ConsoleKey.D8, new Key(ConsoleKey.D8, 1, 0, false));                // 8
            _normal.Add(ConsoleKey.D9, new Key(ConsoleKey.D9, 1, 1, false));                // 9
            _normal.Add(ConsoleKey.D0, new Key(ConsoleKey.D0, 1, 2, false));                // 0

            _normal.Add(ConsoleKey.OemMinus, new Key(ConsoleKey.OemMinus, 1, 4, false));    // -
            _normal.Add(ConsoleKey.Back, new Key(ConsoleKey.Delete, 1, 5, false));          // RUB OUT

            _normal.Add(ConsoleKey.OemPeriod, new Key(ConsoleKey.OemPeriod, 2, 0, false));  // .
            _normal.Add(ConsoleKey.L, new Key(ConsoleKey.L, 2, 1, false));                  // l
            _normal.Add(ConsoleKey.O, new Key(ConsoleKey.O, 2, 2, false));                  // o
            _normal.Add(ConsoleKey.Up, new Key(ConsoleKey.Up, 2, 3, false));                // ^
            _normal.Add(ConsoleKey.Enter, new Key(ConsoleKey.Enter, 2, 4, false));          // CR

            _normal.Add(ConsoleKey.W, new Key(ConsoleKey.W, 3, 0, false));                  // w
            _normal.Add(ConsoleKey.E, new Key(ConsoleKey.E, 3, 1, false));                  // e
            _normal.Add(ConsoleKey.R, new Key(ConsoleKey.R, 3, 2, false));                  // r
            _normal.Add(ConsoleKey.T, new Key(ConsoleKey.T, 3, 3, false));                  // t
            _normal.Add(ConsoleKey.Y, new Key(ConsoleKey.Y, 3, 4, false));                  // y
            _normal.Add(ConsoleKey.U, new Key(ConsoleKey.U, 3, 5, false));                  // u
            _normal.Add(ConsoleKey.I, new Key(ConsoleKey.I, 3, 6, false));                  // i

            _normal.Add(ConsoleKey.S, new Key(ConsoleKey.S, 4, 0, false));                  // s
            _normal.Add(ConsoleKey.D, new Key(ConsoleKey.D, 4, 1, false));                  // d
            _normal.Add(ConsoleKey.F, new Key(ConsoleKey.F, 4, 2, false));                  // f
            _normal.Add(ConsoleKey.G, new Key(ConsoleKey.G, 4, 3, false));                  // g
            _normal.Add(ConsoleKey.H, new Key(ConsoleKey.H, 4, 4, false));                  // h
            _normal.Add(ConsoleKey.J, new Key(ConsoleKey.J, 4, 5, false));                  // j
            _normal.Add(ConsoleKey.K, new Key(ConsoleKey.K, 4, 6, false));                  // k

            _normal.Add(ConsoleKey.X, new Key(ConsoleKey.X, 5, 0, false));                  // x
            _normal.Add(ConsoleKey.C, new Key(ConsoleKey.C, 5, 1, false));                  // c
            _normal.Add(ConsoleKey.V, new Key(ConsoleKey.V, 5, 2, false));                  // v
            _normal.Add(ConsoleKey.B, new Key(ConsoleKey.B, 5, 3, false));                  // b
            _normal.Add(ConsoleKey.N, new Key(ConsoleKey.N, 5, 4, false));                  // n
            _normal.Add(ConsoleKey.M, new Key(ConsoleKey.M, 5, 5, false));                  // m
            _normal.Add(ConsoleKey.Oemcomma, new Key(ConsoleKey.Oemcomma, 5, 6, false));    // ,

            _normal.Add(ConsoleKey.Q, new Key(ConsoleKey.Q, 6, 0, false));                  // q
            _normal.Add(ConsoleKey.A, new Key(ConsoleKey.A, 6, 1, false));                  // a
            _normal.Add(ConsoleKey.Z, new Key(ConsoleKey.Z, 6, 2, false));                  // z
            _normal.Add(ConsoleKey.Space, new Key(ConsoleKey.Space, 6, 3, false));          // space
            _normal.Add(ConsoleKey.Oem2, new Key(ConsoleKey.Oem2, 6, 4, false));            // /
            _normal.Add(ConsoleKey.Oem1, new Key(ConsoleKey.Oem1, 6, 5, false));            // ;
            _normal.Add(ConsoleKey.P, new Key(ConsoleKey.P, 6, 6, false));                  // p

            _normal.Add(ConsoleKey.ControlKey, new Key(ConsoleKey.ControlKey, 7, 1, false));  // CTRL
            _normal.Add(ConsoleKey.LShiftKey, new Key(ConsoleKey.LShiftKey, 7, 5, false));    // Left SHIFT
            _normal.Add(ConsoleKey.RShiftKey, new Key(ConsoleKey.RShiftKey, 7, 6, false));    // Right SHIFT
            _normal.Add(ConsoleKey.Capital, new Key(ConsoleKey.Capital, 7, 7, false));        // SHIFT-LOCK

            // Special ConsoleKey

            _normal.Add(ConsoleKey.Oem7, new Key(ConsoleKey.Oem7, 0, 2, true));                 // #
            _normal.Add(ConsoleKey.Oemplus, new Key(ConsoleKey.Oemplus, 1, 4, true));           // =
            _normal.Add(ConsoleKey.Oem6, new Key(ConsoleKey.Oem6, 5, 5, true));                 // ]
            _normal.Add(ConsoleKey.Oem3, new Key(ConsoleKey.Oem3, 0, 6, true));                 // '
            _normal.Add(ConsoleKey.Oem5, new Key(ConsoleKey.Oem5, 2, 1, true));                 // \
            _normal.Add(ConsoleKey.Oem8, new Key(ConsoleKey.Oem8, 4, 5, false));                // unknown

            // Map keyboard codes over to UK101 keyboard

            _shift.Add(ConsoleKey.D1, new Key(ConsoleKey.D1, 0, 0, true));                      // !
            _shift.Add(ConsoleKey.D2, new Key(ConsoleKey.D2, 0, 1, true));                      // "
            _shift.Add(ConsoleKey.D3, new Key(ConsoleKey.D3, 0, 2, true));                      // #
            _shift.Add(ConsoleKey.D4, new Key(ConsoleKey.D4, 0, 3, true));                      // $
            _shift.Add(ConsoleKey.D5, new Key(ConsoleKey.D5, 0, 4, true));                      // %
            // missing D%
            _shift.Add(ConsoleKey.D7, new Key(ConsoleKey.D7, 0, 5, true));                      // &

            _shift.Add(ConsoleKey.D8, new Key(ConsoleKey.D8, 1, 3, true));                      // *
            _shift.Add(ConsoleKey.D9, new Key(ConsoleKey.D9, 1, 0, true));                      // (
            _shift.Add(ConsoleKey.D0, new Key(ConsoleKey.D0, 1, 1, true));                      // )


            _shift.Add(ConsoleKey.Back, new Key(ConsoleKey.Delete, 1, 5, false));               // RUB OUT

            _shift.Add(ConsoleKey.OemPeriod, new Key(ConsoleKey.OemPeriod, 2, 0, true));        // >
            _shift.Add(ConsoleKey.L, new Key(ConsoleKey.L, 2, 1, true));                        // L
            _shift.Add(ConsoleKey.O, new Key(ConsoleKey.O, 2, 2, true));                        // O
            //_shift.Add(ConsoleKey.Up, new Key(ConsoleKey.Up, 2, 3, true));                    // ^
            _shift.Add(ConsoleKey.Enter, new Key(ConsoleKey.Enter, 2, 4, false));               // CR

            _shift.Add(ConsoleKey.W, new Key(ConsoleKey.W, 3, 0, true));                        // W
            _shift.Add(ConsoleKey.E, new Key(ConsoleKey.E, 3, 1, true));                        // E
            _shift.Add(ConsoleKey.R, new Key(ConsoleKey.R, 3, 2, true));                        // R
            _shift.Add(ConsoleKey.T, new Key(ConsoleKey.T, 3, 3, true));                        // T
            _shift.Add(ConsoleKey.Y, new Key(ConsoleKey.Y, 3, 4, true));                        // Y
            _shift.Add(ConsoleKey.U, new Key(ConsoleKey.U, 3, 5, true));                        // U
            _shift.Add(ConsoleKey.I, new Key(ConsoleKey.I, 3, 6, true));                        // I

            _shift.Add(ConsoleKey.S, new Key(ConsoleKey.S, 4, 0, true));                        // S
            _shift.Add(ConsoleKey.D, new Key(ConsoleKey.D, 4, 1, true));                        // D
            _shift.Add(ConsoleKey.F, new Key(ConsoleKey.F, 4, 2, true));                        // F
            _shift.Add(ConsoleKey.G, new Key(ConsoleKey.G, 4, 3, true));                        // G
            _shift.Add(ConsoleKey.H, new Key(ConsoleKey.H, 4, 4, true));                        // H
            _shift.Add(ConsoleKey.J, new Key(ConsoleKey.J, 4, 5, true));                        // J
            _shift.Add(ConsoleKey.K, new Key(ConsoleKey.K, 4, 6, true));                        // K

            _shift.Add(ConsoleKey.X, new Key(ConsoleKey.X, 5, 0, true));                        // X
            _shift.Add(ConsoleKey.C, new Key(ConsoleKey.C, 5, 1, true));                        // C
            _shift.Add(ConsoleKey.V, new Key(ConsoleKey.V, 5, 2, true));                        // V
            _shift.Add(ConsoleKey.B, new Key(ConsoleKey.B, 5, 3, true));                        // B
            _shift.Add(ConsoleKey.N, new Key(ConsoleKey.N, 5, 4, true));                        // N
            _shift.Add(ConsoleKey.M, new Key(ConsoleKey.M, 5, 5, true));                        // M
            _shift.Add(ConsoleKey.Oemcomma, new Key(ConsoleKey.Oemcomma, 5, 6, true));          // <

            _shift.Add(ConsoleKey.Q, new Key(ConsoleKey.Q, 6, 0, true));                        // Q
            _shift.Add(ConsoleKey.A, new Key(ConsoleKey.A, 6, 1, true));                        // A
            _shift.Add(ConsoleKey.Z, new Key(ConsoleKey.Z, 6, 2, true));                        // Z
            _shift.Add(ConsoleKey.Space, new Key(ConsoleKey.Space, 6, 3, false));               // space
            _shift.Add(ConsoleKey.OemQuestion, new Key(ConsoleKey.OemQuestion, 6, 4, true));    // ?
            _shift.Add(ConsoleKey.Oemplus, new Key(ConsoleKey.Oemplus, 6, 5, true));            // +
            _shift.Add(ConsoleKey.P, new Key(ConsoleKey.P, 6, 6, true));                        // P
            

            _shift.Add(ConsoleKey.ControlKey, new Key(ConsoleKey.ControlKey, 7, 1, false));     // CTRL
            _shift.Add(ConsoleKey.LShiftKey, new Key(ConsoleKey.LShiftKey, 7, 5, false));       // Left SHIFT
            _shift.Add(ConsoleKey.RShiftKey, new Key(ConsoleKey.RShiftKey, 7, 6, false));       // Right SHIFT
            _shift.Add(ConsoleKey.Capital, new Key(ConsoleKey.Capital, 7, 7, false));           // SHIFT-LOCK

            // Special ConsoleKey


            _shift.Add(ConsoleKey.Oem1, new Key(ConsoleKey.Oem1, 1, 3, false));                 // :
            _shift.Add(ConsoleKey.OemMinus, new Key(ConsoleKey.OemMinus, 1, 4, false));         // -
            _shift.Add(ConsoleKey.D6, new Key(ConsoleKey.D6, 5, 4, true));                      // ^
            _shift.Add(ConsoleKey.Oem3, new Key(ConsoleKey.Oem3, 6, 6, true));                  // @

            _shift.Add(ConsoleKey.Oem8, new Key(ConsoleKey.Oem8, 4, 5, true));                  // unknown
        }

        public Key GetKey(ConsoleKey keycode, bool shift)
        {
            Key key = new Key(ConsoleKey.NoName, 0, 7,false);
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
