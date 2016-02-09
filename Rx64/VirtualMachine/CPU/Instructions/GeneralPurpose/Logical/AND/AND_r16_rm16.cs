using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void AND_r16_rm16()   // opcode: 23 /r
        {
            ushort result;
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result &= GetReg16(ModRM.RegOpValue);
                SetReg16(ModRM.RegOpValue, result);
            }
            else
            {
                result = GetReg16(ModRM.RegMemValue);
                result &= GetReg16(ModRM.RegOpValue);
                SetReg16(ModRM.RegOpValue, result);
            }

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);
        }
    }
}