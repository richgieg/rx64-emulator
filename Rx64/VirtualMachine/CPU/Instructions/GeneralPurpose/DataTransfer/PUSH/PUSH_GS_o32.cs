using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_GS_o32()   // opcode: 0F A8
        {
            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), gs);
        }
    }
}