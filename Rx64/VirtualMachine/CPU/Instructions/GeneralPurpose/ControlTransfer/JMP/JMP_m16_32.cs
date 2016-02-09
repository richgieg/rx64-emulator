using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_m16_32()   // opcode: FF /5
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                uint new_ip = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                ushort new_cs = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress + 4);
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