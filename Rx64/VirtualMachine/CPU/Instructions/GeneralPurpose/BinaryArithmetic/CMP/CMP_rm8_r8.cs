using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_rm8_r8()   // opcode: 38 /r
        {
            ModRM_Byte ModRM = GetModRM();
            byte rm8;
            byte r8 = GetReg8(ModRM.RegOpValue);

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm8 = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm8 = GetReg8(ModRM.RegMemValue);

            SubTwoBytesAndSetFlags(rm8, r8);
        }
    }
}