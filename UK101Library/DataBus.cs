using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace UK101Library
{
    public class DataBus
    {
        #region Fields

        private byte _deviceIndex;
        private MemoryBusDevice[] _devices;

        #endregion
        #region Constructors

        public DataBus(MemoryBusDevice[] devices)
        {
            _devices = devices;
        }

        #endregion
        #region Properties

        public byte Index
        {
            get
            {
                return (_deviceIndex);
            }
            set
            {
                _deviceIndex = value;
            }
        }

        #endregion

        #region Methods

        public void Write(byte Data)
        {
            _devices[_deviceIndex].Write(Data);
        }

        public byte Read()
        {
            return (_devices[_deviceIndex].Read());
        }

        #endregion Methods

    }
}
