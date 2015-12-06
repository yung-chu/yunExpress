using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.InteropServices;

namespace LMS.WinForm.Client.Common
{
    public class SoundPaly
    {
        public static uint SND_ASYNC = 0x0001;

        public static uint SND_FILENAME = 0x00020000;

        [DllImport("winmm.dll")]
        public static extern
        uint mciSendString(string lpstrCommand,string lpstrReturnString, uint uReturnLength, uint hWndCallback);

        public void Play(string filePath)
        {
            //mciSendString("setaudio music volume to 255", null, 0, 0);
            mciSendString(@"close temp_alias", null, 0, 0);
            mciSendString("open \"" + filePath + "\" alias temp_alias", null, 0, 0);
            mciSendString("play temp_alias repeat", null, 0, 0);
        }
    }

}
