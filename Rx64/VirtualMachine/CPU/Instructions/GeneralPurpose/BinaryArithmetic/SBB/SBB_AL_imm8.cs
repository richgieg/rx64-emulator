using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SBB_AL_imm8()   // opcode: 1C ib
        {
            byte imm8 = GetNextInstructionByte();
            al = SubTwoBytesWithBorrowAndSetFlags(al, imm8);
        }
    }
}