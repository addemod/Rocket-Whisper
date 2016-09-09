using Rocket.API;

namespace PrivateMessage
{
    public class WhisperConfiguration : IRocketPluginConfiguration
    {
        public string Color;

        public void LoadDefaults()
        {
            Color = "Green";
        }
    }
}
