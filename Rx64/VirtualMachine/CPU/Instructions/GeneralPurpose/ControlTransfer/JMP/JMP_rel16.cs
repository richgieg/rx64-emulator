using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_rel16()   // opcode: 0xe9 cw
        {
            short rel16 = (short)GetNextInstructionWord();

            InstructionPointerJumpRel16(rel16);

            jumpInstruction = true;
        }
    }
}