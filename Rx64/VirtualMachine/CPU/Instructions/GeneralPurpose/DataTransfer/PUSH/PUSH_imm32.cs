using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_imm32()   // opcode: 68 id
        {
            uint value = GetNextInstructionDword();

            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}