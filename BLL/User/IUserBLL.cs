using DataObjects.DTO;

namespace BLL.User
{
    public interface IUserBLL
    {
        ResultModel<UserDto> GetAllUser();
        ResultModel<UserDto> GetActiveUser();
        ResultModel<RoleDto> GetRoles();
        ResultModel<UserDto> GetUserByID(string id);
        ResultModel UpdateUser(string userId, string userRoleId, bool isDisabled);
        ResultModel UpdateUserPassword(string userId, string newPassword);
    }
}
