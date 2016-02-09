using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_moffs16_AX()   // opcode: 0xa3
        {
            ulong offset = GetNextInstructionMemoryOffset();

            SetWordInEffectiveSegment(SegmentEnum.DS, offset, ax);
        }
    }
}