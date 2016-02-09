using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm16_Sreg()   // opcode: 0x8c /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, GetRegSeg(ModRM.RegOpValue));
            else
                SetReg16(ModRM.RegMemValue, GetRegSeg(ModRM.RegOpValue));
        }
    }
}