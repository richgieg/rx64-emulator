using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void RET_o16()   // opcode: C3
        {
            ushort new_ip = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByWord();

            SetInstructionPointer(new_ip);
            jumpInstruction = true;
        }
    }
}