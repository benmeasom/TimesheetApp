using DataObjects.Context;
using DataObjects.DTO;
using DataObjects.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace DAL.User
{
    public class UserDAL : IUserDAL
    {
        #region Members

        private readonly TimesheetDBContext _context;
        private readonly UserManager<TimesheetUser> _userManager;

        #endregion

        #region Constructor
        public UserDAL(TimesheetDBContext context, UserManager<TimesheetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        #endregion

        #region Methods
        public ResultModel<UserDto> GetAllUser()
        {
            ResultModel<UserDto> result = new ResultModel<UserDto>();
            try
            {
                result.DataList = (from user in _context.TimesheetUsers
                                   join uRole in _context.UserRoles on user.Id equals uRole.UserId
                                   join role in _context.Roles on uRole.RoleId equals role.Id
                                   select new UserDto
                                   {
                                       Id = user.Id,
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       Email = user.Email,
                                       IsDisabled = user.IsDisabled,
                                       RoleId = role.Id,
                                       RoleName = role.Name
                                   }).ToList();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }

        public ResultModel<UserDto> GetActiveUser()
        {
            ResultModel<UserDto> result = new ResultModel<UserDto>();
            try
            {
                result.DataList = (from user in _context.TimesheetUsers
                                   join uRole in _context.UserRoles on user.Id equals uRole.UserId
                                   join role in _context.Roles on uRole.RoleId equals role.Id
                                   where !user.IsDisabled
                                   select new UserDto
                                   {
                                       Id = user.Id,
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       Email = user.Email,
                                       IsDisabled = user.IsDisabled,
                                       RoleId = role.Id,
                                       RoleName = role.Name
                                   }).ToList();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }

        public ResultModel<RoleDto> GetRoles()
        {
            ResultModel<RoleDto> result = new ResultModel<RoleDto>();
            try
            {
                result.DataList = _context.Roles.Select(s => new RoleDto
                {
                    Id = s.Id,
                    RoleName = s.Name
                }).ToList();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;

        }

        public ResultModel<UserDto> GetUserByID(string id)
        {
            ResultModel<UserDto> result = new ResultModel<UserDto>();
            try
            {
                result.Data = (from user in _context.TimesheetUsers
                               join uRole in _context.UserRoles on user.Id equals uRole.UserId
                               join role in _context.Roles on uRole.RoleId equals role.Id
                               where user.Id == id
                               select new UserDto
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   IsDisabled = user.IsDisabled,
                                   RoleId = role.Id,
                                   RoleName = role.Name
                               }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }

        public ResultModel UpdateUser(string userId, string userRoleId, bool isDisabled)
        {
            ResultModel result = new ResultModel();
            try
            {
                var existingUser = _context.TimesheetUsers.FirstOrDefault(s => s.Id == userId);
                if (existingUser == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "User doesn't exists.";
                    return result;
                }
                var existingUserRole = _context.UserRoles.FirstOrDefault(s => s.UserId == userId);
                if (existingUserRole == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "User role doesn't exists.";
                    return result;
                }

                existingUser.IsDisabled = isDisabled;
                _context.TimesheetUsers.Update(existingUser);

                //https://stackoverflow.com/questions/62563307/the-property-roleid-on-entity-type-userrole-is-part-of-a-key-and-so-cannot-b
                if (existingUserRole.RoleId != userRoleId)
                {
                    _context.UserRoles.Remove(existingUserRole);

                    _context.UserRoles.Add(new DataObjects.Models.UserRole
                    {
                        RoleId = userRoleId,
                        UserId = userId
                    });
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;

        }

        public ResultModel UpdateUserPassword(string userId, string newPassword)
        {
            ResultModel result = new ResultModel();
            try
            {
                var existingUser = _context.TimesheetUsers.FirstOrDefault(s => s.Id == userId);
                if (existingUser == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "User doesn't exists.";
                    return result;
                }

                var token = _userManager.GeneratePasswordResetTokenAsync(existingUser).Result;

                var update = _userManager.ResetPasswordAsync(existingUser, token, newPassword).Result;

                if (!update.Succeeded)
                {
                    result.IsSuccess = false;
                    result.Msg = update.Errors.FirstOrDefault().Description;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }
        #endregion
    }
}
