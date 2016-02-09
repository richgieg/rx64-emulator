using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void TEST_AL_imm8()   // opcode: A8 ib
        {
            byte result = (byte)(al & GetNextInstructionByte());

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);
        }
    }
}