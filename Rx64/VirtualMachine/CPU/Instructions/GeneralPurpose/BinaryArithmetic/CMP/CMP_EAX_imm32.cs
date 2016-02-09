using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_EAX_imm32()   // opcode: 3D id
        {
            uint imm32 = GetNextInstructionDword();
            SubTwoDwordsAndSetFlags(eax, imm32);
        }
    }
}