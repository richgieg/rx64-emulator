using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_imm8_o16()   // opcode: 6A ib                 16-bit operand size
        {
            ushort value = (ushort)((sbyte)GetNextInstructionByte());           // sign-extend the imm8 value to 16 bits

            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}