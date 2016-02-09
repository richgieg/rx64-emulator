using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void POP_rm16()   // opcode: 8F /0
        {
            ushort value = GetWord(SegmentEnum.SS, GetStackPointer());
            
            // From INTEL manual: "If the ESP register is used as a base register for addressing a destination operande
            // in memory, the POP instruction computes the effective address of the operand after it increments the ESP
            // register.
            
            IncrementStackPointerByWord();


            // Getting the ModRM byte with this function computes the effective address

            ModRM_Byte ModRM = GetModRM();


            if (ModRM.RegMemType == RegMemTypeEnum.Memory)
            {
                SetWordInEffectiveSegment(ModRM.DefaultSegment, ModRM.EffectiveAddress, value);
            }
            else
            {
                SetReg16(ModRM.RegMemValue, value);
            }

            
        }
    }
}