using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using UK101Library;

namespace UK101Console
{
    class Program
    {
        CSignetic6502 CSignetic6502;
        KeyboardMatrix KeyboardMatrix;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            MainPage mainPage = new MainPage();

            CSignetic6502 CSignetic6502 = new CSignetic6502(mainPage);
            KeyboardMatrix KeyboardMatrix;

            mainPage.CSignetic6502 = CSignetic6502; // Store the core processor dont like this, might just inject the processor
            CClock CClock = new CClock(mainPage);
            CSignetic6502.MemoryBus.VDU.InitCVDU(mainPage);
            CSignetic6502.MemoryBus.VDU.NumberOfLines = 16;
            CSignetic6502.Reset();

            CClock.Hold = false;
            CClock.Start();

            ConsoleKeyInfo key = new ConsoleKeyInfo((char)0, ConsoleKey.NoName, false, false, false);

            while (1 == 1)
            {
                if (Console.KeyAvailable == true)
                {
                    key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        CSignetic6502.Reset();
                    }
                    else if (key.Key == ConsoleKey.F1)
                    {
                        CSignetic6502.MemoryBus.ACIA.Mode = CACIA.IO_MODE_6820_TAPE;
                    }
                    else if (key.Key == ConsoleKey.F2)
                    {
                        CSignetic6502.MemoryBus.ACIA.Lines = CSignetic6502.MemoryBus.ACIA.basicProg.Maze;
                        CSignetic6502.MemoryBus.ACIA.line = 0;
                    }
                    else
                    {
                        if (Console.CapsLock == true)
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

                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                        {
                            CSignetic6502.MemoryBus.Keyboard.PressKey((byte)0x12);
                            if (key.Key == ConsoleKey.D1)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x31); // 1
                            }
                            else if (key.Key == ConsoleKey.D2)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x32); // 2
                            }
                            else if (key.Key == ConsoleKey.D3)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x33); // 3
                            }
                            else if (key.Key == ConsoleKey.D4)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x34); // 4
                            }
                            else if (key.Key == ConsoleKey.D5)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x35); // 5
                            }
                            else if (key.Key == ConsoleKey.D6)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0xba); // 6
                            }
                            else if (key.Key == ConsoleKey.D7)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x36); // 7
                            }
                            else if (key.Key == ConsoleKey.D8)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x37); // 8
                            }
                            else if (key.Key == ConsoleKey.D9)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x38); // 9
                            }
                            else if (key.Key == ConsoleKey.D0)
                            {
                                CSignetic6502.MemoryBus.Keyboard.PressKey(0x39); // 0
                            }
                        }
                        else
                        {
                            CSignetic6502.MemoryBus.Keyboard.PressKey((byte)key.KeyChar);

                        }
                    }
                }
                else
                {
                    if (key.Key != ConsoleKey.NoName)
                    {
                        Thread.Sleep(200);

                        if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                        {
                            CSignetic6502.MemoryBus.Keyboard.ReleaseKey((byte)0x12);
                            if (key.Key == ConsoleKey.D1)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x31); // 1
                            }
                            else if (key.Key == ConsoleKey.D2)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x32); // 2
                            }
                            else if (key.Key == ConsoleKey.D3)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x33); // 3
                            }
                            else if (key.Key == ConsoleKey.D4)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x34); // 4
                            }
                            else if (key.Key == ConsoleKey.D5)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x35); // 5
                            }
                            else if (key.Key == ConsoleKey.D6)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0xba); // 6
                            }
                            else if (key.Key == ConsoleKey.D7)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x36); // 7
                            }
                            else if (key.Key == ConsoleKey.D8)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x37); // 8
                            }
                            else if (key.Key == ConsoleKey.D9)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x38); // 9
                            }
                            else if (key.Key == ConsoleKey.D0)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey(0x39); // 0
                            }
                        }
                        else
                        {
                            CSignetic6502.MemoryBus.Keyboard.ReleaseKey((byte)key.KeyChar);
                        }

                        key = new ConsoleKeyInfo((char)0, ConsoleKey.NoName, false, false, false);

                    }          
                }
            }

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            manualResetEvent.WaitOne();

        }
    }
}
