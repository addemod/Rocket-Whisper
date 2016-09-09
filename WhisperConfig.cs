using Rocket.API;

namespace PrivateMessage
{
    public class WhisperConfig : IRocketPluginConfiguration
    {
        public string Color;

        public void LoadDefaults()
        {
            Color = "purple";
        }
    }
}
