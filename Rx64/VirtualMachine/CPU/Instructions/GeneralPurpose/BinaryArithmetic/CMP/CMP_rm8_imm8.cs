using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_rm8_imm8()   // opcode: 80 /7 ib
        {
            ModRM_Byte ModRM = GetModRM();
            byte rm8;
            byte imm8 = GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                rm8 = GetByteInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                rm8 = GetReg8(ModRM.RegMemValue);

            SubTwoBytesAndSetFlags(rm8, imm8);
        }
    }
}