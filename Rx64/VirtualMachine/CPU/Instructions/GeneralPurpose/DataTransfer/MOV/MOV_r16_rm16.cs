using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_r16_rm16()   // opcode: 0x8b /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetReg16(ModRM.RegOpValue, GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress));
            else
                SetReg16(ModRM.RegOpValue, GetReg16(ModRM.RegMemValue));
        }
    }
}