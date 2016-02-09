using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNO_rel8()   // opcode: 71 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (!GetFlag(FlagsEnum.OF_OverflowFlag))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}