using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_DS_o32()   // opcode: 1F
        {
            uint value = GetDword(SegmentEnum.SS, GetStackPointer());
            IncrementStackPointerByDword();
            ds = (ushort)value;
        }
    }
}