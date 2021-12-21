using BLL.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace TimesheetApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region '-- Members --' 

        private readonly IUserBLL _userBLL;
        #endregion

        #region '-- Constructor --'
        public UserController(IUserBLL userBLL)
        {
            _userBLL = userBLL;
        }
        #endregion

        #region '-- Methods --'
        public IActionResult ManageUser()
        {
            ViewBag.ControllerName = "ManageUser";
            BindDropDown();
            return View();
        }

        public IActionResult GetAllUser()
        {
            var users = _userBLL.GetAllUser();
            return Json(new { Data = users.DataList });
        }

        public IActionResult GetUserByID(string id)
        {
            var users = _userBLL.GetUserByID(id);
            return Json(new { Data = users.Data });
        }

        [HttpPost]
        public IActionResult UpdateUser(string userId, string userRoleId, bool isDisabled)
        {
            var status = _userBLL.UpdateUser(userId, userRoleId, isDisabled).IsSuccess;
            return Json(new { Status = status });
        }

        [HttpPost]
        public IActionResult UpdateUserPassword(string userId, string newPassword)
        {
            var response = _userBLL.UpdateUserPassword(userId, newPassword);
            return Json(new { Status = response.IsSuccess, Message = response.Msg });
        }

        private void BindDropDown()
        {
            List<SelectListItem> userRoleType;

            var userRole = _userBLL.GetRoles().DataList;

            userRoleType = userRole.Select(s => new SelectListItem
            {
                Text = s.RoleName,
                Value = s.Id.ToString()
            }).ToList();

            ViewBag.UserRoleType = userRoleType;
        }
        #endregion
    }
}
