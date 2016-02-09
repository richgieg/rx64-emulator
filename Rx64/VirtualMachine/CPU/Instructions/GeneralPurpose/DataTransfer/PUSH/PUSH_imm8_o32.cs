using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_imm8_o32()   // opcode: 6A ib                 32-bit operand size
        {
            uint value = (uint)((sbyte)GetNextInstructionByte());           // sign-extend the imm8 value to 32 bits

            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}