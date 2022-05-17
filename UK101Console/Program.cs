using System;
using System.IO;
using System.Threading;
using TracerLibrary;
using UK101Library;
using static UK101Console.KeyboardMatrix;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace UK101Console
{
    class Program
    {
        #region Fields
        static readonly IPeripheralIO _consoleIO = new ConsoleIO();
        public static bool isclosing = false;
        static private HandlerRoutine ctrlCHandler;

        #endregion
        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        #endregion
        #region Methods
        static void Main(string[] args)
        {
            Debug.WriteLine("Enter Main()");
			
            ctrlCHandler = new HandlerRoutine(ConsoleCtrlCheck);
            SetConsoleCtrlHandler(ctrlCHandler, true);

            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            Parameter<string> filePath = new Parameter<string>();
            Parameter<string> filename = new Parameter<string>();

            // Get the default path directory

            filePath.Value = Environment.CurrentDirectory;
            filePath.Source = Parameter<string>.SourceType.App;

			Parameter<string> appPath = new Parameter<string>("");
            Parameter<string> appName = new Parameter<string>("UK101.xml");

            appPath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;

            int pos = appPath.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                appPath.Value = appPath.Value.ToString().Substring(0, pos);
                appPath.Source = Parameter<string>.SourceType.App;
            }

            Parameter<string> logPath = new Parameter<string>("");
            Parameter<string> logName = new Parameter<string>("UK101Console");
            logPath.Value = System.Reflection.Assembly.GetExecutingAssembly().Location;
            pos = logPath.Value.ToString().LastIndexOf(Path.DirectorySeparatorChar);
            if (pos > 0)
            {
                logPath.Value = logPath.Value.ToString().Substring(0, pos);
                logPath.Source = Parameter<string>.SourceType.App;
            }

            Parameter<SourceLevels> traceLevels = new Parameter<SourceLevels>();
            traceLevels.Value = TraceInternal.TraceLookup("CRITICAL");
            traceLevels.Source = Parameter<SourceLevels>.SourceType.App;

            // Configure tracer options

            string logFilenamePath = logPath.Value.ToString() + Path.DirectorySeparatorChar + logName.Value.ToString() + ".log";
            FileStreamWithRolling dailyRolling = new FileStreamWithRolling(logFilenamePath, new TimeSpan(1, 0, 0, 0), FileMode.Append);
            TextWriterTraceListenerWithTime listener = new TextWriterTraceListenerWithTime(dailyRolling);
            Trace.AutoFlush = true;
            TraceFilter fileTraceFilter = new System.Diagnostics.EventTypeFilter(SourceLevels.Verbose);
            listener.Filter = fileTraceFilter;
            //Trace.Listeners.Clear();
            Trace.Listeners.Add(listener);

            // Check if the config file has been paased in and overwrite the defaults

            string filenamePath = "";
            string extension = "";
            int items = args.Length;
            if (items == 1)
            {
                filenamePath = args[0].Trim('"');
                pos = filenamePath.LastIndexOf('.');
                if (pos > 0)
                {
                    extension = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filenamePath = filenamePath.Substring(0, pos);
                }
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    filePath.Value = filenamePath.Substring(0, pos);
                    filePath.Source = Parameter<string>.SourceType.Command;
                    filename.Value = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filename.Source = Parameter<string>.SourceType.Command;
                }
                else
                {
                    filename.Value = filenamePath;
                    filename.Source = Parameter<string>.SourceType.Command;
                }
                TraceInternal.TraceVerbose("Use filename=" + filename.Value);
                TraceInternal.TraceVerbose("use filePath=" + filePath.Value);
            }
            else
            {
                for (int item = 0; item < items; item++)
                {
                    string lookup = args[item];
                    if (!lookup.StartsWith("/"))
                    {
                        lookup = lookup.ToLower();
                    }
                    switch (lookup)
                    {
                        case "/D":
                        case "--debug":
                            {
                                string traceName = args[item + 1];
                                traceName = traceName.TrimStart('"');
                                traceName = traceName.TrimEnd('"');
                                traceLevels.Value = TraceInternal.TraceLookup(traceName);
                                traceLevels.Source = Parameter<SourceLevels>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value Name=" + traceLevels);
                                break;
                            }
                        case "/AN":
                        case "--name":
                            {
                                appName.Value = args[item + 1];
                                appName.Value = appName.Value.ToString().TrimStart('"');
                                appName.Value = appName.Value.ToString().TrimEnd('"');
                                appName.Source = Parameter<string>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value Name=" + appName);
                                break;
                            }
                        case "/AP":
                        case "--path":
                            {
                                appPath.Value = args[item + 1];
                                appPath.Value = appPath.Value.ToString().TrimStart('"');
                                appPath.Value = appPath.Value.ToString().TrimEnd('"');
                                appPath.Source = Parameter<string>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value Path=" + appPath);
                                break;
                            }
                        case "/n":
                        case "--logname":
                            {
                                logName.Value = args[item + 1];
                                logName.Value = logName.Value.ToString().TrimStart('"');
                                logName.Value = logName.Value.ToString().TrimEnd('"');
                                logName.Source = Parameter<string>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value logName=" + logName);
                                break;
                            }
                        case "/p":
                        case "--logpath":
                            {
                                logPath.Value = args[item + 1];
                                logPath.Value = logPath.Value.ToString().TrimStart('"');
                                logPath.Value = logPath.Value.ToString().TrimEnd('"');
                                logPath.Source = Parameter<string>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value logPath=" + logPath);
                                break;
                            }                
                        case "/N":
                        case "--filename":
                            {
                                filename.Value = args[item + 1];
                                filename.Value = filename.Value.ToString().TrimStart('"');
                                filename.Value = filename.Value.ToString().TrimEnd('"');
                                filename.Source = Parameter<string>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value Name=" + filename);
                                break;
                            }
                        case "/P":
                        case "--filepath":
                            {
                                filePath.Value = args[item + 1];
                                filePath.Value = filePath.Value.ToString().TrimStart('"');
                                filePath.Value = filePath.Value.ToString().TrimEnd('"');
                                filePath.Source = Parameter<string>.SourceType.Command;
                                TraceInternal.TraceVerbose("Use command value Path=" + filePath);
                                break;
                            }
                    }
                }

                // Adjust the log location if it has been overridden in the registry


                if (logPath.Source == Parameter<string>.SourceType.Command)
                {
                    logFilenamePath = logPath.Value.ToString() + Path.DirectorySeparatorChar + logName.Value.ToString() + ".log";
                }
                if (logName.Source == Parameter<string>.SourceType.Command)
                {
                    logFilenamePath = logPath.Value.ToString() + Path.DirectorySeparatorChar + logName.Value.ToString() + ".log";
                }
            }

            // Redirect the output

            listener.Flush();
            Trace.Listeners.Remove(listener);
            listener.Close();
            listener.Dispose();

            dailyRolling = new FileStreamWithRolling(logFilenamePath, new TimeSpan(1, 0, 0, 0), FileMode.Append);
            listener = new TextWriterTraceListenerWithTime(dailyRolling);
            Trace.AutoFlush = true;
            SourceLevels sourceLevels = TraceInternal.TraceLookup(traceLevels.Value.ToString());
            fileTraceFilter = new System.Diagnostics.EventTypeFilter(sourceLevels);
            listener.Filter = fileTraceFilter;
            Trace.Listeners.Add(listener);   

            TraceInternal.TraceInformation("Use Name=" + appName.Value);
            TraceInternal.TraceInformation("Use Path=" + appPath.Value);
            TraceInternal.TraceInformation("Use Filename=" + filename);
            TraceInternal.TraceInformation("Use FilePath=" + filePath);			
            TraceInternal.TraceInformation("Use Log Name=" + logName.Value);
            TraceInternal.TraceInformation("Use Log Path=" + logPath.Value);

			// Start the simulator
			
            UK101 _uk101 = new UK101(_consoleIO);
            KeyboardMatrix _keyboardMatrix = new KeyboardMatrix();
            _uk101.Init();
            _uk101.Run();
            Tape _tape = new Tape(_consoleIO);

            // Not sure we will return from run here.
            // This is slightly different here as need to poll for the keyboard
            // but want to move this to ConsoleIO. Simpler at this stage to expose
            // the cpu and memory bus here and will be 

            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo((char)0, ConsoleKey.NoName, false, false, false);

            while (1 == 1)
            {
                if (Console.KeyAvailable == true)
                {
                    keyInfo = Console.ReadKey(true);

                    //Debug.Print("KeyInfo " + keyInfo.ToString());
                    //Debug.Print("Modifiers " + keyInfo.Modifiers.ToString());
                    //Debug.Print("Key " + keyInfo.Key.ToString());

                    ConsoleKey keyCode = keyInfo.Key;
                    ConsoleModifiers modifiers = keyInfo.Modifiers;

                    if (keyCode == ConsoleKey.Escape)
                    {
                        // I think this should be a key sequence as in the reset buttons
                        // rather than a direct call to the processor.
                        _uk101.Reset();
                    }
                    else if (keyCode == ConsoleKey.F1)  // Enable tape mode
                    {
                        // Enable tape mode
                		Debug.WriteLine("Enable Tape");
                        _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_TAPE;
                        // Would like to consider a _tape.Open() 
                    }
                    else if (keyCode == ConsoleKey.F2)  // Play the tape
                    {
                        // Play tape
                        Debug.WriteLine("Play tape");
                        filenamePath = Path.Combine(filePath.Value.ToString(), filename.ToString() + ".ubt");
                        _tape.Play(filenamePath);

                    }
                    else if (keyCode == ConsoleKey.F3)  // Record to the tape
                    {
                        // Record tape
                        Debug.WriteLine("Record to tape");
                        _tape.Record();
                    }
                    else if (keyCode == ConsoleKey.F4)  // Stop the tape
                    {
                        // Stop tape
                        Debug.WriteLine("Stop tape");
                        _tape.Stop("test.bas");
                    }
                    else if (keyCode == ConsoleKey.F5)  // Disble tape mode
                    {
                        // Disable tape mode
                	Debug.WriteLine("Disable tape");
                        _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_NONE;
                        // Would like to consider a _tape.Close() 
                    }
                    else
                    {
                        if (Console.CapsLock == true)
                        {
                            _consoleIO.KeyStates[7] &= 0xfe; // 1111 1110
                        }
                        else
                        {
                            _consoleIO.KeyStates[7] |= 0x01; // 0000 0001
                        }

                        // Scan codes:
                        // Shift left  = 0x2a  send to UK101 as 0x12 - Assume consolekey.pause
                        // Shift right = 0x36  send to UK101 as 0x10
                        // Caps lock   = 0x3a  send to UK101 as 0x14 - Assume consolekey.pause
                        // Ctrl left   = 0x1d  send to UK101 as 0x11
                        // Ctrl right  = 0x1d  send to UK101 as 0x11
                        // Arrow up    = 0x48  send to UK101 as 0xba
                        // Enter       = 0x1c  send to UK101 as 0x0d

                		Debug.Print("Down KeyCode " + keyCode.ToString());
                		Debug.Print("Down Modifiers " + modifiers.ToString());

                        Key key;
                        if ((modifiers & ConsoleModifiers.Shift) != 0)
                        { 
                            key = _keyboardMatrix.GetKey(keyCode, true);
                            if (key.KeyCode != ConsoleKey.NoName)
                            {
                                if (key.Shift == true)
                                {
                                    TraceInternal.TraceVerbose("Apply shift");
                                    key = _keyboardMatrix.GetKey(ConsoleKey.Pause, false);
                                    _consoleIO.PressKey(key.Row, key.Column); // press shift
                                }
                                key = _keyboardMatrix.GetKey(keyCode, true);
                                _consoleIO.PressKey(key.Row, key.Column);
                            }
                        }
                        else
                        {
                            key = _keyboardMatrix.GetKey(keyCode, false);
                            if (key.KeyCode != ConsoleKey.NoName)
                            {
                                if (key.Shift == true)
                                {
                                    TraceInternal.TraceVerbose("Apply shift");
                                    key = _keyboardMatrix.GetKey(ConsoleKey.Pause, false);
                                    _consoleIO.PressKey(key.Row, key.Column); // press shift
                                }
                                key = _keyboardMatrix.GetKey(keyCode, false);
                                _consoleIO.PressKey(key.Row, key.Column);
                            }
                        }
                    }
                }
                else
                {
                    ConsoleKey keyCode = keyInfo.Key;
                    ConsoleModifiers modifiers = keyInfo.Modifiers;

                    if (keyCode != ConsoleKey.NoName)
                    {
                        //Debug.Print("KeyInfo " + keyInfo.ToString());
                        //Debug.Print("Modifiers " + keyInfo.Modifiers.ToString());
                        //Debug.Print("Key " + keyInfo.Key.ToString());

                        Thread.Sleep(200);
                        
                        // There is a problem here as the key can get overwriten and then repeats
                        // almost want to prevent the next key from being pressed until the key has been read
                        // of have a buffer. Still getting some effects.

                        Key key;
                        if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
                        {
                            key = _keyboardMatrix.GetKey(keyCode, true);
                            if (key.KeyCode != ConsoleKey.NoName)
                            {
                                //Debug.Print("Deselect shift");
                                if (key.Shift == true)
                                {
                                	Debug.Print("Apply shift");
                                    key = _keyboardMatrix.GetKey(ConsoleKey.Pause, false);
                                    _consoleIO.ReleaseKey(key.Row, key.Column); // press shift
                                }
                                key = _keyboardMatrix.GetKey(keyCode, true);
                                _consoleIO.ReleaseKey(key.Row, key.Column);
                            }
                        }
                        else
                        {
                            key = _keyboardMatrix.GetKey(keyCode, false);
                            if (key.KeyCode != ConsoleKey.NoName)
                            {
                                //Debug.Print("Release shift");
                                if (key.Shift == true)
                                {
                            		Debug.Print("Apply shift");
                                    key = _keyboardMatrix.GetKey(ConsoleKey.Pause, false);
                                    _consoleIO.ReleaseKey(key.Row, key.Column); // press shift
                                }
                                key = _keyboardMatrix.GetKey(keyCode, false);
                                _consoleIO.ReleaseKey(key.Row, key.Column);
                            }
                        }
                        keyInfo = new ConsoleKeyInfo((char)0, ConsoleKey.NoName, false, false, false);
                    }          
                }
            }

            Debug.WriteLine("Exit Main()");
        }

        #endregion
        #region Private

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            Debug.WriteLine("Enter ConsoleCtrlCheck()");

            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isclosing = true;
                    TraceInternal.TraceVerbose("CTRL+C received:");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    isclosing = true;
                    TraceInternal.TraceVerbose("CTRL+BREAK received:");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isclosing = true;
                    TraceInternal.TraceVerbose("Program being closed:");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isclosing = true;
                    TraceInternal.TraceVerbose("User is logging off:");
                    break;

            }
            Debug.WriteLine("Exit ConsoleCtrlCheck()");

            Environment.Exit(0);

            return (true);

        }

        #endregion
    }
}
