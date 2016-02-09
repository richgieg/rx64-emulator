using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void AND_rm8_imm8()   // opcode: 80 /4 ib
        {
            ModRM_Byte ModRM = GetModRM();
            byte result;
            byte imm8 = GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result &= imm8;
                SetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg8(ModRM.RegMemValue);
                result &= imm8;
                SetReg8(ModRM.RegMemValue, result);
            }

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);
        }
    }
}