using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_AL_moffs8()   // opcode: 0xa0
        {
            ulong offset = GetNextInstructionMemoryOffset();

            al = GetByteInEffectiveSegment(SegmentEnum.DS, offset);
        }
    }
}