using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JLE_JNG_rel8()   // opcode: 7E cb
        {
            sbyte rel8 = (sbyte)GetNextInstructionByte();

            if (GetFlag(FlagsEnum.ZF_ZeroFlag) || (GetFlag(FlagsEnum.SF_SignFlag) != GetFlag(FlagsEnum.OF_OverflowFlag)))
            {
                InstructionPointerJumpShort(rel8);
                jumpInstruction = true;
            }
        }
    }
}