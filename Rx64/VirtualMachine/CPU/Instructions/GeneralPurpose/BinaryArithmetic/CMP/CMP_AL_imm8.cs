using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_AL_imm8()   // opcode: 3C ib
        {
            byte imm8 = GetNextInstructionByte();
            SubTwoBytesAndSetFlags(al, imm8);
        }
    }
}