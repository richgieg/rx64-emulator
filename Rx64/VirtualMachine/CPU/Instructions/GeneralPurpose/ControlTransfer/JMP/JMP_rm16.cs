using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_rm16()   // opcode: FF /4
        {
            ModRM_Byte ModRM = GetModRM();
            ushort new_ip;

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
                new_ip = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
            else
                new_ip = GetReg16(ModRM.RegMemValue);

            SetInstructionPointer(new_ip);
            jumpInstruction = true;
        }
    }
}