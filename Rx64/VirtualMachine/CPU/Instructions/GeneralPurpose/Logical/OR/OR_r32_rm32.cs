using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void OR_r32_rm32()   // opcode: 0B /r
        {
            uint result;
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result |= GetReg32(ModRM.RegOpValue);
                SetReg32(ModRM.RegOpValue, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result |= GetReg32(ModRM.RegOpValue);
                SetReg32(ModRM.RegOpValue, result);
            }

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);
        }
    }
}