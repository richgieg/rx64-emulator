using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void DEC_r16(byte regcode)   // opcode: 48+rw
        {
            ushort value;
            value = DecWordAndSetFlags(GetReg16(regcode));
            SetReg16(regcode, value);
        }
    }
}