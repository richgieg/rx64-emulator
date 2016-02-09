using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void TEST_rm32_imm32()   // opcode: F7 /0 id
        {
            ModRM_Byte ModRM = GetModRM();

            uint result;
            uint rm32;
            uint imm32 = GetNextInstructionDword();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                rm32 = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                rm32 = GetReg32(ModRM.RegMemValue);
            }

            result = rm32 & imm32;

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);
        }
    }
}