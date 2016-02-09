using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void LOOP_rel8_a32()   // opcode: E2 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            ecx--;

            if (ecx != 0)
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}