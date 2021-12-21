using DAL.User;
using DataObjects.DTO;

namespace BLL.User
{
    public class UserBLL : IUserBLL
    {
        #region '-- Members --'
        private readonly IUserDAL _userDAL;
        #endregion

        #region '-- Constructor --'
        public UserBLL(IUserDAL userDAL)
        {
            _userDAL = userDAL;
        }
        #endregion

        #region '-- Methods --'
        public ResultModel<UserDto> GetAllUser()
        {
            return _userDAL.GetAllUser();
        }
        
        public ResultModel<UserDto> GetActiveUser()
        {
            return _userDAL.GetActiveUser();
        }

        public ResultModel<RoleDto> GetRoles()
        {
            return _userDAL.GetRoles();
        }

        public ResultModel<UserDto> GetUserByID(string id)
        {
            return _userDAL.GetUserByID(id);
        }

        public ResultModel UpdateUser(string userId, string userRoleId, bool isDisabled)
        {
            return _userDAL.UpdateUser(userId, userRoleId, isDisabled);

        }

        public ResultModel UpdateUserPassword(string userId, string newPassword)
        {
            return _userDAL.UpdateUserPassword(userId, newPassword);
        }
        #endregion

    }
}
