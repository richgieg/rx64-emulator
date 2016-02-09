using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_ptr16_16()   // opcode: 9A cd
        {
            ushort new_ip = GetNextInstructionWord();
            ushort new_cs = GetNextInstructionWord();

            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), cs);
            DecrementStackPointerByWord();
            SetWord(SegmentEnum.SS, GetStackPointer(), (ushort)GetTempInstructionPointer());

            cs = new_cs;            
            SetInstructionPointer(new_ip);
            
            jumpInstruction = true;
        }
    }
}