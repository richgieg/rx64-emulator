using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JA_JNBE_rel16()   // opcode: 0F 87 cw
        {
            short rel16 = (short)GetNextInstructionWord();

            if ((GetFlag(FlagsEnum.CF_CarryFlag) == false) && (GetFlag(FlagsEnum.ZF_ZeroFlag) == false))
            {
                InstructionPointerJumpRel16(rel16);
                jumpInstruction = true;
            }
        }
    }
}