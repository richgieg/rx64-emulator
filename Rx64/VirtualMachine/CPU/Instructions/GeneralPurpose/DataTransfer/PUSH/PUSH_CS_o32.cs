using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_CS_o32()   // opcode: 0E
        {
            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), cs);
        }
    }
}