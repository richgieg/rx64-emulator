using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JO_rel8()   // opcode: 70 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (GetFlag(FlagsEnum.OF_OverflowFlag))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}