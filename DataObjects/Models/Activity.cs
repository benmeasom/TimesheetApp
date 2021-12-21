using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataObjects.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        [Required]
        public string ActivityName { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        [Required]
        public int ActivityTypeId { get; set; }
        public virtual IEnumerable<TimesheetItems> TimesheetItems { get; set; }
        public int Status { get; set; }
    }
}
