using System;
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

        private UK101 _uk101;
        private Tape _tape;
        private Display _display;

        //readonly IPeripheralIO _formIO;
        KeyboardMatrix _keyboardMatrix;

        [DllImport("user32.dll")]

        private static extern short GetAsyncKeyState(Keys key);
        int pos = 0;
        bool _stopped = true;

        // Declare a delegate used to communicate with the UI thread
        public delegate void UpdateTextDelegate();
        private UpdateTextDelegate updateTextDelegate = null;

        // Declare our worker thread
        private Thread workerThread = null;
        FormIO _formIO = null;

        byte _row = 2;
        byte _column = 3;

        bool _refreshing = false;
        bool _updated = false;

        // Most recently used
        protected MruStripMenu mruMenu;

        #endregion

        public ConsoleForm(string path, string name, string extension)
        {
            Debug.WriteLine("In ConsoleForm()");

            InitializeComponent();

            this.Icon = Resources.compukit;

            // Add this display

            _display = new Display();
            _display.Width = 50;
            _display.Height = 32;
            _display.Left = 0;
            _display.Top = 0;

            _display.CharacterGenerator = new CHARGEN(0x0);

            _formIO = new FormIO(_display);
            _formIO.TextReceived += new EventHandler<TextEventArgs>(OnMessageReceived);
            _formIO.Top = 0;
            _formIO.Left = 11;
            _formIO.Width = 50;
            _formIO.Height = 32;

            // Initialise the delegate
            //this.updateTextDelegate = new UpdateTextDelegate(this.UpdateText);

            consolePictureBox.Width = 50 * 8;
            consolePictureBox.Height = 32 * 8;
            consolePictureBox.Select();

            // Fix the form size

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            int h = this.MainMenuStrip.Height;
            this.MinimumSize = new Size(52 * 8, h + 37 * 8 - 1);
            this.MaximumSize = new Size(52 * 8, h + 37 * 8 - 1);
            //this.Size = new Size(16 * 8, 16 * 8);

            this.KeyPreview = true;

            // Add most recent used
            mruMenu = new MruStripMenuInline(fileMenuItem, recentFileToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), 4);
            LoadFiles();

            // Add a picturebox update timer

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (int)(0.01 * 1000); // 1 second
            timer.Tick += new EventHandler(timer_Tick);
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

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _refreshing = true;
            Graphics g = e.Graphics;
            Bitmap b = _display.Generate();
            g.DrawImageUnscaled(b, 0, 0);
            _refreshing = false;
            _updated = false;
        }

        private void OnMruFile(int number, string filenamePath)
        {
            string path = "";
            string name = "";
            string extension = "";

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

                char[] program;
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
                mruMenu.RemoveFile(number);
            }
        }

        //private void UpdateText()
        //{
        //    pictureBox1.Invalidate();   
        //}

        // Define the event handlers.
        private void OnMessageReceived(object source, TextEventArgs e)
        {
            _updated = true;
            //if (_refreshing == false)
            //{
            //    this.Invoke(this.updateTextDelegate);
            //}
        }

        private void Run()
        {
            _uk101 = new UK101(_formIO);
            _keyboardMatrix = new KeyboardMatrix();
            _uk101.Init();
            _uk101.MemoryBus.VDU.Init();
            try
            {
                _uk101.Run();
            }
            catch (Exception e)
            {
                TraceInternal.TraceVerbose(e.ToString());
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            consolePictureBox.Invalidate();
        }

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
                _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_TAPE;
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
                _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_NONE;
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

        private void FileOpenMenuItem_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("In FileOpenMenuItem_Click()");

            string path = "";
            string name = "";
            string extension = "";

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

            Debug.WriteLine("Out ConsoleForm_Load()");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Only refresh if there has been a change
            if (_updated == true)
            {
                consolePictureBox.Invalidate();
            }
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

            // Safe Mru
            SaveFiles();

            // Save settings
            Settings.Default.Save();

            // Upgrade settings
            Settings.Default.Reload();

            Debug.WriteLine("Out ConsoleForm_FormClosing()");
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

        public void SaveFiles()
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

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TraceInternal.TraceVerbose("Enable Tape");
            _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_TAPE;
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Play tape
            TraceInternal.TraceVerbose("Play tape");
            _tape.Play();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Stop tape
            TraceInternal.TraceVerbose("Stop tape");
            _tape.Stop(_tape.Path, "test.bas");
        }

        private void recordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Record tape
            TraceInternal.TraceVerbose("Record to tape");
            _tape.Record();
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Disable tape mode
            TraceInternal.TraceVerbose("Disable tape");
            _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_NONE;
            // Would like to consider a _tape.Close() 
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset basic
            _uk101.Reset();
        }
    }
}

