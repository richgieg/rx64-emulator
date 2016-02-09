using System;
using System.Collections.Generic;
using System.Text;

namespace Rx64
{
    // Emulates the 8259A PIC
    public class ProgrammableInterruptController
    {
        private CPU cpu;

        public ProgrammableInterruptController(CPU AttachedCPU)
        {
            cpu = AttachedCPU;
        }
    }
}
