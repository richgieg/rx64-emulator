using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_rel32()   // opcode: E8 cd
        {
            int rel32 = (int)GetNextInstructionDword();

            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), (uint)GetTempInstructionPointer());

            InstructionPointerJumpRel32(rel32);
            jumpInstruction = true;
        }
    }
}