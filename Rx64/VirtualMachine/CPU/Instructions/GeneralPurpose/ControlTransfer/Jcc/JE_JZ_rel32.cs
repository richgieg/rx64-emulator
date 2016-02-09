using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JE_JZ_rel32()   // opcode: 0F 84 cw
        {
            int rel32 = (int)GetNextInstructionDword();

            if (GetFlag(FlagsEnum.ZF_ZeroFlag))
            {
                InstructionPointerJumpRel32(rel32);
                jumpInstruction = true;
            }
        }
    }
}