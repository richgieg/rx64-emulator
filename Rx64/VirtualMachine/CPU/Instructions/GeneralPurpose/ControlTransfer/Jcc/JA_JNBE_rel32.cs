using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JA_JNBE_rel32()   // opcode: 0F 87 cw
        {
            int rel32 = (int)GetNextInstructionDword();

            if ((GetFlag(FlagsEnum.CF_CarryFlag) == false) && (GetFlag(FlagsEnum.ZF_ZeroFlag) == false))
            {
                InstructionPointerJumpRel32(rel32);
                jumpInstruction = true;
            }
        }
    }
}