using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SBB_rm32_imm32()   // opcode: 81 /3 id
        {
            ModRM_Byte ModRM = GetModRM();
            uint result;
            uint imm32 = GetNextInstructionDword();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = SubTwoDwordsWithBorrowAndSetFlags(result, imm32);
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = SubTwoDwordsWithBorrowAndSetFlags(result, imm32);
                SetReg32(ModRM.RegMemValue, result);
            }
        }
    }
}