using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void LOOPE_LOOPZ_rel8_a32()   // opcode: E1 cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            ecx--;

            if ((ecx != 0) && (GetFlag(FlagsEnum.ZF_ZeroFlag)))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}