using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_rm16_r16()   // opcode: 39 /r
        {
            ModRM_Byte ModRM = GetModRM();
            ushort rm16;
            ushort r16 = GetReg16(ModRM.RegOpValue);

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm16 = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm16 = GetReg16(ModRM.RegMemValue);

            SubTwoWordsAndSetFlags(rm16, r16);
        }
    }
}