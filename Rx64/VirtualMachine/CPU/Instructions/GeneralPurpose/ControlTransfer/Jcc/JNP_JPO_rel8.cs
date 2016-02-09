using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNP_JPO_rel8()   // opcode: 7B cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (!GetFlag(FlagsEnum.PF_ParityFlag))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}