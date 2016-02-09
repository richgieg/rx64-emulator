using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADD_r8_rm8()   // opcode: 02 /r
        {
            byte result;
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = AddTwoBytesAndSetFlags(result, GetReg8(ModRM.RegOpValue));
                SetReg8(ModRM.RegOpValue, result);
            }
            else
            {
                result = GetReg8(ModRM.RegMemValue);
                result = AddTwoBytesAndSetFlags(result, GetReg8(ModRM.RegOpValue));
                SetReg8(ModRM.RegOpValue, result);
            }
        }
    }
}