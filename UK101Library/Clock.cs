using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Threading;
//using Windows.UI.Xaml;

namespace UK101Library
{
    public class Clock
    {
        #region Variable

        private Timer _timer;
        public Int32 ProcessorCycles;
        private Signetic6502 _signetic6502;
        private bool _hold;
        private Object _lockObject = new Object();

        #endregion
        #region Constructor

        public Clock(Signetic6502 signetic6502)
        {
            _signetic6502 = signetic6502;
            _timer = new Timer(Timer_Tick, null, Timeout.Infinite, 1);  // Create the Timer delay starting  
            ProcessorCycles = 0;
        }

        #endregion
        #region Properties

        public bool Hold
        {
            get
            {
                return (_hold);
            }
            set
            {
                _hold = value;
            }
        }

        #endregion
        #region Methods

        public void Start()
        {
            _timer.Change(0, 10);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, 10);
        }

        #endregion
        #region Events

        private void Timer_Tick(object sender)
        {
            lock (_lockObject)
            {
                while (ProcessorCycles < 20000)
                {
                    if (!_hold)
                    {
                        ProcessorCycles += _signetic6502.SingleStep();
                    }
                }
                ProcessorCycles -= 20000;
            }
        }

        #endregion
    }
}
