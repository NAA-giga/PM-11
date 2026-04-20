using System;
using System.Collections.Generic;
using System.Media;
using System.Text;

namespace форма_для_сотрудника_охраны.Helpers
{
    public static class SoundHelper
    {
        public static void PlaySystemSound()
        {
            SystemSounds.Beep.Play();
        }
    }
}
