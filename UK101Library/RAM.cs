using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    /// <summary>
    /// General RAM
    /// </summary>
    public class RAM : MemoryBusDevice, IMemoryBusDevice
    {
       
        #region Fields
        #endregion

        #region Constructor

        public RAM()
        {
            _data = new byte[0x8000];
            _readOnly = false;
        }

        #endregion
        #region Properties

        #endregion
        #region Methods

        public bool SetSize(UInt16 newSize)
        {
            bool result = false;
            // Allow max 32 kb RAM:
            if (newSize > 0 && newSize <= 0x8000)
            {
                _endsAt = (UInt16)(_startsAt + newSize - 1);
                _data = new byte[newSize];
                result = true;
            }
            return result;
        }

        public override byte Read()
        {
            if (_address < (_endsAt - ~_startsAt))
            {
                return _data[_address];
            }
            else
            {
                return (byte)(_address & 0x00ff);
            }
        }

        public override void Write(byte InData)
        {
            _data[_address] = InData;
        }

        #endregion
    }
}
