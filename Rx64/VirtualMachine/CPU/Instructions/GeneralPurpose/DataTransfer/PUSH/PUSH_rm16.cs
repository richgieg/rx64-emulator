using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void PUSH_rm16()   // opcode: FF /6
        {
            ModRM_Byte ModRM = GetModRM();
            ushort value;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                value = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                value = GetReg16(ModRM.RegMemValue);
            }

            DecrementStackPointerByWord();

            SetWord(SegmentEnum.SS, GetStackPointer(), value);
        }
    }
}