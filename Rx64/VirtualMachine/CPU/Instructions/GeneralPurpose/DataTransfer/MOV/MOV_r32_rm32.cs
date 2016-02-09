using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_r32_rm32()   // opcode: 0x8b /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetReg32(ModRM.RegOpValue, GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress));
            else
                SetReg32(ModRM.RegOpValue, GetReg32(ModRM.RegMemValue));
        }
    }
}