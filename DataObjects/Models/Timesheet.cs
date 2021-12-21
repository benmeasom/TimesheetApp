using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataObjects.Models
{
    public class Timesheet
    {
        [Key]
        public long TimesheetId { get; set; }
        [Required]
        public DateTime TimesheetDate { get; set; }
        public DateTime WeekEndingDate { get; set; }
        public string Name { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinutes { get; set; }
        public string UserId { get; set; }
        public virtual TimesheetUser TimesheetUser { get; set; }
        public virtual ICollection<TimesheetItems> TimesheetItems { get; set; }
    }
}
