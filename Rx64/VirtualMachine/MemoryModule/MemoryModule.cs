using System;
using System.Collections.Generic;
using System.Text;

namespace Rx64
{
    public class MemoryModule
    {
        private byte[] bytes;
        private uint size;
        public uint Size
        {
            get { return size; }
        }

        public MemoryModule(uint SizeInMegabytes)
        {
            size = SizeInMegabytes * 1024 * 1024;
            bytes = new byte[size];
        }

        public byte GetByte(ulong address)
        {
            return bytes[address];
        }

        public void SetByte(ulong address, byte value)
        {
            bytes[address] = value;
        }
    }
}
