using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void OR_rm32_imm8()   // opcode: 83 /1 ib
        {
            ModRM_Byte ModRM = GetModRM();

            uint result;
            uint imm8 = (uint)((sbyte)GetNextInstructionByte());

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result |= imm8;
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result |= imm8;
                SetReg32(ModRM.RegMemValue, result);
            }

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);
        }
    }
}