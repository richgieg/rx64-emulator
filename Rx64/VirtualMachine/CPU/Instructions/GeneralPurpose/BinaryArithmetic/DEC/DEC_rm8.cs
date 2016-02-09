using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void DEC_rm8()   // opcode: FE /1
        {
            ModRM_Byte ModRM = GetModRM();

            byte result;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = DecByteAndSetFlags(result);
                SetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg8(ModRM.RegMemValue);
                result = DecByteAndSetFlags(result);
                SetReg8(ModRM.RegMemValue, result);
            }
        }
    }
}