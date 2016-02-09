using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_r32(byte regcode)   // opcode: 58+rd
        {
            uint value = GetDword(SegmentEnum.SS, GetStackPointer());

            IncrementStackPointerByDword();

            SetReg32(regcode, value);
        }
    }
}