using System;

namespace DataObjects.DTO
{
    public class ReviewTimesheet
    {
        public long TimeSheetId { get; set; }
        public string DayName { get; set; }
        public DateTime? Date { get; set; }
        public int? TotalHours { get; set; }
        public int? TotalMinutes { get; set; }
    }
}
