using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;
using Rocket.Unturned.Chat;
using Steamworks;

namespace PrivateMessage
{
    public class Plugin : RocketPlugin<WhisperConfig>
    {
        public static Plugin Instance;
        public Dictionary<CSteamID, CSteamID> LastMessageFromPlayer;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    { "whisper_help", "Send a private message (whisper) to a player." },
                    { "whisper_reply_help", "Reply a message to the last player who sent you a private message." },
                    { "whisper_syntax_error", "Syntax: /whisper <player> <message>" },
                    { "whisper_reply_syntax_error", "Syntax: /reply <message>" },
                    { "whisper_received", "[{0}] whispers: {1}" },
                    { "whisper_sent", "To [{0}]: {1}" },
                    { "whisper_player_not_found", "Couldn't find a player named \"{0}\"" },
                    { "whisper_reply_no_last_player", "No one has sent you a private message, you can't reply to no one." },
                    { "whisper_to_self", "You can't send a private message to yourself." }
                };
            }
        }

        protected override void Load()
        {
            Instance = this;
            LastMessageFromPlayer = new Dictionary<CSteamID, CSteamID>();
        }

        protected override void Unload()
        {
            
        }

        public void SetPlayerFromLastMessage(UnturnedPlayer fromPlayer, UnturnedPlayer toPlayer)
        {
            // If the player who sent the message is already in the dictionary
            if (LastMessageFromPlayer.ContainsKey(toPlayer.CSteamID))
            {
                // If sender is console
                if(fromPlayer == null)
                {
                    // If the sender is console we can't set the last player who sent a pm to "toPlayer" as you can't whisper to console.
                    LastMessageFromPlayer.Remove(toPlayer.CSteamID);
                    return;
                }

                // Remove the player who received the message
                LastMessageFromPlayer.Remove(toPlayer.CSteamID);
                // Then add him again and his message partner
                LastMessageFromPlayer.Add(toPlayer.CSteamID, fromPlayer.CSteamID);
            }
            else
            {
                // If sender is console
                if (fromPlayer == null)
                {
                    // If the sender is console we can't set the last player who sent a pm to "toPlayer" as you can't whisper to console.
                    return;
                }
                // Add the player and his message partner
                LastMessageFromPlayer.Add(toPlayer.CSteamID, fromPlayer.CSteamID);
            }
        }

        public UnturnedPlayer GetPlayerFromLastMessage(UnturnedPlayer player)
        {
            CSteamID lastMessageFrom = new CSteamID();

            // If the player who sent the message is in the dictionary
            if (LastMessageFromPlayer.ContainsKey(player.CSteamID))
            {
                // Get the player who last messaged him
                LastMessageFromPlayer.TryGetValue(player.CSteamID, out lastMessageFrom);
            }

            return UnturnedPlayer.FromCSteamID(lastMessageFrom);
        }

        public void WhisperPlayer(UnturnedPlayer fromPlayer, UnturnedPlayer toPlayer, string message)
        {
            string fromPlayerName = null;

            // If the sender was console
            if(fromPlayer == null)
            {
                // The "player name" is now "Server" instead of null
                fromPlayerName = "Server";
            }
            else
            {
                // Set the player name if not console
                fromPlayerName = fromPlayer.DisplayName;
            }

            // Set the last message received from player for the player who received the PM
            SetPlayerFromLastMessage(toPlayer, fromPlayer);

            // Send the PM (The player who receives the PM)
            UnturnedChat.Say(toPlayer, string.Format(Translate("whisper_received"), fromPlayerName, message), UnturnedChat.GetColorFromName(Configuration.Instance.Color, Color.magenta));
            // Inform the sender that the message was sent (will look like a conversation, so you can scroll back to see what you actually wrote)
            UnturnedChat.Say(fromPlayer, string.Format(Translate("whisper_sent"), toPlayer.DisplayName, message), UnturnedChat.GetColorFromName(Configuration.Instance.Color, Color.magenta));
        }
    }
}
