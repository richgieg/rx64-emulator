using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CLI()   // opcode: 0xfa
        {
            ClearFlag(FlagsEnum.IF_InterruptEnableFlag);
        }
    }
}