using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void JMP_ptr16_16()   // opcode: EA cd
        {
            ushort new_ip = GetNextInstructionWord();
            ushort new_cs = GetNextInstructionWord();

            cs = new_cs;            
            SetInstructionPointer(new_ip);
            
            jumpInstruction = true;
        }
    }
}