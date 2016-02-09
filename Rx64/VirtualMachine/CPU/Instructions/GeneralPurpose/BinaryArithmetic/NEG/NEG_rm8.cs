using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void NEG_rm8()   // opcode: F6 /3
        {
            ModRM_Byte ModRM = GetModRM();
            byte result;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = NegByteAndSetFlags(result);
                SetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg8(ModRM.RegMemValue);
                result = NegByteAndSetFlags(result);
                SetReg8(ModRM.RegMemValue, result);
            }
        }
    }
}