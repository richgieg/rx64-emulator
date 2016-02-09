using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void AND_AL_imm8()   // opcode: 24 ib
        {
            byte imm8 = GetNextInstructionByte();
            al &= imm8;

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByByte(al);
            SetZeroFlagByByte(al);
            SetParityFlagByByte(al);
        }
    }
}