using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_r32_rm32()   // opcode: 2B /r
        {
            ModRM_Byte ModRM = GetModRM();
            uint rm32;
            uint r32 = GetReg32(ModRM.RegOpValue);

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm32 = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm32 = GetReg32(ModRM.RegMemValue);

            uint result = SubTwoDwordsAndSetFlags(r32, rm32);
            SetReg32(ModRM.RegOpValue, result);
        }
    }
}