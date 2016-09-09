
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PrivateMessage
{
    class CommandReply : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>
                {
                    "r"
                };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Help
        {
            get { return Plugin.Instance.Translate("whisper_reply_help"); }
        }

        public string Name
        {
            get { return "reply"; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string> {
                    "whisper.reply"
                };
            }
        }

        public string Syntax
        {
            get { return "<message>"; }
        }

        public void Execute(IRocketPlayer caller, string[] args)
        {
            // If command called from console
            if (caller == null)
            {
                Logger.Log("This command can't be used from the console.");
                return;
            }

            if(args.Length >= 1)
            {
                // Get the UnturnedPlayer object from the caller
                UnturnedPlayer fromPlayer = UnturnedPlayer.FromCSteamID((Steamworks.CSteamID)ulong.Parse(caller.Id));
                // Get the player that we should send a reply to
                UnturnedPlayer replyTo = Plugin.Instance.GetPlayerFromLastMessage(fromPlayer);
                
                // If no player has sent a private message to command caller
                if(replyTo == null)
                {
                    UnturnedChat.Say(fromPlayer, Plugin.Instance.Translate("whisper_reply_no_last_player"), Color.red);
                    return;
                }

                // Create a full string from the arguments passed
                string FullMessage = "";
                for (int i = 0; i < args.Length; i++)
                {
                    // Append a the word and a space to the string
                    string word = args[i];
                    FullMessage += word + " ";
                }

                // Send the PM
                Plugin.Instance.WhisperPlayer(fromPlayer, replyTo, FullMessage);
            }
            else
            {
                UnturnedChat.Say(caller, Plugin.Instance.Translate("whisper_reply_syntax_error"), Color.red);
            }
        }
    }
}
