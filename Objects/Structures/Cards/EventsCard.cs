﻿using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Mercury.Snapshot.Objects.Util;
using Mercury.Snapshot.Objects.Util.Google.Calendar;
using Mercury.Snapshot.Objects.Util.Weather;
using openweathermap.NET.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Snapshot.Objects.Structures.Cards
{
    internal class EventsCard : ICard
    {
        internal EventsCard()
        {
        }

        public IReadOnlyList<EmbedFieldBuilder> Render()
        {
            List<EmbedFieldBuilder> EmbedFieldBuilders = new();

            EventsResource.ListRequest RequestToday = (Program.GoogleClient.CalendarManager.Service ?? throw new NullReferenceException(nameof(Program.GoogleClient.CalendarManager.Service) + " can not be null.")).Events.List("primary");
            RequestToday.TimeMin = DateTime.Today.Date;
            RequestToday.TimeMax = DateTime.Today.Date.Add(new TimeSpan(23, 59, 59));
            RequestToday.SingleEvents = true;
            RequestToday.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            EventsResource.ListRequest RequestWeek = (Program.GoogleClient.CalendarManager.Service ?? throw new NullReferenceException(nameof(Program.GoogleClient.CalendarManager.Service) + " can not be null.")).Events.List("primary");
            RequestWeek.TimeMin = DateTime.Today.Date.AddDays(1);
            RequestWeek.TimeMax = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(7).Date.Add(new TimeSpan(23, 59, 59));
            RequestWeek.SingleEvents = true;
            RequestWeek.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            EventsResource.ListRequest RequestMonth = (Program.GoogleClient.CalendarManager.Service ?? throw new NullReferenceException(nameof(Program.GoogleClient.CalendarManager.Service) + " can not be null.")).Events.List("primary");
            RequestMonth.TimeMin = RequestWeek.TimeMax;
            RequestMonth.TimeMax = new(DateTime.Today.Date.Year, DateTime.Today.Month + 1, 1);
            RequestMonth.SingleEvents = true;
            RequestMonth.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            IReadOnlyList<Event> EventsToday = Program.GoogleClient.CalendarManager.GetIzolabellasEvents(RequestToday);
            IReadOnlyList<Event> EventsWeek = Program.GoogleClient.CalendarManager.GetIzolabellasEvents(RequestWeek);
            IReadOnlyList<Event> EventsMonth = Program.GoogleClient.CalendarManager.GetIzolabellasEvents(RequestMonth);

            WeatherResponse? WeatherToday = WeatherManager.GetWeatherForToday();

            if (EventsToday.Count > 0 || WeatherToday != null)
            {
                EmbedFieldBuilder Today = new()
                {
                    Name = $"TODAY • {DateTime.Today.ToLongDateString()}",
                };
                if(WeatherToday != null)
                {
                    int TemperatureF = (int)((((double)WeatherToday.Main.Temp - 273.15) * (9 / 5)) + 32); // from kelvin
                    int TemperatureFMax = (int)((((double)WeatherToday.Main.TempMaximum - 273.15) * (9 / 5)) + 32); // from kelvin
                    int TemperatureFMin = (int)((((double)WeatherToday.Main.TempMinimum - 273.15) * (9 / 5)) + 32); // from kelvin
                    Today.Value = $"{TemperatureF}° - {WeatherToday.Name}\nH: {TemperatureFMax}° L: {TemperatureFMin}°\n";
                }
                foreach(Event Event in EventsToday)
                {
                    if(Event.Start.DateTime.HasValue)
                        Today.Value += $"\n{Event.Start.DateTime.Value.ToShortTimeString()}\n```\n{Event.Summary}{Event.Description}\n```";
                }
                Today.Value += "\u200b\n";
                EmbedFieldBuilders.Add(Today);
            }

            if(EventsWeek.Count > 0)
            {
                EmbedFieldBuilder Week = new()
                {
                    Name = $"THIS WEEK",
                };
                foreach(Event Event in EventsWeek)
                {
                    if(Event.Start.DateTime.HasValue)
                        Week.Value += $"\n{Event.Start.DateTime.Value.ToShortDateString()} {Event.Start.DateTime.Value.ToShortTimeString()}\n```\n{Event.Summary}{Event.Description}\n```";
                }
                Week.Value += "\u200b\n";
                EmbedFieldBuilders.Add(Week);
            }

            if(EventsMonth.Count > 0)
            {
                EmbedFieldBuilder Month = new()
                {
                    Name = $"THIS MONTH",
                };
                foreach(Event Event in EventsMonth)
                {
                    if(Event.Start.DateTime != null)
                        Month.Value += $"\n{Event.Start.DateTime.Value.ToShortDateString()} {Event.Start.DateTime.Value.ToShortTimeString()}\n```\n{Event.Summary}{Event.Description}\n```";
                }
                Month.Value += "\u200b\n";
                EmbedFieldBuilders.Add(Month);
            }
            return EmbedFieldBuilders;
        }
    }
}