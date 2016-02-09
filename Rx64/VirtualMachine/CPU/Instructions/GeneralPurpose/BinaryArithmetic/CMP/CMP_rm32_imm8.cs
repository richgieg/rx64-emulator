using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_rm32_imm8()   // opcode: 83 /7 ib
        {
            ModRM_Byte ModRM = GetModRM();
            uint rm32;
            sbyte imm8 = (sbyte)GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm32 = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm32 = GetReg32(ModRM.RegMemValue);

            SubTwoDwordsAndSetFlags(rm32, (uint)imm8);
        }
    }
}