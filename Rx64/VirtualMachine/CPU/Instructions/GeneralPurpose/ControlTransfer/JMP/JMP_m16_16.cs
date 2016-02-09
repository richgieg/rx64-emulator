using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_m16_16()   // opcode: FF /5
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                ushort new_ip = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                ushort new_cs = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress + 2);
                cs = new_cs;
                SetInstructionPointer(new_ip);
                jumpInstruction = true;
            }
            else
            {
                RaiseException(ExceptionEnum.InvalidOpcode, ExceptionDetailEnum.InvalidModValueInModRM);
            }
        }
    }
}