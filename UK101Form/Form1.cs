using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UK101Library;

namespace UK101Form
{
    public partial class Form1 : Form
    {
        CSignetic6502 CSignetic6502;

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
            CSignetic6502.MemoryBus.VDU.NumberOfLines = 32;
            CSignetic6502.Reset();

            CClock.Hold = false;
            CClock.Start();

            this.KeyPreview = true;

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
            Keys key = e.KeyData;
            Keys modifiers = e.Modifiers;

            if (key == Keys.Escape)
            {
                CSignetic6502.Reset();
            }
            else if (key == Keys.F1)
            {
                CSignetic6502.MemoryBus.ACIA.Mode = CACIA.IO_MODE_6820_TAPE;
            }
            else if (key == Keys.F2)
            {
                CSignetic6502.MemoryBus.ACIA.Lines = CSignetic6502.MemoryBus.ACIA.basicProg.Gunfight;
                CSignetic6502.MemoryBus.ACIA.line = 0;
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

                // Scan codes:
                // Shift left  = 0x2a  send to UK101 as 0x12
                // Shift right = 0x36  send to UK101 as 0x10
                // Caps lock   = 0x3a  send to UK101 as 0x14
                // Ctrl left   = 0x1d  send to UK101 as 0x11
                // Ctrl right  = 0x1d  send to UK101 as 0x11
                // Arrow up    = 0x48  send to UK101 as 0xba
                // Enter       = 0x1c  send to UK101 as 0x0d

                Debug.Print("KeyCode " + keyCode.ToString());
                Debug.Print("Modifiers " + modifiers.ToString());

                if ((modifiers == Keys.Shift) && (keyCode != Keys.ShiftKey))
                {
                    CSignetic6502.MemoryBus.Keyboard.PressKey(0x12);
                    if (keyCode == Keys.D1)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x31); // 1
                    }
                    else if (keyCode == Keys.D2)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x32); // 2
                    }
                    else if (keyCode == Keys.D3)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x33); // 3
                    }
                    else if (keyCode == Keys.D4)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x34); // 4
                    }
                    else if (keyCode == Keys.D5)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x35); // 5
                    }
                    else if (keyCode == Keys.D6)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0xba); // 6
                    }
                    else if (keyCode == Keys.D7)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x36); // 7
                    }
                    else if (key == Keys.D8)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x37); // 8
                    }
                    else if (keyCode == Keys.D9)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x38); // 9
                    }
                    else if (keyCode == Keys.D0)
                    {
                        CSignetic6502.MemoryBus.Keyboard.PressKey(0x39); // 0
                    }

                }
                else
                {
                    CSignetic6502.MemoryBus.Keyboard.PressKey((byte)keyCode);
                }

            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys key = e.KeyData;
            Keys modifiers = e.Modifiers;

            if (Form1.IsKeyLocked(Keys.CapsLock) == true)
            {
                CSignetic6502.MemoryBus.Keyboard.Keystates[7] &= 0xfe;
            }
            else
            {
                CSignetic6502.MemoryBus.Keyboard.Keystates[7] |= 0x01;
            }

            if ((modifiers == Keys.Shift) && (keyCode != Keys.ShiftKey))
            {
                CSignetic6502.MemoryBus.Keyboard.ReleaseKey((byte)0x12);
                if (keyCode == Keys.D1)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x31); // 1
                }
                else if (keyCode == Keys.D2)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x32); // 2
                }
                else if (keyCode == Keys.D3)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x33); // 3
                }
                else if (keyCode == Keys.D4)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x34); // 4
                }
                else if (keyCode == Keys.D5)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x35); // 5
                }
                else if (keyCode == Keys.D6)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0xba); // 6
                }
                else if (keyCode == Keys.D7)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x36); // 7
                }
                else if (keyCode == Keys.D8)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x37); // 8
                }
                else if (keyCode == Keys.D9)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x38); // 9
                }
                else if (keyCode == Keys.D0)
                {
                    CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x39); // 0
                }
            }
            else
            {
                CSignetic6502.MemoryBus.Keyboard.ReleaseKey((byte)keyCode);
            }
        }
    }
}

