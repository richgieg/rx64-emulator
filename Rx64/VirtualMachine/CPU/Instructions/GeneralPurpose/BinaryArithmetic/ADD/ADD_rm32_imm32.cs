using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADD_rm32_imm32()   // opcode: 81 /0 id
        {
            ModRM_Byte ModRM = GetModRM();
            uint result;
            uint imm32 = GetNextInstructionDword();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = AddTwoDwordsAndSetFlags(result, imm32);
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = AddTwoDwordsAndSetFlags(result, imm32);
                SetReg32(ModRM.RegMemValue, result);
            }
        }
    }
}