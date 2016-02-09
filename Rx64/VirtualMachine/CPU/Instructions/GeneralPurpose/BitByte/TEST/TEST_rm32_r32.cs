using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void TEST_rm32_r32()   // opcode: 85 /r
        {
            ModRM_Byte ModRM = GetModRM();

            uint result;
            uint rm32;
            uint r32;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                rm32 = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                rm32 = GetReg32(ModRM.RegMemValue);
            }

            r32 = GetReg32(ModRM.RegOpValue);
            result = (rm32 & r32);

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);
        }
    }
}