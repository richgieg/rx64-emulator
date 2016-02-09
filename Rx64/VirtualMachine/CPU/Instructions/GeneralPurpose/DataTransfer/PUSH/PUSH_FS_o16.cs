using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_FS_o16()   // opcode: 0F A0
        {
            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), fs);
        }
    }
}