using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void ADD_AX_imm16()   // opcode: 05 iw
        {
            ushort imm16 = GetNextInstructionWord();
            ax = AddTwoWordsAndSetFlags(ax, imm16);
        }
    }
}