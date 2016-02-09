using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm8_imm8()   // opcode: 0xc6 /0
        {
            ModRM_Byte ModRM = GetModRM();

            byte imm8 = GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, imm8);
            else
                SetReg8(ModRM.RegMemValue, imm8);
        }
    }
}