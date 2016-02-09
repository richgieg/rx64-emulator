using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_rm32()   // opcode: FF /2
        {
            ModRM_Byte ModRM = GetModRM();
            uint new_ip;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                new_ip = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                new_ip = GetReg32(ModRM.RegMemValue);

            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), (uint)GetTempInstructionPointer());

            SetInstructionPointer(new_ip);
            jumpInstruction = true;
        }
    }
}