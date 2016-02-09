using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_r32_imm32(byte regcode)   // opcode: 0xb8+ rd
        {
            uint src = GetNextInstructionDword();
            SetReg32(regcode, src);
        }
    }
}