using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void OR_AL_imm8()   // opcode: 0C ib
        {
            byte imm8 = GetNextInstructionByte();
            al |= imm8;

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByByte(al);
            SetZeroFlagByByte(al);
            SetParityFlagByByte(al);
        }
    }
}