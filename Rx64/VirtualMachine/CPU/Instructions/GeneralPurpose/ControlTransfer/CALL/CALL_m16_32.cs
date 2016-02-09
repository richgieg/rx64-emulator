using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_m16_32()   // opcode: FF /3
        {
            ModRM_Byte ModRM = GetModRM();

            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                uint new_ip = GetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress);
                ushort new_cs = GetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress + 4);

                DecrementStackPointerByDword();
                SetDword(SegmentEnum.SS, GetStackPointer(), cs);
                DecrementStackPointerByDword();
                SetDword(SegmentEnum.SS, GetStackPointer(), (uint)GetTempInstructionPointer());

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