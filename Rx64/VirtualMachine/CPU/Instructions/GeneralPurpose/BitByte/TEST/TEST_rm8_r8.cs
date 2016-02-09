using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void TEST_rm8_r8()   // opcode: 84 /r
        {
            ModRM_Byte ModRM = GetModRM();

            byte result;
            byte rm8;
            byte r8;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                rm8 = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                rm8 = GetReg8(ModRM.RegMemValue);
            }

            r8 = GetReg8(ModRM.RegOpValue);
            result = (byte)(rm8 & r8);

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);
        }
    }
}