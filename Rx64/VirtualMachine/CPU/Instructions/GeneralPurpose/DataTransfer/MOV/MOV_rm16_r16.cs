using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm16_r16()   // opcode: 0x89 /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, GetReg16(ModRM.RegOpValue));
            else
                SetReg16(ModRM.RegMemValue, GetReg16(ModRM.RegOpValue));
        }
    }
}