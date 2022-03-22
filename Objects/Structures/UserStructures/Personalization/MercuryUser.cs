﻿using Mercury.Snapshot.Objects.Structures.UserStructures.Calendars;
using Mercury.Snapshot.Objects.Structures.UserStructures.Calendars.Events;
using Mercury.Snapshot.Objects.Structures.UserStructures.Financial;
using Mercury.Snapshot.Objects.Structures.UserStructures.Financial.Entries;
using Mercury.Snapshot.Objects.Util;
using Mercury.Unification.IO.File.Records;
using Mercury.Unification.IO.File.Registers;

namespace Mercury.Snapshot.Objects.Structures.UserStructures.Personalization
{
    public class MercuryUser
    {
        public MercuryUser(ulong DiscordId, MercuryUserSettings Settings)
        {
            this.DiscordId = DiscordId;
            this.Settings = new Record<MercuryUserSettings>(Settings, new List<string>());
            this.GoogleClient = new(this.DiscordId);
            this.CalendarEventsRegister = Registers.CalendarsRegister.GetSubRegister<ICalendar>(this.DiscordId)?.GetSubRegister<CalendarEvent>(this.DiscordId);
        }

        public MercuryUser(ulong DiscordId)
        {
            this.DiscordId = DiscordId;
            this.GoogleClient = new(this.DiscordId);
            this.CalendarEventsRegister = Registers.CalendarsRegister.GetSubRegister<ICalendar>(this.DiscordId)?.GetSubRegister<CalendarEvent>(this.DiscordId);
        }

        public GoogleClient GoogleClient { get; }

        public Record<MercuryUserSettings> Settings
        {
            get => Registers.MercurySettingsRegister.GetRecord(this.DiscordId.ToString()) ?? new Record<MercuryUserSettings>(new(), new List<string>() { "Auto-generated" });
            set => Registers.MercurySettingsRegister.SaveRecord(this.DiscordId.ToString(), value);
        }
        public async Task<IReadOnlyCollection<CalendarEvent>> GetAllCalendarEventsAsync(DateTime TimeMin, DateTime TimeMax, int MaxResults)
        {
            List<CalendarEvent> Events = new();
            foreach (ICalendar? Calendar in this.Calendars)
            {
                if (Calendar != null)
                {
                    IReadOnlyCollection<CalendarEvent> CalendarEvents = await Calendar.GetEvents(TimeMin, TimeMax, MaxResults);
                    foreach (CalendarEvent Event in CalendarEvents)
                    {
                        CalendarEvent? AlreadyHere = Events.FirstOrDefault(FromList =>
                        {
                            return FromList.Start == Event.Start && FromList.Summary == Event.Summary && FromList.Description == Event.Description;
                        });
                        if (AlreadyHere == null)
                        {
                            Events.Add(Event);
                        }
                    }
                }
            }
            return Events;
        }
        public IReadOnlyCollection<ICalendar?> Calendars
        {
            get => new List<ICalendar?>()
            {
                this.GoogleClient.CalendarManager,
                this.Calendar
            };
        }
        public MercuryCalendar Calendar
        {
            get => Registers.CalendarsRegister.GetSubRegister<MercuryCalendar>(this.DiscordId)?.GetRecord("primary")?.ObjectToStore ?? new MercuryCalendar(this);
            set => Registers.CalendarsRegister.GetSubRegister<MercuryCalendar>(this.DiscordId)?.SaveRecord("primary", new Record<MercuryCalendar>(value));
        }

        public IReadOnlyCollection<IExpenditureLog?> ExpenditureLogs
        {
            get => new List<IExpenditureLog?>()
            {
                this.ExpenditureLog
            };
        }
        public MercuryExpenditureLog ExpenditureLog
        {
            get => Registers.ExpenditureLogsRegister.GetSubRegister<MercuryExpenditureLog>(this.DiscordId)?.GetRecord("primary")?.ObjectToStore ?? new MercuryExpenditureLog(this);
            set => Registers.ExpenditureLogsRegister.GetSubRegister<MercuryExpenditureLog>(this.DiscordId)?.SaveRecord("primary", new Record<MercuryExpenditureLog>(value));
        }

        public Register<CalendarEvent>? CalendarEventsRegister { get; }
        public Register<ExpenditureEntry>? ExpenditureEntriesRegister { get; }

        public ulong DiscordId { get; }
    }
}
