using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_r16(byte regcode)   // opcode: 58+rw
        {
            ushort value = GetWord(SegmentEnum.SS, GetStackPointer());

            IncrementStackPointerByWord();

            SetReg16(regcode, value);
        }
    }
}