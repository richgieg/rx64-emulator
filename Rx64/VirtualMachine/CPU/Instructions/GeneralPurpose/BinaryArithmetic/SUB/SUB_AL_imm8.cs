using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_AL_imm8()   // opcode: 2C ib
        {
            byte imm8 = GetNextInstructionByte();
            al = SubTwoBytesAndSetFlags(al, imm8);
        }
    }
}