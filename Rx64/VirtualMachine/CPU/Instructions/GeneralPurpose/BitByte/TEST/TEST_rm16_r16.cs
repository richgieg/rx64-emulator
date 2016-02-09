using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void TEST_rm16_r16()   // opcode: 85 /r
        {
            ModRM_Byte ModRM = GetModRM();

            ushort result;
            ushort rm16;
            ushort r16;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                rm16 = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                rm16 = GetReg16(ModRM.RegMemValue);
            }

            r16 = GetReg16(ModRM.RegOpValue);
            result = (ushort)(rm16 & r16);

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);
        }
    }
}