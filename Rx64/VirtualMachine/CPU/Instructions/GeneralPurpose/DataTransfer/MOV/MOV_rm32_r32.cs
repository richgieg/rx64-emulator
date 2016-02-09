using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm32_r32()   // opcode: 0x89 /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, GetReg32(ModRM.RegOpValue));
            else
                SetReg32(ModRM.RegMemValue, GetReg32(ModRM.RegOpValue));
        }
    }
}