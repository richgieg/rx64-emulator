using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_ptr16_32()   // opcode: EA cp
        {
            uint new_ip = GetNextInstructionDword();
            ushort new_cs = GetNextInstructionWord();

            cs = new_cs;            
            SetInstructionPointer(new_ip);
            
            jumpInstruction = true;
        }
    }
}