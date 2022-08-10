using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    public class MemoryBusDevice
    {
        #region Fields

        public byte[] pData;

        #endregion

        #region Constructor

        public MemoryBusDevice()
        {
            ReadOnly = false;
            WriteOnly = false;
            Accessible = true;
            //StartsAt = new Address();
            //EndsAt = new Address();
            //Address = new Address();
        }

        #endregion
        #region Properies

        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public bool Accessible { get; set; }
        public ushort Address { get; set; }
        public byte Data { get; set; }
        public ushort StartsAt { get; set; }
        public ushort EndsAt { get; set; }

        #endregion
        #region Methods

        public void SetAddress(UInt16 InAddress)
        {
            if (InAddress >= StartsAt && InAddress <= EndsAt)
            {
                Address = InAddress;
            }
        }

        public virtual void Write(byte Data) { }
        public virtual byte Read() { return 0x00; }

        #endregion

    }
}
