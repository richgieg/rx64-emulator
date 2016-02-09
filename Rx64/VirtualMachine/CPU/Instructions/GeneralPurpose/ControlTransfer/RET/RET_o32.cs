using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void RET_o32()   // opcode: C3
        {
            uint new_eip = GetDword(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByDword();

            SetInstructionPointer(new_eip);
            jumpInstruction = true;
        }
    }
}