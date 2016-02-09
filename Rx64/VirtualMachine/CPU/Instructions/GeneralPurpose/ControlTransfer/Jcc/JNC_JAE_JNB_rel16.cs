using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNC_JAE_JNB_rel16()   // opcode: 0F 83 cw
        {
            short rel16 = (short)GetNextInstructionWord();

            if (!GetFlag(FlagsEnum.CF_CarryFlag))
            {
                InstructionPointerJumpRel16(rel16);
                jumpInstruction = true;
            }
        }
    }
}