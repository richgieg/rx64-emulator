using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_rm32_imm32()   // opcode: 81 /7 ib
        {
            ModRM_Byte ModRM = GetModRM();
            uint rm32;
            uint imm32 = GetNextInstructionDword();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm32 = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm32 = GetReg32(ModRM.RegMemValue);

            SubTwoDwordsAndSetFlags(rm32, imm32);
        }
    }
}