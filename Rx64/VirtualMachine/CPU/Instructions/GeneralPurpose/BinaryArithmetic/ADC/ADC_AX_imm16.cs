using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADC_AX_imm16()   // opcode: 15 iw
        {
            ushort imm16 = GetNextInstructionWord();
            ax = AddTwoWordsWithCarryAndSetFlags(ax, imm16);
        }
    }
}