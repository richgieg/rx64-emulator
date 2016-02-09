using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_Sreg_rm16()   // opcode: 0x8e /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetRegSeg(ModRM.RegOpValue, GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress));
            else
                SetRegSeg(ModRM.RegOpValue, GetReg16(ModRM.RegMemValue));
        }
    }
}