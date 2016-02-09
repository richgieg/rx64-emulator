using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_CS_o16()   // opcode: 0E
        {
            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), cs);
        }
    }
}