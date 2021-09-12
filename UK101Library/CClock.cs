using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Threading;
//using Windows.UI.Xaml;

namespace UK101Library
{
    public class CClock
    {
        #region Variable

        private Timer _timer;
        public Int32 ProcessorCycles;
        private MainPage mainPage;
        //private DateTime dt;
        private bool _hold;
        private Object _lockObject = new Object();

        #endregion
        #region Constructor

        public CClock(MainPage mainPage)
        {
            this.mainPage = mainPage;
            _timer = new Timer(Timer_Tick, null, Timeout.Infinite, 1);  // Create the Timer delay starting  
            _timer.Change(0, 1);    // Start the timer and check every 1 milisecond
            ProcessorCycles = 0;
            TimeSpan oneTick = new TimeSpan(100);
        }

        #endregion
        #region Properties

        public Boolean Hold
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
            _timer.Change(0, 1);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, 1);
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
                        ProcessorCycles += mainPage.CSignetic6502.SingleStep();
                    }
                }
                ProcessorCycles -= 20000;
            }
        }

        #endregion

    }
}
