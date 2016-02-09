using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_r16_imm16(byte regcode)   // opcode: 0xb8+ rw
        {
            ushort src = GetNextInstructionWord();
            SetReg16(regcode, src);
        }
    }
}