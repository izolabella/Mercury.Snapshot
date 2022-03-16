﻿using Mercury.Snapshot.Objects.Structures.Calendars.Events;

namespace Mercury.Snapshot.Objects.Structures.Calendars
{
    public interface ICalendar
    {
        public Task<IReadOnlyCollection<IEvent>> GetEvents(DateTime TimeMin, DateTime TimeMax, int MaxResults);
    }
}
