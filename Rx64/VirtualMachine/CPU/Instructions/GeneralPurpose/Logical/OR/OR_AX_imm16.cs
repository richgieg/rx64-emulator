using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void OR_AX_imm16()   // opcode: 0D iw      
        {
            ushort imm16 = GetNextInstructionWord();
            ax |= imm16;

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByWord(ax);
            SetZeroFlagByWord(ax);
            SetParityFlagByWord(ax);
        }
    }
}