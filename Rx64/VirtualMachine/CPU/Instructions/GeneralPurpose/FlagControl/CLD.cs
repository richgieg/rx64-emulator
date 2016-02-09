using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void CLD()   // opcode: 0xfc
        {
            ClearFlag(FlagsEnum.DF_DirectionFlag);
        }
    }
}