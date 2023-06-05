using System;
using System.Media;
using System.Windows;

namespace TriggerDiscipline
{
    public class SoundManager
    {
        public SoundPlayer clickPlayer = new(Application.GetResourceStream(new Uri("/resources/sounds/click.wav", UriKind.Relative)).Stream);
        public SoundPlayer missPlayer = new(Application.GetResourceStream(new Uri("/resources/sounds/miss.wav", UriKind.Relative)).Stream);
        public static SoundManager instance;
        public SoundManager()
        {
            instance = this;
            clickPlayer.Load();
            missPlayer.Load();
        }
    }
}
