using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JNO_rel32()   // opcode: 0F 81 cw
        {
            int rel32 = (int)GetNextInstructionDword();

            if (!GetFlag(FlagsEnum.OF_OverflowFlag))
            {
                InstructionPointerJumpRel32(rel32);
                jumpInstruction = true;
            }
        }
    }
}