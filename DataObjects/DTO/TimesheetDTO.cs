using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataObjects.DTO
{
    public class TimesheetDTO : IValidatableObject
    {
        public long TimesheetId { get; set; }

        [Required(ErrorMessage = "The Timesheet Date Is Required.")]
        public DateTime TimesheetDate { get; set; }

        [Required(ErrorMessage = "The Week Ending Date Is Required.")]
        public DateTime WeekEndingDate { get; set; }
        public string Name { get; set; }

        public int? TotalHours { get; set; }

        public int? TotalMinutes { get; set; }
        public string UserId { get; set; }

        public bool IsTimeSheetExists { get; set; }
        public int? ExistingTotalHours { get; set; }
        public int? ExistingTotalMinutes { get; set; }

        public List<TimesheetItemsDTO> TimesheetItems { get; set; }
        public UserDto TimesheetUser { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (TimesheetDate == DateTime.MinValue)
            {
                results.Add(new ValidationResult("Please select valid timesheet date."));
            }
            if (WeekEndingDate == DateTime.MinValue)
            {
                results.Add(new ValidationResult("Please select valid week sending date."));
            }
            return results;
        }
    }
}
