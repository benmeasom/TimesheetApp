using System.ComponentModel.DataAnnotations;

namespace DataObjects.Models
{
    public class TimesheetItems
    {
        [Key]
        public long TimesheetItemsId { get; set; }
        public virtual Timesheet Timesheet { get; set; }
        [Required]
        public long TimesheetId { get; set; }
        public virtual Activity Activity { get; set; }
        public int ActivityId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public int ActivityTypeId { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public string Notes { get; set; }
    }
}
