using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_r8_rm8()   // opcode: 3A /r
        {
            ModRM_Byte ModRM = GetModRM();
            byte rm8;
            byte r8 = GetReg8(ModRM.RegOpValue);

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm8 = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm8 = GetReg8(ModRM.RegMemValue);

            SubTwoBytesAndSetFlags(r8, rm8);
        }
    }
}