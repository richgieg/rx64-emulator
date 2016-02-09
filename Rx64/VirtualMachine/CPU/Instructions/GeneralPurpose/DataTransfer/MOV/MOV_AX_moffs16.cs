using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_AX_moffs16()   // opcode: 0xa1
        {
            ulong offset = GetNextInstructionMemoryOffset();

            ax = GetWordInEffectiveSegment(SegmentEnum.DS, offset);
        }
    }
}