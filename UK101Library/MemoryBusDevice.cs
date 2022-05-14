using System;
using System.Collections.Generic;
using System.Text;

namespace UK101Library
{
    public class MemoryBusDevice
    {
        public Boolean ReadOnly { get; set; }
        public Boolean WriteOnly { get; set; }
        public Boolean Accessible { get; set; }
        public ushort Address { get; set; }
        public byte Data { get; set; }
        public ushort StartsAt { get; set; }
        public ushort EndsAt { get; set; }

        public MemoryBusDevice()
        {
            ReadOnly = false;
            WriteOnly = false;
            Accessible = true;
            //StartsAt = new Address();
            //EndsAt = new Address();
            //Address = new Address();
        }

        public void SetAddress(UInt16 InAddress)
        {
            if (InAddress >= StartsAt && InAddress <= EndsAt)
            {
                Address = InAddress;
            }
        }

        public virtual void Write(byte Data) { }
        public virtual byte Read() { return 0x00; }
    }
}
