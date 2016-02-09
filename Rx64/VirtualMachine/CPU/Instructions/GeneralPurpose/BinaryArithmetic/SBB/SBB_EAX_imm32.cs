using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SBB_EAX_imm32()   // opcode: 1D id
        {
            uint imm32 = GetNextInstructionDword();
            eax = SubTwoDwordsWithBorrowAndSetFlags(eax, imm32);
        }
    }
}