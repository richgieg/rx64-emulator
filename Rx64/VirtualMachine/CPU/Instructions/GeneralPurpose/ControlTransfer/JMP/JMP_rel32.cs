using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_rel32()   // opcode: 0xe9 cd
        {
            int rel32 = (int)GetNextInstructionDword();

            InstructionPointerJumpRel32(rel32);

            jumpInstruction = true;
        }
    }
}