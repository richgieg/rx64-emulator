using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void TEST_rm8_imm8()   // opcode: F6 /0 ib
        {
            ModRM_Byte ModRM = GetModRM();

            byte result;
            byte rm8;
            byte imm8 = GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                rm8 = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            }
            else
            {
                rm8 = GetReg8(ModRM.RegMemValue);
            }

            result = (byte)(rm8 & imm8);

            ClearFlag(FlagsEnum.OF_OverflowFlag | FlagsEnum.CF_CarryFlag);
            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);
        }
    }
}