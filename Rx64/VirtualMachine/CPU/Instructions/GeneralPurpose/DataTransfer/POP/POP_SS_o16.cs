using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_SS_o16()   // opcode: 17
        {
            ushort value = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByWord();
            ss = value;
        }
    }
}