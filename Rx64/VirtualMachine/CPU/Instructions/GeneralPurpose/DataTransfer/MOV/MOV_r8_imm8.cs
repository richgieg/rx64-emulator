using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_r8_imm8(byte regcode)   // opcode: 0xb0+ rb
        {
            byte src = GetNextInstructionByte();
            SetReg8(regcode, src);
        }
    }
}