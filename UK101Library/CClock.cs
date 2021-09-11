using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
//using Windows.UI.Xaml;

namespace UK101Library
{
    public class CClock
    {
        DispatcherTimer _timer;

        public DispatcherTimer Timer
        {
            get
            {
                return (_timer);
            }
            set
            {
                _timer = value;
            }
        }
        public Boolean Hold { get; set; }

        public Int32 ProcessorCycles;
        private MainPage mainPage;
        //private DateTime dt;

        public CClock(MainPage mainPage)
        {
            this.mainPage = mainPage;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1); // 1 ms
            _timer.Tick += Timer_Tick;
            ProcessorCycles = 0;
            TimeSpan oneTick = new TimeSpan(100);
        }

        #region Events

        private void Timer_Tick(object sender, object e)
        {
            while (ProcessorCycles < 20000)
            {
                if (!Hold)
                {
                    ProcessorCycles += mainPage.CSignetic6502.SingleStep();
                }
            }
            ProcessorCycles -= 20000;
        }

        #endregion

    }
}
