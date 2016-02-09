using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JLE_JNG_rel16()   // opcode: 0F 8E cw
        {
            short rel16 = (short)GetNextInstructionWord();

            if (GetFlag(FlagsEnum.ZF_ZeroFlag) || (GetFlag(FlagsEnum.SF_SignFlag) != GetFlag(FlagsEnum.OF_OverflowFlag)))
            {
                InstructionPointerJumpRel16(rel16);
                jumpInstruction = true;
            }
        }
    }
}