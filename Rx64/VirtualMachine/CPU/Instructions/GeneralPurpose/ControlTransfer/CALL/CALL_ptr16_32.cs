using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CALL_ptr16_32()   // opcode: 9A cp
        {
            uint new_ip = GetNextInstructionDword();
            ushort new_cs = GetNextInstructionWord();

            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), cs);
            DecrementStackPointerByDword();
            SetDword(SegmentEnum.SS, GetStackPointer(), (uint)GetTempInstructionPointer());

            cs = new_cs;            
            SetInstructionPointer(new_ip);
            
            jumpInstruction = true;
        }
    }
}