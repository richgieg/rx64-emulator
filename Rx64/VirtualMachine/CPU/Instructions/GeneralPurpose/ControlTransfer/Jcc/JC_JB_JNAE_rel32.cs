using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JC_JB_JNAE_rel32()   // opcode: 0F 82 cw
        {
            int rel32 = (int)GetNextInstructionDword();

            if (GetFlag(FlagsEnum.CF_CarryFlag))
            {
                InstructionPointerJumpRel32(rel32);
                jumpInstruction = true;
            }
        }
    }
}