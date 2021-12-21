using BLL.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace TimesheetApp.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        public IActivityBLL _activityBLL { get; set; }
        public ActivityController(IActivityBLL activityBLL)
        {
            _activityBLL = activityBLL;
        }

        #region 'Activity'

        [HttpGet]
        public IActionResult Activity()
        {
            ViewBag.ControllerName = "Activity";
            BindActivityDropDown();
            return View();
        }
        [HttpGet]
        public IActionResult GetAllActivities()
        {
            var activities = _activityBLL.GetAllActivities().DataList;
            return Json(new { Data = activities });
        }

        [HttpGet]
        public IActionResult GetActivityByID(int activityId)
        {
            var activities = _activityBLL.GetActivityByID(activityId).Data;
            return Json(new { Data = activities });
        }

        [HttpPost]
        public IActionResult DeleteActivityByID(int activityId)
        {
            var status = _activityBLL.DeleteActivityByID(activityId).IsSuccess;
            return Json(new { Status = status });
        }
        
        [HttpPost]
        public IActionResult AddActivity(string activityName,int activityType)
        {
            var status = _activityBLL.AddActivity(activityName, activityType).IsSuccess;
            return Json(new { Status = status });
        }

        [HttpPost]
        public IActionResult UpdateActivity(int activityId, string activityName, int activityType)
        {
            var status = _activityBLL.UpdateActivity(activityId, activityName, activityType).IsSuccess;
            return Json(new { Status = status });
        }
        #endregion


        #region 'Activity Type'
        public IActionResult ActivityType()
        {
            ViewBag.ControllerName = "ActivityType";
            return View();
        }

        [HttpGet]
        public IActionResult GetAllActivityType()
        {
            var activittType = _activityBLL.GetAllActivityType().DataList;
            return Json(new { Data = activittType });
        }

        [HttpGet]
        public IActionResult GetActivityTypeByID(int activityTypeId)
        {
            var activities = _activityBLL.GetActivityTypeByID(activityTypeId).Data;
            return Json(new { Data = activities });
        }

        [HttpPost]
        public IActionResult DeleteActivityTypeByID(int activityTypeId)
        {
            var status = _activityBLL.DeleteActivityTypeByID(activityTypeId).IsSuccess;
            return Json(new { Status = status });
        }

        [HttpPost]
        public IActionResult AddActivityType(string activityTypeName)
        {
            var status = _activityBLL.AddActivityType(activityTypeName).IsSuccess;
            return Json(new { Status = status });
        }

        [HttpPost]
        public IActionResult UpdateActivityType(int activityTypeId, string activityTypeName)
        {
            var status = _activityBLL.UpdateActivityType(activityTypeId, activityTypeName).IsSuccess;
            return Json(new { Status = status });
        }
        #endregion

        private void BindActivityDropDown()
        {
            List<SelectListItem> activityType;
            var activities = _activityBLL.GetAllActivityType().DataList;
            activityType = activities.Select(s => new SelectListItem
            {
                Text = s.ActivityTypeName,
                Value = s.ActivityTypeId.ToString()
            }).ToList();
            activityType.Insert(0, new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });
            ViewBag.ActivityType = activityType;
        }
    }
}
