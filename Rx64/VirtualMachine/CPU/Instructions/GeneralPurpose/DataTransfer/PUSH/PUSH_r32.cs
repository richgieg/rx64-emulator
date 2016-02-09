using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_r32(byte regcode)   // opcode: 50+rd
        {
            uint value;
            value = GetReg32(regcode);

            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}