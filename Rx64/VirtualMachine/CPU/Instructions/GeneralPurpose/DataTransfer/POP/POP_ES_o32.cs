using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_ES_o32()   // opcode: 07
        {
            uint value = GetDword(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByDword();
            es = (ushort)value;
        }
    }
}