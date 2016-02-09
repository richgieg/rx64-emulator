using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_rm32_r32()   // opcode: 29 /r
        {
            uint result;
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = SubTwoDwordsAndSetFlags(result, GetReg32(ModRM.RegOpValue));
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = SubTwoDwordsAndSetFlags(result, GetReg32(ModRM.RegOpValue));
                SetReg32(ModRM.RegMemValue, result);
            }
        }
    }
}