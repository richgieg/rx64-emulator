using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_rel8()   // opcode: 0xeb cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            InstructionPointerJumpShort(rel8);

            jumpInstruction = true;
        }
    }
}