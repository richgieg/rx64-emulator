using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_rm16_imm8()   // opcode: 83 /5 ib
        {
            ModRM_Byte ModRM = GetModRM();

            ushort result;
            sbyte imm8 = (sbyte)GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = SubTwoWordsAndSetFlags(result, (ushort)imm8);
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg16(ModRM.RegMemValue);
                result = SubTwoWordsAndSetFlags(result, (ushort)imm8);
                SetReg16(ModRM.RegMemValue, result);
            }
        }
    }
}