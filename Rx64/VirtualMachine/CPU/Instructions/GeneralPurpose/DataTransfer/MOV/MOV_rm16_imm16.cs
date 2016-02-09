using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void MOV_rm16_imm16()   // opcode: 0xc7 /0
        {
            ModRM_Byte ModRM = GetModRM();

            ushort imm16 = GetNextInstructionWord();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, imm16);
            else
                SetReg16(ModRM.RegMemValue, imm16);
        }
    }
}