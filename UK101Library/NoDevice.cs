﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UK101Library
{
    public class NoDevice : MemoryBusDevice, IMemoryBusDevice
    {
        #region Methods
        public override byte Read()
        {
            return (byte)(Address / 256);
        }
        #endregion
    }
}
