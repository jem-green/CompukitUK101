﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using UK101Library;
using static UK101Form.KeyboardMatrix;
using TracerLibrary;
using System.IO;
using UK101Form.Properties;
using System.Runtime.InteropServices;

namespace UK101Form
{
    public partial class ConsoleForm : Form
    {
        #region Fields

        private Micro _uk101;
        private Tape _tape;
        private Display _display;

        //readonly IPeripheralIO _formIO;
        KeyboardMatrix _keyboardMatrix;

        [DllImport("user32.dll")]

        private static extern short GetAsyncKeyState(Keys key);
        bool _stopped = true;

        // Declare a delegate used to communicate with the UI thread
        public delegate void UpdateTextDelegate();
        private UpdateTextDelegate updateTextDelegate = null;

        // Declare our worker thread
        private Thread workerThread = null;
        FormIO _formIO = null;

        // display updating fix

        bool _updated = false;
        int _height = 32;
        int _width = 50;
        int _scale = 1;
        double _aspect = 1;

        // Most recently used
        protected MruStripMenu mruMenu;

        #endregion
        #region Constructor

        public ConsoleForm(string path, string name, string extension)
        {
            Debug.WriteLine("In ConsoleForm()");

            InitializeComponent();

            this.Icon = Resources.compukit;

            // Add this display

            _display = new Display();
            _display.Width = _width;
            _display.Height = _height;
            _display.Left = 0;
            _display.Top = 0;
            _display.Scale = _scale;
            _display.Aspect = _aspect;
            _display.CharacterGenerator = new CHARGEN(0x0);

            _formIO = new FormIO(_display);
            _formIO.TextReceived += new EventHandler<TextEventArgs>(OnMessageReceived);
            _formIO.Top = 0;
            _formIO.Left = 11;
            _formIO.Width = _width;
            _formIO.Height = _height;

            // Fit the picturebox to the form

            consolePictureBox.Select();

            // Fix the form size

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            SetLimits();

            this.KeyPreview = true;

            // Add most recent used
            mruMenu = new MruStripMenuInline(fileMenuItem, recentFileToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), 4);
            LoadFiles();

            // Add a consolePictureBox update timer

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (int)(0.01 * 1000); // 1 second
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

            // Add the tape

            _tape = new Tape(_formIO);

            this.Text = "uk101 " + ProductVersion;

            if ((path.Length > 0) && (name.Length > 0))
            {
                consolePictureBox.Invalidate();
                this.Text = "uk101 " + ProductVersion + " - " + name;

                string filenamePath = "";
                filenamePath = path + Path.DirectorySeparatorChar + name + "." + extension;
                _tape.Path = path;
                _tape.Name = name + "." + extension;
                mruMenu.AddFile(filenamePath);

                try
                {
                    // Start the simulator

                    this.workerThread = new Thread(new ThreadStart(this.Run));
                    this.workerThread.Start();

                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }
            else
            {
                // Start the simulator

                this.workerThread = new Thread(new ThreadStart(this.Run));
                this.workerThread.Start();
            }
            Debug.WriteLine("Out ConsoleForm()");
        }

        #endregion
        #region Methods
        private void Run()
        {
            _uk101 = new Micro(_formIO);
            _keyboardMatrix = new KeyboardMatrix();
            _uk101.Init(_height);
            VDU VDU = (VDU)_uk101["VDU"];
            VDU.Init();

            try
            {
                _uk101.Run();
            }
            catch (Exception e)
            {
                TraceInternal.TraceVerbose(e.ToString());
            }
        }

        private void LoadFiles()
        {
            Debug.WriteLine("In LoadFiles()");
            TraceInternal.TraceVerbose("Files " + Properties.Settings.Default.FileCount);
            for (int i = 0; i < 4; i++)
            {
                string property = "File" + (i + 1);
                string file = (string)Properties.Settings.Default[property];
                if (file != "")
                {
                    mruMenu.AddFile(file);
                    TraceInternal.TraceVerbose("Load " + file);
                }
            }
            Debug.WriteLine("Out LoadFiles()");
        }

        private void SaveFiles()
        {
            Debug.WriteLine("In SaveFiles");
            string[] files = mruMenu.GetFiles();
            Properties.Settings.Default["FileCount"] = files.Length;
            TraceInternal.TraceVerbose("Files=" + files.Length);
            for (int i = 0; i < 4; i++)
            {
                string property = "File" + (i + 1);
                if (i < files.Length)
                {
                    Properties.Settings.Default[property] = files[i];
                    TraceInternal.TraceVerbose("Save " + property + "=" + files[i]);
                }
                else
                {
                    Properties.Settings.Default[property] = "";
                    TraceInternal.TraceVerbose("Save " + property + "=");
                }
            }
            Debug.WriteLine("Out SaveFiles");
        }

        private void UpdateText()
        {
            consolePictureBox.Invalidate();
        }

        private void SetScaleMenu()
        {

            if (_display.Scale == 1)
            {
                smallToolStripMenuItem.Enabled = false;
            }
            else
            {
                smallToolStripMenuItem.Enabled = true;
            }

            if (_display.Scale == 2)
            {
                mediumToolStripMenuItem.Enabled = false;
            }
            else
            {
                mediumToolStripMenuItem.Enabled = true;
            }

            if (_display.Scale == 3)
            {
                largeToolStripMenuItem.Enabled = false;
            }
            else
            {
                largeToolStripMenuItem.Enabled = true;
            }

            if (_aspect < 1)
            {
                lessToolStripMenuItem.Enabled = false;
            }
            else
            {
                lessToolStripMenuItem.Enabled = true;
            }
            if (_aspect == 1)
            {
                equalToolStripMenuItem.Enabled = false;
            }
            else
            {
                equalToolStripMenuItem.Enabled = true;

            }
            if (_aspect > 1)
            {
                greaterToolStripMenuItem.Enabled = false;
            }
            else
            {
                greaterToolStripMenuItem.Enabled = true;
            }
        }

        private void SetTapeMenu()
        {
            ACIA ACIA = (ACIA)_uk101["ACIA"];
            if (ACIA.Mode == ACIA.IO_MODE_6820_NONE)
            {
                enableToolStripMenuItem.Enabled = true;
                playToolStripMenuItem.Enabled = false;
                recordToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = false;
                disableToolStripMenuItem.Enabled = false;
            }
            else
            {
                enableToolStripMenuItem.Enabled = false;


                if ((_tape.Mode == Tape.TapeMode.Enabled) || (_tape.Mode == Tape.TapeMode.Stopped))
                {
                    playToolStripMenuItem.Enabled = true;
                    recordToolStripMenuItem.Enabled = true;
                    stopToolStripMenuItem.Enabled = false;
                    disableToolStripMenuItem.Enabled = true;
                }
                else if (_tape.Mode == Tape.TapeMode.Playing)
                {
                    playToolStripMenuItem.Enabled = false;
                    recordToolStripMenuItem.Enabled = false;
                    stopToolStripMenuItem.Enabled = true;
                    disableToolStripMenuItem.Enabled = false;
                }
                else if (_tape.Mode == Tape.TapeMode.Recording)
                {
                    playToolStripMenuItem.Enabled = false;
                    recordToolStripMenuItem.Enabled = false;
                    stopToolStripMenuItem.Enabled = true;
                    disableToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void SetLimits()
        {
            _display.Scale = _scale;
            _display.Aspect = _aspect;
            int height = 39 + this.MainMenuStrip.Height;    // Must be some fixed elements to the form
            int width = 16;                                 // Must have some fixed element to the form (8 pixels each side)
            if (_aspect > 1)
            {
                height = height + _scale * _height * 8;
                width = (int)(width + 400 * _scale * _aspect);
            }
            else
            {
                height = (int)(height + _scale * _height * 8 / _aspect);
                width = width + 400 * _scale;
            }

            // Height
            // 296 = 37 x 8 - for small = 1
            // 552 = 69 x 8 - for medium = 2
            // 808 = 101 x 8 - for large = 3
            // Width
            // 417 - for small = 1
            // 817 - For medium = 2
            // 1217 - for large = 3

            this.MinimumSize = new Size(width, height); // 40
            this.MaximumSize = new Size(width, height);

            this.consolePictureBox.Invalidate();
        }

        #endregion
        #region Events

        // Define the event handlers.
        private void OnMessageReceived(object source, TextEventArgs e)
        {
            _updated = true;
            //if (_refreshing == false)
            //{
            //    this.Invoke(this.updateTextDelegate);
            //}
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Only refresh if there has been a change
            if (_updated == true)
            {
                consolePictureBox.Invalidate();
            }
        }

        private void OnMruFile(int number, string filenamePath)
        {
            string path = "";
            string name = "";
            string extension = "";
            int pos;

            if (File.Exists(filenamePath) == true)
            {
                mruMenu.SetFirstFile(number);
                pos = filenamePath.LastIndexOf('.');
                if (pos > 0)
                {
                    extension = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filenamePath = filenamePath.Substring(0, pos);
                }
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    path = filenamePath.Substring(0, pos);
                    name = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                }
                else
                {
                    path = filenamePath;
                }
                TraceInternal.TraceInformation("Use Name=" + name);
                TraceInternal.TraceInformation("Use Path=" + path);
                TraceInternal.TraceInformation("Use Extension=" + extension);

                this.Text = "uBasic " + ProductVersion + " - " + name;

                filenamePath = path + Path.DirectorySeparatorChar + name + "." + extension;
                _tape.Path = path;
                _tape.Name = name + "." + extension;

            }
            else
            {
                mruMenu.RemoveFile(number);
            }
        }

        #endregion

        #region consolePictureBox Events

        private void ConsolePictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Bitmap b = _display.Generate();
            g.DrawImageUnscaled(b, 0, 0);
            _updated = false;
        }

        private void ConsolePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            consolePictureBox.Invalidate();
        }

        #endregion

        #region Form Events
        
        private void ConsoleForm_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys modifiers = e.Modifiers;
            int keyValue = e.KeyValue;

            if (keyCode == Keys.Escape)
            {
                // I think this should be a key sequence as in the reset buttons
                // rather than a direct call to the processor.
                _uk101.Reset();
            }
            else if (keyCode == Keys.F1) // Enable tape mode
            {
                // Enable tape mode
                TraceInternal.TraceVerbose("Enable Tape");
                ACIA ACIA = (ACIA)_uk101["ACIA"];
                ACIA.Mode = ACIA.IO_MODE_6820_TAPE;
                // Would like to consider a _tape.Open() 
            }
            else if (keyCode == Keys.F2)  // Play the tape
            {
                // Play tape
                TraceInternal.TraceVerbose("Play tape");
                _tape.Play();
            }
            else if (keyCode == Keys.F3)  // Record to the tape
            {
                // Record tape
                TraceInternal.TraceVerbose("Record to tape");
                _tape.Record();
            }
            else if (keyCode == Keys.F4)  // Stop the tape
            {
                // Stop tape
                TraceInternal.TraceVerbose("Stop tape");
                _tape.Stop(_tape.Path,"test.bas");
            }
            else if (keyCode == Keys.F5) // Disble tape mode
            {
                // Disable tape mode
                TraceInternal.TraceVerbose("Disable tape");
                ACIA ACIA = (ACIA)_uk101["ACIA"];
                ACIA.Mode = ACIA.IO_MODE_6820_NONE;
                // Would like to consider a _tape.Close() 
            }
            else
            {
                if (ConsoleForm.IsKeyLocked(Keys.CapsLock) == true)
                {
                    _formIO.KeyStates[7] &= 0xfe;  // 1111 1110
                }
                else
                {
                    _formIO.KeyStates[7] |= 0x01;  // 0000 0001
                }

                // Scan codes:
                // Shift left  = 0x2a  send to UK101 as 0x12
                // Shift right = 0x36  send to UK101 as 0x10
                // Caps lock   = 0x3a  send to UK101 as 0x14
                // Ctrl left   = 0x1d  send to UK101 as 0x11
                // Ctrl right  = 0x1d  send to UK101 as 0x11
                // Arrow up    = 0x48  send to UK101 as 0xba
                // Enter       = 0x1c  send to UK101 as 0x0d

                TraceInternal.TraceVerbose("Down KeyCode " + keyCode.ToString());
                TraceInternal.TraceVerbose("Down Modifiers " + modifiers.ToString());
                TraceInternal.TraceVerbose("Down Key " + keyValue.ToString());

                Key key;
                if (modifiers == Keys.Shift)
                {
                    //if (keyCode == Keys.ShiftKey)
                    //{
                    //    TraceInternal.TraceVerbose("Apply shift");
                    //    if (Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)))
                    //    {
                    //        key = _keyboardMatrix.GetKey(Keys.LShiftKey,false);
                    //        _formIO.PressKey(key.Row, key.Column);
                    //    }
                    //    else if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
                    //    {
                    //        key = _keyboardMatrix.GetKey(Keys.RShiftKey,false);
                    //        _formIO.PressKey(key.Row, key.Column);
                    //    }
                    //}
                    //else
                    //{
                    TraceInternal.TraceVerbose("lshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)));
                    TraceInternal.TraceVerbose("rshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)));

                    key = _keyboardMatrix.GetKey(keyCode, true);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            TraceInternal.TraceVerbose("Apply shift");
                            key = _keyboardMatrix.GetKey(Keys.LShiftKey, false);
                            _formIO.PressKey(key.Row, key.Column);
                        }
                        key = _keyboardMatrix.GetKey(keyCode, true);
                        _formIO.PressKey(key.Row, key.Column);
                    }
                    //}
                }
                else
                {
                    key = _keyboardMatrix.GetKey(keyCode, false);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            TraceInternal.TraceVerbose("Apply shift");
                            key = _keyboardMatrix.GetKey(Keys.LShiftKey, false);
                            _formIO.PressKey(key.Row, key.Column);
                        }
                        key = _keyboardMatrix.GetKey(keyCode, false);
                        _formIO.PressKey(key.Row, key.Column);
                    }
                }
            }
        }

        private void ConsoleForm_KeyUp(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys modifiers = e.Modifiers;
            int keyValue = e.KeyValue;

            if (keyCode == Keys.F1) // Enable tape mode
            {
                // Eanble tape
                //Debug.WriteLine("Enable Tape");
            }
            else if (keyCode == Keys.F2)  // Play the tape
            {
                // Play tape
                //Debug.WriteLine("Play tape");
            }
            else if (keyCode == Keys.F3)  // Record to the tape
            {
                // Record tape
                //Debug.WriteLine("Record to tape");
            }
            else if (keyCode == Keys.F4)  // Stop the tape
            {
                // Stop tape
                //Debug.WriteLine("Stop tape");
            }
            else if (keyCode == Keys.F5) // Disble tape mode
            {
                // Disable tape mode
                //Debug.WriteLine("Disable tape");
            }
            else
            {
                if (ConsoleForm.IsKeyLocked(Keys.CapsLock) == true)
                {
                    _formIO.KeyStates[7] &= 0xfe;
                }
                else
                {
                    _formIO.KeyStates[7] |= 0x01;
                }

                TraceInternal.TraceVerbose("Up KeyCode " + keyCode.ToString());
                TraceInternal.TraceVerbose("Up Modifiers " + modifiers.ToString());
                TraceInternal.TraceVerbose("Up Key " + keyValue.ToString());

                Key key;
                if (modifiers == Keys.Shift)
                {
                    //if (keyCode == Keys.ShiftKey)
                    //{
                    //    if (Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)))
                    //    {
                    //        key = _keyboardMatrix.GetKey(Keys.LShiftKey,false);
                    //        _formIO.ReleaseKey(key.Row, key.Column);
                    //    }
                    //    else if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
                    //    {
                    //        key = _keyboardMatrix.GetKey(Keys.RShiftKey,false);
                    //        _formIO.ReleaseKey(key.Row, key.Column);
                    //    }
                    //}
                    //else
                    //{
                    TraceInternal.TraceVerbose("lshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)));
                    TraceInternal.TraceVerbose("rshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)));

                    key = _keyboardMatrix.GetKey(keyCode, true);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            TraceInternal.TraceVerbose("Release shift");
                            key = _keyboardMatrix.GetKey(Keys.LShiftKey, false);
                            _formIO.ReleaseKey(key.Row, key.Column);
                        }
                        key = _keyboardMatrix.GetKey(keyCode, true);
                        _formIO.ReleaseKey(key.Row, key.Column);
                    }
                    //}
                }
                else
                {
                    key = _keyboardMatrix.GetKey(keyCode, false);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            TraceInternal.TraceVerbose("Release shift");
                            key = _keyboardMatrix.GetKey(Keys.LShiftKey, false);
                            _formIO.ReleaseKey(key.Row, key.Column);
                        }
                        key = _keyboardMatrix.GetKey(keyCode, false);
                        _formIO.ReleaseKey(key.Row, key.Column);
                    }
                }
            }
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("In ConsoleForm_Load()");

            Settings.Default.Upgrade();

            // Set window location
            if (Settings.Default.ConsoleLocation != null)
            {
                // fIx errors with location being negative or off the main display

                this.Location = Settings.Default.ConsoleLocation;
                if ((this.Location.X < 0) || (this.Location.Y < 0))
                {
                    this.Location = new Point(0, 0);
                }
            }
            this.WindowState = FormWindowState.Normal;


            // Set window size
            if (Settings.Default.ConsoleSize != null)
            {
                this.Size = Settings.Default.ConsoleSize;
            }

            // Set Console font color
            if (Settings.Default.ConsoleFontColor != null)
            {
                this.consolePictureBox.ForeColor = Settings.Default.ConsoleFontColor;
            }

            // Set Console color
            if (Settings.Default.ConsoleColor != null)
            {
                this.consolePictureBox.BackColor = Settings.Default.ConsoleColor;
            }

            //// Set Display height
            //if (Settings.Default.Height > 0)
            //{
            //    _height = Settings.Default.Height;
            //}

            // Set Display width
            if (Settings.Default.Width > 0)
            {
                _width = Settings.Default.Width;
            }

            // Set Console scaling
            if (Settings.Default.ConsoleScale > 0)
            {
                _aspect = Settings.Default.ConsoleAspect;
                _scale = Settings.Default.ConsoleScale;
            }

            SetLimits();
            SetTapeMenu();
            SetScaleMenu();

            Debug.WriteLine("Out ConsoleForm_Load()");
        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("In ConsoleForm_FormClosing()");

            // Need to stop the thread
            // think i will try a better approach

            if (_stopped == false)
            {
                workerThread.Abort();
            }

            // Copy window location to app settings
            Settings.Default.ConsoleLocation = this.Location;

            // Copy window size to app settings
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.ConsoleSize = this.Size;
            }
            else
            {
                Settings.Default.ConsoleSize = this.RestoreBounds.Size;
            }

            // Copy console font color to app settings
            Settings.Default.ConsoleFontColor = this.consolePictureBox.ForeColor;

            // Copy console color to app settings
            Settings.Default.ConsoleColor = this.consolePictureBox.BackColor;

            // Copy console scale to app settings
            Settings.Default.ConsoleScale = _display.Scale;

            // Copy console scale to app settings
            Settings.Default.ConsoleAspect = _display.Aspect;

            // Copy display height to app settings
            Settings.Default.Height = _height;

            // Copy display width to app settings
            Settings.Default.Width = _width;

            // Safe Mru
            SaveFiles();

            // Save settings
            Settings.Default.Save();

            // Upgrade settings
            Settings.Default.Reload();

            Debug.WriteLine("Out ConsoleForm_FormClosing()");
        }

        #endregion
        #region Menu Events

        private void FileOpenMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("In FileOpenMenuItem_Click()");

            string path = "";
            string name = "";
            string extension = "";
            int pos;

            //consolePictureBox.Enabled = false;
            //consolePictureBox.Visible = false;
            if (_stopped == false)
            {
                workerThread.Abort();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "uk101 (*.bas;*.mc)|*.bas;*.mc",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filenamePath = openFileDialog.FileName;
                pos = filenamePath.LastIndexOf('.');
                if (pos > 0)
                {
                    extension = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                    filenamePath = filenamePath.Substring(0, pos);
                }
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    path = filenamePath.Substring(0, pos);
                    name = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                }
                else
                {
                    name = filenamePath;
                }
                TraceInternal.TraceInformation("Use Name=" + name);
                TraceInternal.TraceInformation("Use Path=" + path);
                TraceInternal.TraceInformation("Use Path=" + extension);

                this.Text = "uk101 " + ProductVersion + " - " + name;

                filenamePath = path + Path.DirectorySeparatorChar + name + "." + extension;
                _tape.Path = path;
                _tape.Name = name + "." + extension;
                mruMenu.AddFile(filenamePath);

            }
            Debug.WriteLine("Out FileOpenMenuItem_Click()");
        }

        private void FileExitMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Out FileExitMenuItem_Click()");
            this.Close();
        }

        private void FormatColorMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("In FileExitMenuItem_Click()");
            ColorDialog colorDialog = new ColorDialog
            {
                Color = this.consolePictureBox.BackColor
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color color = colorDialog.Color;
                this.consolePictureBox.BackColor = color;

            }
            Debug.WriteLine("Out FileExitMenuItem_Click()");
        }

        private void Display16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _height = 16;
            _formIO.Height = _height;
            _display.Height = _height;
            _uk101.Height = _height;
            _uk101.SetLines(_height);
            SetLimits();
            SetScaleMenu();
        }

        private void EnableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TraceInternal.TraceVerbose("Enable Tape");
            ACIA ACIA = (ACIA)_uk101["ACIA"];
            ACIA.Mode = ACIA.IO_MODE_6820_TAPE;
            _tape.Open();
            SetTapeMenu();
        }

        private void PlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Play tape
            TraceInternal.TraceVerbose("Play tape");
            _tape.Play();
            SetTapeMenu();
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stop tape
            TraceInternal.TraceVerbose("Stop tape");
            _tape.Stop(_tape.Path, "test.bas");
            SetTapeMenu();
        }

        private void recordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Record tape
            TraceInternal.TraceVerbose("Record to tape");
            _tape.Record();
            SetTapeMenu();
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Disable tape mode
            TraceInternal.TraceVerbose("Disable tape");
            //_uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_NONE;
            _tape.Close();
            SetTapeMenu();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset basic
            _uk101.Reset();
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _scale = 3;
            SetLimits();
            SetScaleMenu();
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _scale = 2;
            SetLimits();
            SetScaleMenu();
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _scale = 1;
            SetLimits();
            SetScaleMenu();
        }

        private void equalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _aspect = 1;
            SetLimits();
            SetScaleMenu();
        }

        private void greaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _aspect = 2;
            SetLimits();
            SetScaleMenu();
        }

        private void lessToolStripMenuItem_Click(object sender, EventArgs e)
        {;
            _aspect = 0.5;
            SetLimits();
            SetScaleMenu();
        }

        #endregion

        private void Display32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _height = 32;
            _formIO.Height = _height;
            _display.Height = _height;
            _uk101.Height = _height;
            _uk101.SetLines(_height);
            SetLimits();
            SetScaleMenu();
        }
    }
}

