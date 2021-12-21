using DataObjects.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataObjects.DTO
{
    public class TimesheetItemsDTO : IValidatableObject
    {
        public int Index { get; set; }
        public int? ActivityTypeId { get; set; }
        public int? ActivityId { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public string Notes { get; set; }
        public long TimesheetId { get; set; }
        public long TimesheetItemsId { get; set; }
        public Timesheet Timesheet { get; set; }
        public Activity Activity { get; set; }
        public int Saved { get; set; }

        //https://dev.to/cesarcodes/net-core-api-rest-post-validation-with-ivalidatableobject-5d
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            Hours = Hours == null ? 0 : Hours;
            Minutes = Minutes == null ? 0 : Minutes;
            if (Hours == 0 && Minutes == 0)
            {
                results.Add(new ValidationResult("The Hours Is Required.", new List<string> { "Hours" }));
                results.Add(new ValidationResult("The Minutes Is Required.", new List<string> { "Minutes" }));
            }

            if (ActivityId == null || ActivityId == 0)
            {
                results.Add(new ValidationResult("The Activity Is Required.", new List<string> { "ActivityId" }));
            }

            if (ActivityTypeId == null || ActivityTypeId == 0)
            {
                results.Add(new ValidationResult("The Activity Type Is Required.", new List<string> { "ActivityTypeId" }));
            }

            return results;
        }
    }
}
