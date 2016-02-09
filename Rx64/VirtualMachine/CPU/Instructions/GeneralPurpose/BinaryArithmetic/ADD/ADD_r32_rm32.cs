using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADD_r32_rm32()   // opcode: 03 /r
        {
            uint result;
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = AddTwoDwordsAndSetFlags(result, GetReg32(ModRM.RegOpValue));
                SetReg32(ModRM.RegOpValue, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = AddTwoDwordsAndSetFlags(result, GetReg32(ModRM.RegOpValue));
                SetReg32(ModRM.RegOpValue, result);
            }
        }
    }
}