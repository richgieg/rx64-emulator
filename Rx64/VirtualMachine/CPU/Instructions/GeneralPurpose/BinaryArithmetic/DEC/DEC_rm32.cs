using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void DEC_rm32()   // opcode: FF /1
        {
            ModRM_Byte ModRM = GetModRM();

            uint result;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = DecDwordAndSetFlags(result);
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = DecDwordAndSetFlags(result);
                SetReg32(ModRM.RegMemValue, result);
            }
        }
    }
}