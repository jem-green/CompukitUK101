using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace UK101Library
{
    public class AddressBus
    {
        #region Fields

        //private int _devices = 0;
        private byte[] _map = new byte[0x10000];
        private MemoryBusDevice[] _devices;
        private DataBus _dataBus;

        #endregion
        #region Constructors

        public AddressBus(MemoryBusDevice[] devices, DataBus dataBus)
        {
            _devices = devices;
            _dataBus = dataBus;
        }

        #endregion
        #region Properties

        #endregion
        #region Methods

        /// <summary>
        /// Build the memory map
        /// </summary>
        public void Init()
        {
            /*
             * 0x0000-0x9fff	40KB RAM
             * 0xa000-0xbfff	8KB BASIC in ROM
             * 0xc000-0xcfff	External SRAM (not viewable by BASIC)
             * 0xd000-0xd3ff	1KB Display RAM
             * 0xdc00-0xdfff	Polled Keyboard
             * 0xf000-0xf001	ACIA Serial port
             * 0xf800-0xffff	2KB MONITOR ROM
             */

            for (Int32 address = 0; address < 0x10000; address++)
            {
                for (byte index = 0; index < _devices.Length; index++)
                {
                    MemoryBusDevice device = _devices[index];
                    if ((address >= device.StartsAt) && (address <= device.EndsAt))
                    {
                        _map[address] = index;
                    }
                }
            }
        }

        /// <summary>
        /// Set the device based on the memory map
        /// </summary>
        /// <param name="address"></param>
        public void SetAddress(UInt16 address)
        {
            byte index = _map[address];
            _dataBus.Index = index;
            _devices[index].SetAddress(address);
        }

        #endregion Methods

    }
}
