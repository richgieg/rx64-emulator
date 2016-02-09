using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNP_JPO_rel32()   // opcode: 0F 8B cw
        {
            int rel32 = (int)GetNextInstructionDword();

            if (!GetFlag(FlagsEnum.PF_ParityFlag))
            {
                InstructionPointerJumpRel32(rel32);
                jumpInstruction = true;
            }
        }
    }
}