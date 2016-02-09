using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void RET_FAR_o32()   // opcode: CB
        {
            uint new_eip = GetDword(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByDword();
            cs = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByDword();

            SetInstructionPointer(new_eip);
            jumpInstruction = true;
        }
    }
}