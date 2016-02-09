using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_SS_o32()   // opcode: 16
        {
            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), ss);
        }
    }
}