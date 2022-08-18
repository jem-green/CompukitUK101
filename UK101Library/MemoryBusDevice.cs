using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    public class MemoryBusDevice
    {
        #region Fields

        protected byte[] _data;
        protected bool _readOnly;
        protected bool _writeOnly;
        protected bool _accessible;
        protected ushort _address;
        protected ushort _startsAt;
        protected ushort _endsAt;

        #endregion
        #region Constructor

        public MemoryBusDevice()
        {
            _readOnly = false;
            _writeOnly = false;
            _accessible = true;
        }

        #endregion
        #region Properies

        public bool ReadOnly
        {
            get
            {
                return (_readOnly);
            }
            set
            {
                _readOnly = value;
            }
        }

        public bool WriteOnly
        {
            get
            {
                return (_writeOnly);
            }
            set
            {
                _writeOnly = value;
            }
        }

        public bool Accessible
        {
            get
            {
                return (_accessible);
            }
            set
            {
                _accessible = value;
            }
        }

        public ushort Address
        {
            get
            {
                return (_address);
            }
            set
            {
                _address = value;
            }
        }

        public byte[] Data
        {
            get
            {
                return (_data);
            }
            set
            {
                _data = value;
            }
        }

        public ushort StartsAt
        {
            get
            {
                return (_startsAt);
            }
            set
            {
                _startsAt = value;
            }
        }

        public ushort EndsAt
        {
            get
            {
                return (_endsAt);
            }
            set
            {
                _endsAt = value;
            }
        }

        #endregion
        #region Methods

        public void SetAddress(UInt16 InAddress)
        {
            if (InAddress >= _startsAt && InAddress <= _endsAt)
            {
                _address = InAddress;
            }
        }

        public virtual void Write(byte Data) { }
        public virtual byte Read() { return 0x00; }

        #endregion

    }
}
