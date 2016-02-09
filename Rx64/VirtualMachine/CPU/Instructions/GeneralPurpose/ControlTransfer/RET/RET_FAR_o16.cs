using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void RET_FAR_o16()   // opcode: CB
        {
            ushort new_ip = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByWord();
            cs = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByWord();

            SetInstructionPointer(new_ip);
            jumpInstruction = true;
        }
    }
}