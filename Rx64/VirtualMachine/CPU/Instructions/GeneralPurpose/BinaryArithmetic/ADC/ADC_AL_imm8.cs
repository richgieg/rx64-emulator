using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADC_AL_imm8()   // opcode: 14 ib
        {
            byte imm8 = GetNextInstructionByte();
            al = AddTwoBytesWithCarryAndSetFlags(al, imm8);
        }
    }
}
