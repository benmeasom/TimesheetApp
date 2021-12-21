using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataObjects.Models
{
    public class ActivityType
    {
        [Key]
        public int ActivityTypeId { get; set; }
        [Required]
        public string ActivityTypeName { get; set; }
        public virtual IEnumerable<TimesheetItems> TimesheetItems { get; set; }
        public int Status { get; set; }
    }
}
