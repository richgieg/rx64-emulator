using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_moffs8_AL()   // opcode: 0xa2
        {
            ulong offset = GetNextInstructionMemoryOffset();

            SetByteInEffectiveSegment(SegmentEnum.DS, offset, al);
        }
    }
}