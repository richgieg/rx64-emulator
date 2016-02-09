using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void STC()   // opcode: 0xf9
        {
            SetFlag(FlagsEnum.CF_CarryFlag);
        }
    }
}