﻿using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using Mercury.Snapshot.Objects.Structures.UserStructures.Personalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Snapshot.Commands
{
    public static class Settings
    {
        [Command(new[] { "assign-settings" }, "Tells me what settings to use for personalized responses.", Defer = false, LocalOnly = true)]
        public static async void ChangeSettings(CommandArguments Args, string? ExpenditureSheetId, string? ZipCode)
        {
            MercuryUser User = new(Args.SlashCommand.User.Id);
            User.Settings = new(User.Settings.GoogleCalendarSettings, new(ExpenditureSheetId), new(ZipCode), User.Settings.CultureSettings);
            await Args.SlashCommand.RespondAsync(Strings.SettingsStrings.SettingsSaved, null, false, true).ConfigureAwait(false);
        }

        [Command(new[] { "assign-language-settings" }, "Personalizes things such as decimal delimiters and dates.", Defer = false, LocalOnly = true)]
        public static async void ChangeSettings(CommandArguments Args, string ISOLanguageCode)
        {
            MercuryUser User = new(Args.SlashCommand.User.Id);
            User.Settings = new(User.Settings.GoogleCalendarSettings, User.Settings.GoogleSheetsSettings, User.Settings.WeatherSettings, new(ISOLanguageCode));
            await Args.SlashCommand.RespondAsync(Strings.SettingsStrings.SettingsSaved, null, false, true).ConfigureAwait(false);
        }
    }
}
