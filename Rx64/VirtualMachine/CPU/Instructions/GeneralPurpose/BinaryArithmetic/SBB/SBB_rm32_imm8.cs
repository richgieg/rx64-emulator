﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SBB_rm32_imm8()   // opcode: 83 /3 ib
        {
            ModRM_Byte ModRM = GetModRM();

            uint result;
            sbyte imm8 = (sbyte)GetNextInstructionByte();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                result = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                result = SubTwoDwordsWithBorrowAndSetFlags(result, (uint)imm8);
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, result);
            }
            else
            {
                result = GetReg32(ModRM.RegMemValue);
                result = SubTwoDwordsWithBorrowAndSetFlags(result, (uint)imm8);
                SetReg32(ModRM.RegMemValue, result);
            }
        }
    }
}