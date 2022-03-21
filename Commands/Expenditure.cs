﻿using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using izolabella.Discord.Internals.Structures.Commands;
using Mercury.Snapshot.Objects.Structures.Embeds;
using Mercury.Snapshot.Objects.Structures.UserStructures.Financial.Entries;
using Mercury.Snapshot.Objects.Structures.UserStructures.Identification;
using Mercury.Snapshot.Objects.Structures.UserStructures.Personalization;
using Mercury.Unification.IO.File.Records;

namespace Mercury.Snapshot.Commands
{
    public class Expenditure
    {
        [Command(new string[] { "log-expenditure" }, "Log an expenditure.", Defer = false, LocalOnly = true)]
        public static async void LogExpenditure(CommandArguments Args, double Amount, string PayerOrPayee, string Category, bool Credit = false)
        {
            MercuryUser User = new(Args.SlashCommand.User.Id);
            if (User.ExpenditureEntriesRegister != null)
            {
                MercuryExpenditureEntry Entry = new(DateTime.UtcNow, Credit ? (Math.Abs(Amount)) : (-Math.Abs(Amount)), PayerOrPayee, Category ?? string.Empty, Origins.Mercury, Identifier.GetIdentifier());
                User.ExpenditureEntriesRegister.SaveRecord(Entry.Id, new Record<IExpenditureEntry>(Entry, null));
                await Args.SlashCommand.RespondAsync(Strings.EmbedStrings.Expenditures.ExpenditureSuccessfullyLogged, new Embed[] { new ExpenditureLoggedEmbed(Entry).Build() }, false, true);
            }
            else
            {
                await Args.SlashCommand.RespondAsync(Strings.EmbedStrings.Expenditures.ExpenditureLogFailed, null, false, true);
            }
        }
    }
}
