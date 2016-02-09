using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CMC()   // opcode: 0xf5
        {
            ComplementFlag(FlagsEnum.CF_CarryFlag);
        }
    }
}