using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_EAX_imm32()   // opcode: 2D id
        {
            uint imm32 = GetNextInstructionDword();
            eax = SubTwoDwordsAndSetFlags(eax, imm32);
        }
    }
}