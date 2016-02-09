using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_rm16()   // opcode: FF /2
        {
            ModRM_Byte ModRM = GetModRM();
            ushort new_ip;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                new_ip = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                new_ip = GetReg16(ModRM.RegMemValue);

            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), (ushort)GetTempInstructionPointer());

            SetInstructionPointer(new_ip);
            jumpInstruction = true;
        }
    }
}