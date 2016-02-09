using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm32_imm32()   // opcode: 0xc7 /0
        {
            ModRM_Byte ModRM = GetModRM();

            uint imm32 = GetNextInstructionDword();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, imm32);
            else
                SetReg32(ModRM.RegMemValue, imm32);
        }
    }
}