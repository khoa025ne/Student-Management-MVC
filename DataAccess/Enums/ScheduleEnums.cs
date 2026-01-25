namespace DataAccess.Enums
{
    /// <summary>
    /// Cặp ngày trong tuần: 2-5, 3-6, 4-7
    /// </summary>
    public enum DayOfWeekPair
    {
        MonThu = 1, // Thứ 2 - Thứ 5
        TueFri = 2, // Thứ 3 - Thứ 6
        WedSat = 3  // Thứ 4 - Thứ 7
    }

    /// <summary>
    /// Slot học trong ngày
    /// </summary>
    public enum TimeSlot
    {
        Slot1 = 1,
        Slot2 = 2,
        Slot3 = 3,
        Slot4 = 4
    }

    /// <summary>
    /// Ngày trong tuần
    /// </summary>
    public enum WeekDay
    {
        Monday = 1,    // Thứ 2
        Tuesday = 2,   // Thứ 3
        Wednesday = 3, // Thứ 4
        Thursday = 4,  // Thứ 5
        Friday = 5,    // Thứ 6
        Saturday = 6   // Thứ 7
    }
}
