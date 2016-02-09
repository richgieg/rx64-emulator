using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADD_EAX_imm32()   // opcode: 05 id
        {
            uint imm32 = GetNextInstructionDword();
            eax = AddTwoDwordsAndSetFlags(eax, imm32);
        }
    }
}