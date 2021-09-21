using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UK101Library;
using static UK101Form.KeyboardMatrix;

namespace UK101Form
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);

        CSignetic6502 CSignetic6502;
        KeyboardMatrix KeyboardMatrix;
        byte _row = 2;
        byte _column = 3;

        public Form1()
        {
            InitializeComponent();

            MainPage mainPage = new MainPage();

            mainPage.pictureBox = pictureBox1; // Store the picture box
            pictureBox1.Width = 88 * 8;
            pictureBox1.Height = 72 * 8;
            pictureBox1.Select();

            CSignetic6502 = new CSignetic6502(mainPage);
            mainPage.CSignetic6502 = CSignetic6502; // Store the core processor dont like this, might just inject the processor

            CClock CClock = new CClock(mainPage);
            CSignetic6502.MemoryBus.VDU.InitCVDU(mainPage);
            CSignetic6502.MemoryBus.VDU.NumberOfLines = 16;
            CSignetic6502.Reset();

            CClock.Hold = false;
            CClock.Start();

            this.KeyPreview = true;
            KeyboardMatrix = new KeyboardMatrix();

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Bitmap b = CSignetic6502.MemoryBus.VDU.Generate();
            g.DrawImageUnscaled(b, 0, 0);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys modifiers = e.Modifiers;
            int keyValue = e.KeyValue;

            if (keyCode == Keys.Escape)
            {
                CSignetic6502.Reset();
            }
            else if (keyCode == Keys.F1)
            {
                CSignetic6502.MemoryBus.ACIA.Mode = CACIA.IO_MODE_6820_TAPE;
            }
            else if (keyCode == Keys.F2)
            {
                CSignetic6502.MemoryBus.ACIA.Lines = CSignetic6502.MemoryBus.ACIA.basicProg.Gunfight;
                CSignetic6502.MemoryBus.ACIA.line = 0;
            }
            else if (keyCode == Keys.F3)
            {
                CSignetic6502.MemoryBus.Keyboard.PressKey(7, 7);
                CSignetic6502.MemoryBus.Keyboard.PressKey(_row, _column);
            }
            else
            {
                if (Form1.IsKeyLocked(Keys.CapsLock) == true)
                {
                    CSignetic6502.MemoryBus.Keyboard.Keystates[7] &= 0xfe;  // 1111 1110
                }
                else
                {
                    CSignetic6502.MemoryBus.Keyboard.Keystates[7] |= 0x01;  // 0000 0001
                }

                // Scan codes:
                // Shift left  = 0x2a  send to UK101 as 0x12
                // Shift right = 0x36  send to UK101 as 0x10
                // Caps lock   = 0x3a  send to UK101 as 0x14
                // Ctrl left   = 0x1d  send to UK101 as 0x11
                // Ctrl right  = 0x1d  send to UK101 as 0x11
                // Arrow up    = 0x48  send to UK101 as 0xba
                // Enter       = 0x1c  send to UK101 as 0x0d

                Debug.Print("Down KeyCode " + keyCode.ToString());
                Debug.Print("Down Modifiers " + modifiers.ToString());
                Debug.Print("Down Key " + keyValue.ToString());

                Key key;
                if (modifiers == Keys.Shift)
                {
                    if (keyCode == Keys.ShiftKey)
                    {
                        //if (Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)))
                        //{
                        //    key = KeyboardMatrix.GetKey(Keys.LShiftKey);
                        //    CSignetic6502.MemoryBus.Keyboard.PressKey(key.Row, key.Column);
                        //}
                        //else if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
                        //{
                        //    key = KeyboardMatrix.GetKey(Keys.RShiftKey);
                        //    CSignetic6502.MemoryBus.Keyboard.PressKey(key.Row, key.Column);
                        //}
                    }
                    else
                    {
                        Debug.Print("lshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)));
                        Debug.Print("rshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)));

                        key = KeyboardMatrix.GetKey(keyCode, true);
                        if (key.KeyCode != Keys.NoName)
                        {
                            Debug.Print("Apply shift");
                            if (key.Shift == true)
                            {
                                key = KeyboardMatrix.GetKey(Keys.LShiftKey, false);
                                CSignetic6502.MemoryBus.Keyboard.PressKey(key.Row, key.Column);
                            }
                            key = KeyboardMatrix.GetKey(keyCode, true);
                            CSignetic6502.MemoryBus.Keyboard.PressKey(key.Row, key.Column);
                        }
                    }
                }
                else
                {
                    key = KeyboardMatrix.GetKey(keyCode, false);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            Debug.Print("Apply shift");
                            key = KeyboardMatrix.GetKey(Keys.LShiftKey, false);
                            CSignetic6502.MemoryBus.Keyboard.PressKey(key.Row, key.Column);
                        }
                        key = KeyboardMatrix.GetKey(keyCode, false);
                        CSignetic6502.MemoryBus.Keyboard.PressKey(key.Row, key.Column);
                    }
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys modifiers = e.Modifiers;
            int keyValue = e.KeyValue;

            if (keyCode == Keys.F3)
            {
                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(7, 7);
                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(_row, _column);
                //_column++;
                //if (_column > 6)
                //{
                //    _column = 0;
                //    _row++;
                //}
            }
            else
            {
                if (Form1.IsKeyLocked(Keys.CapsLock) == true)
                {
                    CSignetic6502.MemoryBus.Keyboard.Keystates[7] &= 0xfe;
                }
                else
                {
                    CSignetic6502.MemoryBus.Keyboard.Keystates[7] |= 0x01;
                }

                Debug.Print("Up KeyCode " + keyCode.ToString());
                Debug.Print("Up Modifiers " + modifiers.ToString());
                Debug.Print("Up Key " + keyValue.ToString());

                Key key;


                if (modifiers == Keys.Shift)
                {
                    if (keyCode == Keys.ShiftKey)
                    {
                        //if (Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)))
                        //{
                        //    key = KeyboardMatrix.GetKey(Keys.LShiftKey);
                        //    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(key.Row, key.Column);
                        //}
                        //else if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
                        //{
                        //    key = KeyboardMatrix.GetKey(Keys.RShiftKey);
                        //    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(key.Row, key.Column);
                        //}
                    }
                    else
                    {
                        Debug.Print("lshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)));
                        Debug.Print("rshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)));

                        key = KeyboardMatrix.GetKey(keyCode, true);
                        if (key.KeyCode != Keys.NoName)
                        {
                            if (key.Shift == true)
                            {
                                Debug.Print("Apply shift");
                                key = KeyboardMatrix.GetKey(Keys.LShiftKey, false);
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(key.Row, key.Column);
                            }
                            key = KeyboardMatrix.GetKey(keyCode, true);
                            CSignetic6502.MemoryBus.Keyboard.ReleaseKey(key.Row, key.Column);
                        }
                    }
                }
                else
                {
                    key = KeyboardMatrix.GetKey(keyCode, false);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            Debug.Print("Apply shift");
                            key = KeyboardMatrix.GetKey(Keys.LShiftKey, false);
                            CSignetic6502.MemoryBus.Keyboard.ReleaseKey(key.Row, key.Column);
                        }
                        key = KeyboardMatrix.GetKey(keyCode, false);
                        CSignetic6502.MemoryBus.Keyboard.ReleaseKey(key.Row, key.Column);
                    }
                }
            }
        }
    }
}

