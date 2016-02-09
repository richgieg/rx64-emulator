using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_moffs32_EAX()   // opcode: 0xa3
        {
            ulong offset = GetNextInstructionMemoryOffset();

            SetDwordInEffectiveSegment(SegmentEnum.DS, offset, eax);
        }
    }
}