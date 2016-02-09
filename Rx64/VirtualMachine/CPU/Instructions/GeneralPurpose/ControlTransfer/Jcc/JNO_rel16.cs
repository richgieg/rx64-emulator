using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNO_rel16()   // opcode: 0F 81 cw
        {
            short rel16 = (short)GetNextInstructionWord();

            if (!GetFlag(FlagsEnum.OF_OverflowFlag))
            {
                InstructionPointerJumpRel16(rel16);
                jumpInstruction = true;
            }
        }
    }
}