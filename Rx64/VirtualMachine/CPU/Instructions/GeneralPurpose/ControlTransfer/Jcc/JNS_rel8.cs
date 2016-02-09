using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNS_rel8()   // opcode: 79 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (!GetFlag(FlagsEnum.SF_SignFlag))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}