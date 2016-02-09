using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_ES_o16()   // opcode: 07
        {
            ushort value = GetWord(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByWord();
            es = value;
        }
    }
}