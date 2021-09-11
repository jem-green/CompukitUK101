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
        static MainPage mainPage;
        static Int32 ProcessorCycles = 0;
        static bool Hold = false;
        static private Timer _timer = null;
        static CSignetic6502 CSignetic6502;
        static protected readonly object _lockObject = new Object();

        static void Main(string[] args)
        {
            mainPage = new MainPage();
            CSignetic6502 = new CSignetic6502(mainPage);
            CSignetic6502.MemoryBus.VDU.InitCVDU(mainPage);
            CSignetic6502.MemoryBus.ACIA.Lines = CSignetic6502.MemoryBus.ACIA.basicProg.test;
            CSignetic6502.Reset();

            // Create a Timer object that knows to call our TimerCallback
            // method once every 2000 milliseconds.
            _timer = new Timer(Timer_Tick, null, 0, 1);

            ConsoleKeyInfo key = new ConsoleKeyInfo((char)0, ConsoleKey.NoName, false, false, false);
            int count = 0;
            while (1 == 1)
            {
                if (Console.KeyAvailable == true)
                {
                    key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        CSignetic6502.Reset();
                        break;
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
                        }

                        if (key.Key == ConsoleKey.Enter)
                        {
                            CSignetic6502.MemoryBus.Keyboard.PressKey(13);
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
                        if (count > 1000)
                        {
                            CSignetic6502.MemoryBus.Keyboard.ReleaseKey((byte)key.KeyChar);

                            if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
                            {
                                CSignetic6502.MemoryBus.Keyboard.ReleaseKey((byte)0x12);
                            }

                            key = new ConsoleKeyInfo((char)0, ConsoleKey.NoName, false, false, false);
                            count = 0;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
            }

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            manualResetEvent.WaitOne();

        }

        private static void Timer_Tick(object sender)
        {
            lock (_lockObject)
            {
                while (ProcessorCycles < 20000)
                {
                    if (!Hold)
                    {
                        ProcessorCycles += CSignetic6502.SingleStep();
                    }
                }
                ProcessorCycles -= 20000;
            }
        }

    }
}
