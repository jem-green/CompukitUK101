using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UK101Library
{
    internal interface IMemoryBusDevice
    {
        #region Properties
        bool ReadOnly { get; set; }

        bool WriteOnly { get; set; }

        bool Accessible { get; set; }

        ushort Address { get; set; }

        byte[] Data { get; set; }

        ushort StartsAt { get; set; }

        ushort EndsAt { get; set; }

        #endregion
        #region Methods

        byte Read();
        void Write(byte value);

        #endregion
    }
}
