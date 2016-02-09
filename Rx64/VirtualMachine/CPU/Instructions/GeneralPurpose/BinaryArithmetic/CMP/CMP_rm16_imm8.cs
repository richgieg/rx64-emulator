using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_rm16_imm8()   // opcode: 83 /7 ib
        {
            ModRM_Byte ModRM = GetModRM();
            ushort rm16;
            sbyte imm8 = (sbyte)GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm16 = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm16 = GetReg16(ModRM.RegMemValue);

            SubTwoWordsAndSetFlags(rm16, (ushort)imm8);
        }
    }
}