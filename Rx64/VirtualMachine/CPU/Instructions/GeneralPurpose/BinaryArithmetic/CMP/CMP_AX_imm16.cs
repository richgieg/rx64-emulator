using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMP_AX_imm16()   // opcode: 3D iw
        {
            ushort imm16 = GetNextInstructionWord();
            SubTwoWordsAndSetFlags(ax, imm16);
        }
    }
}