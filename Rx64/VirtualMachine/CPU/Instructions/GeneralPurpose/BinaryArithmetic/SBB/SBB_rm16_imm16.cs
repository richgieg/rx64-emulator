using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SBB_rm16_imm16()   // opcode: 81 /3 iw
        {
            ModRM_Byte ModRM = GetModRM();
            ushort result;
            ushort imm16 = GetNextInstructionWord();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = SubTwoWordsWithBorrowAndSetFlags(result, imm16);
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg16(ModRM.RegMemValue);
                result = SubTwoWordsWithBorrowAndSetFlags(result, imm16);
                SetReg16(ModRM.RegMemValue, result);
            }
        }
    }
}