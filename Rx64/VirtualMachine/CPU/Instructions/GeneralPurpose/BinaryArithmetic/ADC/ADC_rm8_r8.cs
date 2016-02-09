using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADC_rm8_r8()   // opcode: 10 /r
        {
            byte result;
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = AddTwoBytesWithCarryAndSetFlags(result, GetReg8(ModRM.RegOpValue));
                SetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg8(ModRM.RegMemValue);
                result = AddTwoBytesWithCarryAndSetFlags(result, GetReg8(ModRM.RegOpValue));
                SetReg8(ModRM.RegMemValue, result);
            }
        }
    }
}