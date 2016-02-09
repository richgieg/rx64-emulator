using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void DEC_rm16()   // opcode: FF /1
        {
            ModRM_Byte ModRM = GetModRM();

            ushort result;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = DecWordAndSetFlags(result);
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg16(ModRM.RegMemValue);
                result = DecWordAndSetFlags(result);
                SetReg16(ModRM.RegMemValue, result);
            }
        }
    }
}