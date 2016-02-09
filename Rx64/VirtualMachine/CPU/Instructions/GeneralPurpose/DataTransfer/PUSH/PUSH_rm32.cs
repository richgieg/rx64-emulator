using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_rm32()   // opcode: FF /6
        {
            ModRM_Byte ModRM = GetModRM();
            uint value;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                value = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                value = GetReg32(ModRM.RegMemValue);
            }

            DecrementStackPointerByDword();

            SetDword(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}