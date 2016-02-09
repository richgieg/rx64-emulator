using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void NEG_rm32()   // opcode: F7 /3
        {
            ModRM_Byte ModRM = GetModRM();
            uint result;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = NegDwordAndSetFlags(result);
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = NegDwordAndSetFlags(result);
                SetReg32(ModRM.RegMemValue, result);
            }
        }
    }
}