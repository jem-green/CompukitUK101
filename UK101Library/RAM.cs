﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    public class RAM : MemoryBusDevice
    {
        public UInt16 RAMSize { get; set; }

        public byte[] pData;

        public RAM()
        {
            pData = null;
            ReadOnly = false;
        }

        public Boolean SetRamSize(UInt16 newSize)
        {
            bool result = false;

            // Allow max 32 kb RAM:
            if (newSize > 0 && newSize <= 0x8000)
            {
                RAMSize = newSize;
                StartsAt = 0x0000;
                EndsAt = (UInt16)(newSize - 1);
                pData = new byte[newSize];
                result = true;
            }

            return result;
        }

        public override byte Read()
        {
            if (Address < RAMSize)
            {
                return pData[Address];
            }
            else
            {
                return (byte)(Address & 0x00ff);
            }
        }

        public override void Write(byte InData)
        {
            pData[Address] = InData;
        }
    }
}