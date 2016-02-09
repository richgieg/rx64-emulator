using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_rm32()   // opcode: 8F /0
        {
            uint value = GetDword(SegmentEnum.SS, GetStackPointer());

            // From INTEL manual: "If the ESP register is used as a base register for addressing a destination operande
            // in memory, the POP instruction computes the effective address of the operand after it increments the ESP
            // register.

            IncrementStackPointerByDword();


            // Getting the ModRM byte with this function computes the effective address

            ModRM_Byte ModRM = GetModRM();


            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                SetDwordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, value);
            }
            else
            {
                SetReg32(ModRM.RegMemValue, value);
            }


        }
    }
}