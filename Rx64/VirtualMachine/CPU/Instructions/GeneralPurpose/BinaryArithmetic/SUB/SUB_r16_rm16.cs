using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_r16_rm16()   // opcode: 2B /r
        {
            ModRM_Byte ModRM = GetModRM();
            ushort rm16;
            ushort r16 = GetReg16(ModRM.RegOpValue);

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm16 = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm16 = GetReg16(ModRM.RegMemValue);

            ushort result = SubTwoWordsAndSetFlags(r16, rm16);
            SetReg16(ModRM.RegOpValue, result);
        }
    }
}