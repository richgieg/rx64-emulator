using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm8_r8()   // opcode: 0x88 /r
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, GetReg8(ModRM.RegOpValue));
            else
                SetReg8(ModRM.RegMemValue, GetReg8(ModRM.RegOpValue));
        }
    }
}