using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADC_EAX_imm32()   // opcode: 15 id
        {
            uint imm32 = GetNextInstructionDword();
            eax = AddTwoDwordsWithCarryAndSetFlags(eax, imm32);
        }
    }
}