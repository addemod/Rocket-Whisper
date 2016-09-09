using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace PrivateMessage
{
    class CommandPrivateMessage : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>
                {
                    "w",
                    "msg",
                    "pm"
                };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public string Help
        {
            get { return PrivateMessage.Instance.Translate("whisper_help"); }
        }

        public string Name
        {
            get { return "whisper"; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string> {
                    "whisper.message"
                };
            }
        }

        public string Syntax
        {
            get { return "<player> <message>"; }
        }

        public void Execute(IRocketPlayer caller, string[] args)
        {
            // Check and store who called the command
            string CalledFrom = "console";
            if (caller is UnturnedPlayer)
            {
                CalledFrom = "player";
            }
            else
            {
                // Can't send PM from console (yet)
                Logger.Log("Can't send PM via console (yet!)");
                return;
            }


            // If there's at least two arguments to the command (<player> <message>
            if (args.Length >= 2)
            {
                UnturnedPlayer toPlayer;

                // Get the player from the first argument
                if(!((toPlayer = UnturnedPlayer.FromName(args[0])) is UnturnedPlayer))
                {
                    if(CalledFrom == "console")
                    {
                        Logger.Log(string.Format(PrivateMessage.Instance.Translate("whisper_player_not_found"), args[0]));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, string.Format(PrivateMessage.Instance.Translate("whisper_player_not_found"), args[0]), Color.red);
                    }
                    return;
                }

                // Initialize the fromPlayer object
                UnturnedPlayer fromPlayer = null;
                if (CalledFrom == "player")
                {
                    fromPlayer = UnturnedPlayer.FromCSteamID((Steamworks.CSteamID)ulong.Parse(caller.Id));
                }
                
                // If the caller tries to whisper to himself
                if(fromPlayer.CSteamID == toPlayer.CSteamID)
                {
                    UnturnedChat.Say(fromPlayer, PrivateMessage.Instance.Translate("whisper_to_self"), Color.red);
                    return;
                }

                // Create a full string from the arguments passed
                string FullMessage = "";
                for (int i = 0; i < args.Length; i++)
                {
                    // We don't want to include the player name in the message, let's skip that one
                    if(i == 0)
                    {
                        continue;
                    }

                    // Append a the word and a space to the string
                    string word = args[i];
                    FullMessage += word + " ";
                }

                // Whisper the player
                PrivateMessage.Instance.WhisperPlayer(fromPlayer, toPlayer, FullMessage);
            }
            else
            {
                UnturnedChat.Say(caller, PrivateMessage.Instance.Translate("whisper_syntax_error"), Color.red);
            }
        }
    }
}
