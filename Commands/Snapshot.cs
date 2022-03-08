﻿using Discord.Rest;
using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using Mercury.Snapshot.Objects.Structures.Cards;
using Mercury.Snapshot.Objects.Util.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Snapshot.Commands
{
    internal class Snapshot
    {
        [Command(new string[] { "snapshot", "sn", "s", "snap", "shot" })]
        internal static async Task Abc(CommandArguments Args)
        {
            if (Program.Client.GetChannel(942608553028501544) is SocketTextChannel Channel)
            {
                RestUserMessage Message = await Channel.SendMessageAsync("", false, new EmbedBuilder()
                {
                    Color = new(0x00000),
                    Footer = new()
                    {
                        Text = "-☿-"
                    },
                    Description = "<a:loadinghearts:950503533910835241>"
                }.Build());
                List<EmbedFieldBuilder> Fields = new CardHelper(new List<ICard> { new EventsCard(), new ExpendituresCard() }).CorrectWhitespacing();
                await Message.ModifyAsync(MessageInfo =>
                {
                    MessageInfo.Embed = new EmbedBuilder()
                    {
                        Fields = Fields,
                        Color = new(0x00000),
                        Footer = new()
                        {
                            Text = "-☿-"
                        }
                    }.Build();
                });
            }
        }
    }
}
