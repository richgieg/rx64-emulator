using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_ES_o32()   // opcode: 06
        {
            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), es);
        }
    }
}