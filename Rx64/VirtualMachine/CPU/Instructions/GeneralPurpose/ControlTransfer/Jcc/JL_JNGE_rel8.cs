using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JL_JNGE_rel8()   // opcode: 7C cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (GetFlag(FlagsEnum.SF_SignFlag) != GetFlag(FlagsEnum.OF_OverflowFlag))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}