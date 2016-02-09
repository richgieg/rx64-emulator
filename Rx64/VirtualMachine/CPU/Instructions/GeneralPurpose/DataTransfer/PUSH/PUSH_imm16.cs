using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_imm16()   // opcode: 68 iw
        {
            ushort value = GetNextInstructionWord();

            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}