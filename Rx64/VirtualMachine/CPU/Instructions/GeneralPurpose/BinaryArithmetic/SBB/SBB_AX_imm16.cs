using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void SBB_AX_imm16()   // opcode: 1D iw
        {
            ushort imm16 = GetNextInstructionWord();
            ax = SubTwoWordsWithBorrowAndSetFlags(ax, imm16);
        }
    }
}