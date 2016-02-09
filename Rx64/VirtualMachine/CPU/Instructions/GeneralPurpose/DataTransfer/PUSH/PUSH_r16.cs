using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_r16(byte regcode)   // opcode: 50+rw
        {
            ushort value;
            value = GetReg16(regcode);

            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}