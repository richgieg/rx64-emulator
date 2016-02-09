using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_GS_o16()   // opcode: 0F A9
        {
            ushort value = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByWord();
            gs = value;
        }
    }
}