using System;

namespace DataObjects.DTO
{
    public class ReviewTimesheetItems
    {
        public DateTime Date { get; set; }
        public string ActivityName { get; set; }
        public string ActivityTypeName { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public string Notes { get; set; }
    }
}
