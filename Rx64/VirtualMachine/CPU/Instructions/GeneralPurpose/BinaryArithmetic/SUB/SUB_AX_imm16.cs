using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SUB_AX_imm16()   // opcode: 2D iw
        {
            ushort imm16 = GetNextInstructionWord();
            ax = SubTwoWordsAndSetFlags(ax, imm16);
        }
    }
}