using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADD_AL_imm8()   // opcode: 04 ib
        {
            byte imm8 = GetNextInstructionByte();
            al = AddTwoBytesAndSetFlags(al, imm8);
        }
    }
}