using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_EAX_moffs32()   // opcode: 0xa1
        {
            ulong offset = GetNextInstructionMemoryOffset();

            eax = GetDwordInEffectiveSegment(SegmentEnum.DS, offset);
        }
    }
}