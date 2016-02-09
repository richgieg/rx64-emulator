using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JA_JNBE_rel8()   // opcode: 77 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if ((GetFlag(FlagsEnum.CF_CarryFlag) == false) && (GetFlag(FlagsEnum.ZF_ZeroFlag) == false))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}