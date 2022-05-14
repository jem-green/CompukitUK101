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
        bool stopped = true;

        // Declare a delegate used to communicate with the UI thread
        public delegate void UpdateTextDelegate();
        private UpdateTextDelegate updateTextDelegate = null;

        // Declare our worker thread
        private Thread workerThread = null;
        FormIO _formIO = null;

        byte _row = 2;
        byte _column = 3;

        bool _updating = false;

        #endregion

        public ConsoleForm(string filepath, string name)
        {
            Debug.WriteLine("In ConsoleForm()");
            InitializeComponent();

            this.Icon = Resources.compukit;

            // Add this display

            _display = new Display();
            _display.Width = 64;
            _display.Height = 32;
            _display.Left = 0;
            _display.Top = 0;
            _display.CharacterGenerator = new CHARGEN(0x0);

            _formIO = new FormIO(_display);
            _formIO.TextReceived += new EventHandler<TextEventArgs>(OnMessageReceived);

            // Initialise the delegate
            this.updateTextDelegate = new UpdateTextDelegate(this.UpdateText);

            pictureBox1.Width = 88 * 8;
            pictureBox1.Height = 72 * 8;
            pictureBox1.Select();

            this.KeyPreview = true;

            // Add most recent used
            //mruMenu = new MruStripMenuInline(fileMenuItem, recentFileToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), 4);
            //LoadFiles();

            // Add the tape

            _tape = new Tape(_formIO);
			
			this.Text = "uk101 " + ProductVersion;
			
			if ((filepath.Length > 0) && (name.Length > 0))
            {
                pictureBox1.Invalidate();
                this.Text = "uk101 " + ProductVersion + " - " + name ;

                string filenamePath = "";
                filenamePath = filepath + Path.DirectorySeparatorChar + name + ".ubt";
                _tape.Filename = filenamePath;

                try
                {
                    // Start the simulator

                    _uk101 = new UK101(_formIO);
                    _keyboardMatrix = new KeyboardMatrix();
                    _uk101.Init();
                    _uk101.Run();
                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }
            else
            {
                this.workerThread = new Thread(new ThreadStart(this.Run));
                this.workerThread.Start();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _updating = true;
            Graphics g = e.Graphics;
            Bitmap b = _display.Generate();
            g.DrawImageUnscaled(b, 0, 0);
            _updating  = false;
        }

        private void OnMruFile(int number, String filenamePath)
        {
            string path = "";
            string filename = "";

            if (File.Exists(filenamePath) == true)
            {
                //mruMenu.SetFirstFile(number);
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    path = filenamePath.Substring(0, pos);
                    filename = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                }
                else
                {
                    path = filenamePath;
                }
                TraceInternal.TraceInformation("Use Name=" + filename);
                TraceInternal.TraceInformation("Use Path=" + path);

                this.Text = "uBasic " + ProductVersion + " - " + filename;

                filenamePath = path + Path.DirectorySeparatorChar + filename;
                char[] program;
                try
                {

                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }
            else
            {
                //mruMenu.RemoveFile(number);
            }
        }

        private void UpdateText()
        {
            pictureBox1.Invalidate();   
        }

        // Define the event handlers.
        private void OnMessageReceived(object source, TextEventArgs e)
        {
            if (_updating == false)
            {
                this.Invoke(this.updateTextDelegate);
            }
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
            pictureBox1.Invalidate();
        }

        private void ConsoleForm_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            Keys modifiers = e.Modifiers;
            int keyValue = e.KeyValue;

            if (keyCode == Keys.Escape)
            {
                _uk101.Reset();
            }
            else if (keyCode == Keys.F1) // Enable tape mode
            {
                // Enable tape mode
                Debug.WriteLine("Tape mode");
                _uk101.MemoryBus.ACIA.Mode = ACIA.IO_MODE_6820_TAPE;
                // Would like to consider a _tape.Open() 
            }
            else if (keyCode == Keys.F2)  // Play the tape
            {
                // Play tape
                Debug.WriteLine("Play tape");

                // Need to open up a file dialogue

                //string filenamePath = Path.Combine(filePath.Value.ToString(), filename.ToString() + ".ubt");
                //_tape.Play(filenamePath);

                _tape.Play();

            }
            else if (keyCode == Keys.F3)  // Record to the tape
            {
                // Record tape
                Debug.WriteLine("Record to tape");
                _tape.Record();
            }
            else if (keyCode == Keys.F4)  // Stop the tape
            {
                // Stop tape
                Debug.WriteLine("Stop tape");
                _tape.Stop("test.bas");
            }

            else if (keyCode == Keys.F5) // Disble tape mode
            {
                // Disable tape mode
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

                Debug.Print("Down KeyCode " + keyCode.ToString());
                Debug.Print("Down Modifiers " + modifiers.ToString());
                Debug.Print("Down Key " + keyValue.ToString());

                Key key;
                if (modifiers == Keys.Shift)
                {
                    if (keyCode == Keys.ShiftKey)
                    {
                        if (Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)))
                        {
                            key = _keyboardMatrix.GetKey(Keys.LShiftKey,false);
                            _formIO.PressKey(key.Row, key.Column);
                        }
                        else if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
                        {
                            key = _keyboardMatrix.GetKey(Keys.RShiftKey,false);
                            _formIO.PressKey(key.Row, key.Column);
                        }
                    }
                    else
                    {
                        TraceInternal.TraceVerbose("lshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)));
                        TraceInternal.TraceVerbose("rshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)));

                        key = _keyboardMatrix.GetKey(keyCode, true);
                        if (key.KeyCode != Keys.NoName)
                        {
                            TraceInternal.TraceVerbose("Apply shift");
                            if (key.Shift == true)
                            {
                                key = _keyboardMatrix.GetKey(Keys.LShiftKey, false);
                                _formIO.PressKey(key.Row, key.Column);
                            }
                            key = _keyboardMatrix.GetKey(keyCode, true);
                            _formIO.PressKey(key.Row, key.Column);
                        }
                    }
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

            if (keyCode == Keys.F3)
            {
                _formIO.ReleaseKey(7, 7);
                _formIO.ReleaseKey(_row, _column);
                //_column++;
                //if (_column > 6)
                //{
                //    _column = 0;
                //    _row++;
                //}
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

                Debug.Print("Up KeyCode " + keyCode.ToString());
                Debug.Print("Up Modifiers " + modifiers.ToString());
                Debug.Print("Up Key " + keyValue.ToString());

                Key key;
                if (modifiers == Keys.Shift)
                {
                    if (keyCode == Keys.ShiftKey)
                    {
                        if (Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)))
                        {
                            key = _keyboardMatrix.GetKey(Keys.LShiftKey,false);
                            _formIO.ReleaseKey(key.Row, key.Column);
                        }
                        else if (Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)))
                        {
                            key = _keyboardMatrix.GetKey(Keys.RShiftKey,false);
                            _formIO.ReleaseKey(key.Row, key.Column);
                        }
                    }
                    else
                    {
                        Debug.Print("lshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.LShiftKey)));
                        Debug.Print("rshift=" + Convert.ToBoolean(GetAsyncKeyState(Keys.RShiftKey)));

                        key = _keyboardMatrix.GetKey(keyCode, true);
                        if (key.KeyCode != Keys.NoName)
                        {
                            if (key.Shift == true)
                            {
                                Debug.Print("Apply shift");
                                key = _keyboardMatrix.GetKey(Keys.LShiftKey, false);
                                _formIO.ReleaseKey(key.Row, key.Column);
                            }
                            key = _keyboardMatrix.GetKey(keyCode, true);
                            _formIO.ReleaseKey(key.Row, key.Column);
                        }
                    }
                }
                else
                {
                    key = _keyboardMatrix.GetKey(keyCode, false);
                    if (key.KeyCode != Keys.NoName)
                    {
                        if (key.Shift == true)
                        {
                            Debug.Print("Apply shift");
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
            string filename = "";

            pictureBox1.Enabled = false;
            pictureBox1.Visible = false;
            if (stopped == false)
            {
                workerThread.Abort();
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "UK101 (*.bas)|*.bas",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filenamePath = openFileDialog.FileName;
                pos = filenamePath.LastIndexOf('\\');
                if (pos > 0)
                {
                    path= filenamePath.Substring(0, pos);
                    filename = filenamePath.Substring(pos + 1, filenamePath.Length - pos - 1);
                }
                else
                {
                    filename = filenamePath;
                }
                TraceInternal.TraceInformation("Use Name=" + filename);
                TraceInternal.TraceInformation("Use Path=" + path);

                //consoleTextBox.Text = "";
                this.Text = "UK101 " + ProductVersion + " - " + filename;


                filenamePath = path + Path.DirectorySeparatorChar + filename;
                char[] program;
                try
                {

                }
                catch (Exception e1)
                {
                    TraceInternal.TraceError(e1.ToString());
                }
            }
            Debug.WriteLine("Out FileOpenMenuItem_Click()");
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("In ConsoleForm_Load()");

            Settings.Default.Upgrade();

            this.WindowState = FormWindowState.Normal;

            // Fixed windows size

            //this.Width = textBoxIO.Width;

            //// Set window size
            //if (Settings.Default.ConsoleSize != null)
            //{
            //    this.Size = Settings.Default.ConsoleSize;
            //}

            //// Set Console font
            //if (Settings.Default.ConsoleFont != null)
            //{
            //    this.consoleTextBox.Font = Settings.Default.ConsoleFont;
            //}

            //// Set Console font color
            //if (Settings.Default.ConsoleFontColor != null)
            //{
            //    this.consoleTextBox.ForeColor = Settings.Default.ConsoleFontColor;
            //}

            //// Set Console color
            //if (Settings.Default.ConsoleColor != null)
            //{
            //    this.consoleTextBox.BackColor = Settings.Default.ConsoleColor;
            //}

            Debug.WriteLine("Out ConsoleForm_Load()");

        }

		private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("In ConsoleForm_FormClosing()");

            // Need to stop the thread
            // think i will try a better approach

            if (stopped == false)
            {
                workerThread.Abort();
            }

            //// Copy window location to app settings
            //Settings.Default.ConsoleLocation = this.Location;

            //// Copy window size to app settings
            //if (this.WindowState == FormWindowState.Normal)
            //{
            //    Settings.Default.ConsoleSize = this.Size;
            //}
            //else
            //{
            //    Settings.Default.ConsoleSize = this.RestoreBounds.Size;
            //}

            //// Copy console font type to app settings
            //Settings.Default.ConsoleFont = this.consoleTextBox.Font;

            //// Copy console font color to app settings
            //Settings.Default.ConsoleFontColor = this.consoleTextBox.ForeColor;

            //// Copy console color to app settings
            //Settings.Default.ConsoleColor = this.consoleTextBox.BackColor;

            //// Safe Mru
            //SaveFiles();

            //// Save settings
            //Settings.Default.Save();

            //// Upgrade settings
            //Settings.Default.Reload();

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
                Color = this.pictureBox1.BackColor
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color color = colorDialog.Color;
                this.pictureBox1.BackColor = color;
                //Properties.Settings.Default.ConsoleColor = color;
            }
            Debug.WriteLine("Out FileExitMenuItem_Click()");
        }

        private void LoadFiles()
        {
            Debug.WriteLine("In LoadFiles()");
            //TraceInternal.TraceVerbose("Files " + Properties.Settings.Default.FileCount);
            for (int i = 0; i < 4; i++)
            {
                string property = "File" + (i + 1);
                string file = (string)Properties.Settings.Default[property];
                if (file != "")
                {
                    //mruMenu.AddFile(file);
                    TraceInternal.TraceVerbose("Load " + file);
                }
            }
            Debug.WriteLine("Out LoadFiles()");
        }

        public void SaveFiles()
        {
            Debug.WriteLine("In SaveFiles");
            string[] files = null; // = mruMenu.GetFiles();
            Properties.Settings.Default["FileCount"] = files.Length;
            TraceInternal.TraceVerbose("Files=" + files.Length);
            for (int i=0; i < 4; i++)
            {
                string property = "File" + (i + 1);
                if (i < files.Length)
                {
                    Properties.Settings.Default[property] = files[i];
                    TraceInternal.TraceVerbose("Save " + property + "="+ files[i]);
                }
                else
                {
                    Properties.Settings.Default[property] = "";
                    TraceInternal.TraceVerbose("Save " + property + "=");
                }
            }
            Debug.WriteLine("Out SaveFiles");
        }

    }
}

