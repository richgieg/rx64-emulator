using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rx64
{
    public class MemoryController
    {
        private List<MemoryModule> memory_modules;
        private ulong[] memory_module_base_addresses;
        private ulong bytesAvailable;
        public ulong BytesAvailable
        {
            get { return bytesAvailable; }
        }

        public ulong MegabytesAvailable
        {
            get { return bytesAvailable / (1024 * 1024); }
        }

        public uint[] MemoryModuleSizes
        {
            get
            {
                uint[] memoryModuleSizes = new uint[memory_modules.Count];
                for (int i = 0; i < memory_modules.Count; i++)
                {
                    memoryModuleSizes[i] = memory_modules[i].Size;
                }
                return memoryModuleSizes;
            }
        }

        public MemoryController(List<MemoryModule> MemoryModules)
        {
            memory_modules = MemoryModules;
            memory_module_base_addresses = new ulong[memory_modules.Count];

            for (int i = 0; i < memory_modules.Count; i++)
            {
                zeroMemoryModule(memory_modules[i]);
                bytesAvailable += memory_modules[i].Size;
                if (i == 0)
                {
                    memory_module_base_addresses[i] = 0;
                }
                else
                {
                    memory_module_base_addresses[i] = memory_module_base_addresses[i - 1] + memory_modules[i - 1].Size;
                }
            }
        }

        public byte GetByte(ulong address)
        {
            if (address < bytesAvailable)
            {
                for (int i = 0; i < memory_modules.Count; i++)
                {
                    if (address < (memory_module_base_addresses[i] + memory_modules[i].Size))
                    {
                        return memory_modules[i].GetByte(address - memory_module_base_addresses[i]);
                    }
                }
            }
            return 0xff;
        }

        public void SetByte(ulong address, byte value)
        {
            if (address < bytesAvailable)
            {
                for (int i = 0; i < memory_modules.Count; i++)
                {
                    if (address < (memory_module_base_addresses[i] + memory_modules[i].Size))
                    {
                        memory_modules[i].SetByte(address - memory_module_base_addresses[i], value);
                        break;
                    }
                }
            }
        }

        public ushort GetWord(ulong address)
        {
            ushort word = GetByte(address + 1);
            word <<= 8;
            word |= GetByte(address);
            return word;
        }

        public void SetWord(ulong address, ushort value)
        {
            SetByte(address, (byte)value);
            address++;
            SetByte(address, (byte)(value >>= 8));
        }

        public uint GetDword(ulong address)
        {
            uint dword = GetByte(address + 3);
            for (int i = 2; i >= 0; i--)
            {
                dword <<= 8;
                dword |= GetByte(address + (ulong)i);
            }
            return dword;
        }

        public void SetDword(ulong address, uint value)
        {
            for (ulong i = 0; i < 4; i++)
            {
                SetByte(address + i, (byte)value);
                value >>= 8;
            }
        }

        public ulong GetQword(ulong address)
        {
            ulong qword = GetByte(address + 7);
            for (int i = 6; i >= 0; i--)
            {
                qword <<= 8;
                qword |= GetByte(address + (ulong)i);
            }
            return qword;
        }

        public void SetQword(ulong address, ulong value)
        {
            for (ulong i = 0; i < 8; i++)
            {
                SetByte(address + i, (byte)value);
                value >>= 8;
            }
        }

        private void zeroMemoryModule(MemoryModule module)
        {
            for (uint i = 0; i < module.Size; i++)
            {
                module.SetByte(i, 0);
            }
        }
    }
}
