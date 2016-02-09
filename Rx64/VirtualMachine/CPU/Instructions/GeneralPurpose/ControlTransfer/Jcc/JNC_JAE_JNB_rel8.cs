using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNC_JAE_JNB_rel8()   // opcode: 73 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (!GetFlag(FlagsEnum.CF_CarryFlag))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}