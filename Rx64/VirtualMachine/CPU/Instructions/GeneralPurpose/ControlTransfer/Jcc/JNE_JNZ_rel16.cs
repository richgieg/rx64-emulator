using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNE_JNZ_rel16()   // opcode: 0F 85 cw
        {
            short rel16 = (short)GetNextInstructionWord();

            if (!GetFlag(FlagsEnum.ZF_ZeroFlag))
            {
                InstructionPointerJumpRel16(rel16);
                jumpInstruction = true;
            }
        }
    }
}