using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Rx64
{
    public class VideoController
    {
        private Label textModeScreen;
        private Thread refreshThread;
        private MemoryController mch;

        public VideoController(MemoryController MCH, Label TextModeScreen)
        {
            mch = MCH;
            textModeScreen = TextModeScreen;
            refreshThread = new Thread(refresh);
            refreshThread.Start();
        }

        public void Kill()
        {
            refreshThread.Abort();
        }

        private void refresh()
        {
            char character;
            char[] line;
            char[][] lines = new char[25][];
            uint offset;

            while (true)
            {
                for (uint i = 0; i < 25; i++)
                {
                    offset = (i * 80) + 0x000B8000;
                    line = new char[80];

                    for (uint j = 0; j < 80; j++)
                    {
                        character = (char)mch.GetByte(offset + j);
                        if ((character >= 0x20) && (character <= 0x7e))
                            line[j] = character;
                        else
                            line[j] = (char)0x20;
                    }
                    lines[i] = line;
                }

                string screen_chars = "";
                foreach (char[] l in lines)
                {
                    screen_chars += new string(l) + "\n";
                }

                textModeScreen.Text = screen_chars;
                Thread.Sleep(16);
            }
        }
    }
}
