using Microsoft.AspNetCore.Identity;


namespace DataObjects.Models
{
    public class TimesheetUser : IdentityUser
    {
        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDisabled { get; set; }
    }
}
