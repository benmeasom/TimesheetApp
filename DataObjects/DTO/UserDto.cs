namespace DataObjects.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public bool IsDisabled { get; set; }
    }
}
