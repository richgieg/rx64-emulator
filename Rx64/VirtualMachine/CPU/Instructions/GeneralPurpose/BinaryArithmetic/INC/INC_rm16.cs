using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void INC_rm16()   // opcode: FF /0
        {
            ModRM_Byte ModRM = GetModRM();

            ushort result;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = IncWordAndSetFlags(result);
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg16(ModRM.RegMemValue);
                result = IncWordAndSetFlags(result);
                SetReg16(ModRM.RegMemValue, result);
            }
        }
    }
}