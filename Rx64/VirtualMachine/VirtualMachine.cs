using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Rx64
{
    public class VirtualMachine
    {
        public CPU cpu0;
        public ProgrammableInterruptController pic;
        public MemoryController mch;
        public VideoController vid0;
        public List<MemoryModule> memory_modules;
        
        public VirtualMachine(Label textModeScreen)
        {
            memory_modules = new List<MemoryModule>();
            memory_modules.Add(new MemoryModule(32));
            memory_modules.Add(new MemoryModule(32));
            memory_modules.Add(new MemoryModule(32));
            memory_modules.Add(new MemoryModule(32));

            mch = new MemoryController(memory_modules);

            
            // load BIOS image to RAM at 0xf000
            //byte[] BIOS_bytes;
            //if (File.Exists("bios.img"))
            //{
            //    BIOS_bytes = File.ReadAllBytes("bios.img");
            //    for (uint i = 0; i < BIOS_bytes.Length; i++)
            //    {
            //        mch.SetByte(0xf000 + i, BIOS_bytes[i]);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("BIOS image is missing!");
            //}


            // load boot sector image to RAM at 0x7c00
            byte[] bootsect_bytes;
            if (File.Exists("bootsect.bin"))
            {
                bootsect_bytes = File.ReadAllBytes("bootsect.bin");
                for (uint i = 0; i < bootsect_bytes.Length; i++)
                {
                    mch.SetByte(0x7c00 + i, bootsect_bytes[i]);
                }
            }
            else
            {
                MessageBox.Show("Boot sector is missing!");
            }


            cpu0 = new CPU(mch, 0x7c00);
            pic = new ProgrammableInterruptController(cpu0);

            vid0 = new VideoController(mch, textModeScreen);
        }

        public void PowerOff()
        {
            cpu0.Kill();
            vid0.Kill();
        }

        public void WriteMemoryDump(ulong StartAddress, ulong EndAddress, string OutputFile)
        {
            FileStream f = File.Open(OutputFile, FileMode.Create);

            if (EndAddress > StartAddress)
            {
                for (ulong i = StartAddress; i < EndAddress; i++)
                {
                    f.WriteByte(mch.GetByte(i));
                }
            }

            f.Close();
        }
    }
}
