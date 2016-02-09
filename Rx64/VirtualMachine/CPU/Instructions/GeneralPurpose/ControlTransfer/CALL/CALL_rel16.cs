using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_rel16()   // opcode: E8 cw
        {
            short rel16 = (short)GetNextInstructionWord();

            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), (ushort)GetTempInstructionPointer());

            InstructionPointerJumpRel16(rel16);
            jumpInstruction = true;
        }
    }
}