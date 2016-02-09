using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_r8_rm8()   // opcode: 0x8A /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetReg8(ModRM.RegOpValue, GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress));
            else
                SetReg8(ModRM.RegOpValue, GetReg8(ModRM.RegMemValue));
        }
    }
}