using BLL;
using BLL.Activity;
using BLL.User;
using DataObjects.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace TimesheetApp.Controllers
{
    [Authorize]
    public class TimesheetController : Controller
    {

        #region '-- Members --' 

        private readonly IActivityBLL _activityBLL;
        private readonly ITimesheetBLL _timesheetBLL;
        private readonly IUserBLL _userBLL;
        private readonly string TimesheetData = "TimesheetData";

        #endregion

        #region '-- Constructor --'

        public TimesheetController(IActivityBLL activityBLL, ITimesheetBLL timesheetBLL, IUserBLL userBLL)
        {
            _activityBLL = activityBLL;
            _timesheetBLL = timesheetBLL;
            _userBLL = userBLL;
        }

        #endregion

        #region '-- Methods --'

        #region '-- TimeSheet --'
        public IActionResult Timesheet()
        {
            User.FindFirst(ClaimTypes.Name);
            ViewBag.ControllerName = "Timesheet";
            BindDropDown();
            TimesheetDTO timesheet = new TimesheetDTO();
            timesheet.TimesheetItems = new List<TimesheetItemsDTO>();
            timesheet.TimesheetItems.Add(new TimesheetItemsDTO());
            return View(timesheet);
        }

        [HttpPost]
        public IActionResult Timesheet(TimesheetDTO timesheet)
        {
            BindDropDown();
            if (!ModelState.IsValid && timesheet.TimesheetItems.Count == 1 && timesheet.TimesheetItems[0].Saved == 0)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    //https://stackoverflow.com/questions/15296069/how-to-figure-out-which-key-of-modelstate-has-error
                    if (modelStateKey != "TimesheetDate" && modelStateKey != "WeekEndingDate")
                    {
                        ModelState.Remove(modelStateKey);
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(timesheet);
                }
                else
                {
                    ViewBag.NoItemSaved = true;
                    return View(timesheet);
                }
            }

            if (ModelState.IsValid && timesheet.TimesheetItems.Count == 1 && timesheet.TimesheetItems[0].Saved == 0)
            {
                ViewBag.NoItemSaved = true;
                return View(timesheet);
            }
            else if (!ModelState.IsValid)
            {
                for (int i = 0; i < timesheet.TimesheetItems.Count; i++)
                {
                    if (timesheet.TimesheetItems[i].Saved == 0)
                    {
                        timesheet.TimesheetItems.Remove(timesheet.TimesheetItems[i]);
                        ModelState.Remove(string.Format("TimesheetItems[{0}].ActivityTypeId", i));
                        ModelState.Remove(string.Format("TimesheetItems[{0}].ActivityId", i));
                        ModelState.Remove(string.Format("TimesheetItems[{0}].Hours", i));
                        ModelState.Remove(string.Format("TimesheetItems[{0}].Minutes", i));
                    }
                }

                timesheet.TimesheetItems.RemoveAll(t => t.Saved == 0);
                var time = TimeSpan.FromMinutes(timesheet.TimesheetItems.Sum(s => s.Minutes) ?? 0);
                timesheet.TotalMinutes = Convert.ToInt32(time.Minutes);
                timesheet.TotalHours = timesheet.TimesheetItems.Sum(s => s.Hours) + time.Hours + (time.Days * 24);
                if (timesheet.TotalHours >= 24)
                {
                    ViewBag.TimeLimit = true;
                    return View(timesheet);
                }
                if (!ModelState.IsValid)
                {
                    return View(timesheet);
                }
            }

            timesheet.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            timesheet.TimesheetItems.RemoveAll(t => t.Saved == 0);
            var response = _timesheetBLL.AddTimesheet(timesheet);
            if (response.IsSuccess)
            {
                TempData["SavedSuccessfully"] = true;
                return RedirectToAction("Timesheet");
            }
            return View(timesheet);
        }

        [HttpPost]
        public ActionResult AddItemsData(TimesheetDTO data)
        {
            BindDropDown();
            if (data.TimesheetId <= 0)
            {
                var existingTimesheet = GetExistingTimeSheet(data.TimesheetDate);

                if (existingTimesheet.Item1)
                {
                    data.IsTimeSheetExists = existingTimesheet.Item1;
                    data.ExistingTotalHours = existingTimesheet.Item2;
                    data.ExistingTotalMinutes = existingTimesheet.Item3;
                    ModelState.Clear();
                    return PartialView("_TimesheetItems", data);
                }
            }


            var time = TimeSpan.FromMinutes(data.TimesheetItems.Sum(s => s.Minutes) ?? 0);
            data.TotalMinutes = Convert.ToInt32(time.Minutes);
            data.TotalHours = data.TimesheetItems.Sum(s => s.Hours) + time.Hours + (time.Days * 24);

            if (!ModelState.IsValid)
            {
                return PartialView("_TimesheetItems", data);
            }

            if (data.TotalHours >= 24)
            {
                if (!(data.TotalHours == 24 && data.TotalMinutes == 0))
                {
                    return PartialView("_TimesheetItems", data);
                }
            }
            data.TimesheetItems.ForEach(f => f.Saved = 1);
            data.TimesheetItems.Add(new TimesheetItemsDTO());
            TempData[TimesheetData] = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return PartialView("_TimesheetItems", data);
        }

        [HttpPost]
        public ActionResult RemoveItemsData(TimesheetDTO data, int index)
        {
            data.TimesheetItems.RemoveAt(index);
            var time = TimeSpan.FromMinutes(data.TimesheetItems.Sum(s => s.Minutes) ?? 0);
            data.TotalMinutes = Convert.ToInt32(time.Minutes);
            data.TotalHours = data.TimesheetItems.Sum(s => s.Hours) + time.Hours + (time.Days * 24);
            ModelState.Clear();
            BindDropDown();
            TempData[TimesheetData] = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return PartialView("_TimesheetItems", data);
        }

        [HttpPost]
        public ActionResult GetTimesheetItemForEdit(TimesheetDTO data, int index)
        {
            TempData[TimesheetData] = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var item = data.TimesheetItems.ElementAt(index);
            item.Index = index;
            BindEditDropDown(item.ActivityTypeId.Value);
            return PartialView("_EditTimesheetItems", item);
        }

        [HttpPost]
        public ActionResult SaveTimesheetItem(TimesheetItemsDTO TimesheetItem)
        {
            string html = "";

            if (!ModelState.IsValid)
            {
                TimesheetItem.ActivityTypeId = TimesheetItem.ActivityTypeId == null ? 0 : TimesheetItem.ActivityTypeId;
                TimesheetItem.ActivityId = TimesheetItem.ActivityId == null ? 0 : TimesheetItem.ActivityId;
                BindEditDropDown(TimesheetItem.ActivityTypeId.Value);
                html = RenderViewAsync(this, "_EditTimesheetItems", TimesheetItem, true);
                return Json(new { Status = false, Html = html });
            }

            var timesheetData = Newtonsoft.Json.JsonConvert.DeserializeObject<TimesheetDTO>(TempData[TimesheetData].ToString());
            TempData.Keep(TimesheetData);
            timesheetData.TimesheetItems[TimesheetItem.Index].ActivityId = TimesheetItem.ActivityId;
            timesheetData.TimesheetItems[TimesheetItem.Index].ActivityTypeId = TimesheetItem.ActivityTypeId;
            timesheetData.TimesheetItems[TimesheetItem.Index].Hours = TimesheetItem.Hours;
            timesheetData.TimesheetItems[TimesheetItem.Index].Minutes = TimesheetItem.Minutes;
            timesheetData.TimesheetItems[TimesheetItem.Index].Notes = TimesheetItem.Notes;
            timesheetData.TimesheetItems[TimesheetItem.Index].TimesheetItemsId = TimesheetItem.TimesheetItemsId;
            timesheetData.TimesheetItems[TimesheetItem.Index].TimesheetId = TimesheetItem.TimesheetId;
            var time = TimeSpan.FromMinutes(timesheetData.TimesheetItems.Sum(s => s.Minutes) ?? 0);
            timesheetData.TotalMinutes = Convert.ToInt32(time.Minutes);
            timesheetData.TotalHours = timesheetData.TimesheetItems.Sum(s => s.Hours) + time.Hours + (time.Days * 24);

            if (timesheetData.TotalHours >= 24)
            {
                if (!(timesheetData.TotalHours == 24 && timesheetData.TotalMinutes == 0))
                {
                    BindEditDropDown(TimesheetItem.ActivityTypeId.Value);
                    html = RenderViewAsync(this, "_EditTimesheetItems", TimesheetItem, true);
                    return Json(new { Status = false, Message = "TimesheetHours", Html = html });
                }
            }

            ModelState.Clear();
            BindDropDown();
            TempData[TimesheetData] = Newtonsoft.Json.JsonConvert.SerializeObject(timesheetData);
            html = RenderViewAsync(this, "_TimesheetItems", timesheetData, true);
            return Json(new { Status = true, Html = html });
        }

        [HttpGet]
        public IActionResult GetTimesheetHoursByDate(DateTime date)
        {
            var weekTimesheetData = _timesheetBLL.GetTimesheetByDate(date, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Json(new { IsDataExists = weekTimesheetData.DataList.Any(), TotalHours = weekTimesheetData.Data.TotalHours, TotalMinutes = weekTimesheetData.Data.TotalMinutes });
        }

        [HttpGet]
        public ActionResult GetActivityByActivityTypeId(int activityTypeId)
        {
            List<SelectListItem> ActivityTypeList;

            var result = _activityBLL.GetActivityByActivityTypeId(activityTypeId);

            ActivityTypeList = result.DataList.Select(s => new SelectListItem
            {
                Text = s.ActivityName,
                Value = s.ActivityId.ToString()
            }).ToList();

            ActivityTypeList.Insert(0, new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });

            return Json(new { data = ActivityTypeList });
        }

        #endregion

        #region '-- Review --'
        public IActionResult Review()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetTimesheetByEndDate(string endDate)
        {
            var weekTimesheetData = _timesheetBLL.GetTimesheetByEndDate(Convert.ToDateTime(endDate), User.FindFirstValue(ClaimTypes.NameIdentifier)).DataList;
            return Json(weekTimesheetData);
        }

        [HttpGet]
        public IActionResult GetTimesheetByUser(string endDate, string userId)
        {
            var weekTimesheetData = _timesheetBLL.GetTimesheetByEndDate(Convert.ToDateTime(endDate), userId).DataList;
            return Json(weekTimesheetData);
        }

        [HttpGet]
        public IActionResult GetTimesheetDatesByEndDate(string endDate)
        {
            var weekTimesheetData = _timesheetBLL.GetTimesheetDatesByEndDate(Convert.ToDateTime(endDate), User.FindFirstValue(ClaimTypes.NameIdentifier)).DataList;
            return Json(weekTimesheetData);
        }

        [HttpGet]
        public IActionResult GetTimesheetDetailsByTimesheetId(long timesheetId, string endDate)
        {
            if (timesheetId == -1)
            {
                return PartialView("_ReviewTimesheetItems", new List<ReviewTimesheetItems>());
            }
            var weekTimesheetData = _timesheetBLL.GetTimesheetDetailsByTimesheetId(timesheetId, Convert.ToDateTime(endDate), User.FindFirstValue(ClaimTypes.NameIdentifier)).DataList;
            return PartialView("_ReviewTimesheetItems", weekTimesheetData);
        }
        [HttpGet]
        public IActionResult GetTimesheetDetailsByUserTimesheetId(long timesheetId, string endDate, string userId)
        {
            if (timesheetId == -1)
            {
                return PartialView("_ReviewTimesheetItems", new List<ReviewTimesheetItems>());
            }
            var weekTimesheetData = _timesheetBLL.GetTimesheetDetailsByTimesheetId(timesheetId, Convert.ToDateTime(endDate), userId).DataList;
            return PartialView("_ReviewTimesheetItems", weekTimesheetData);
        }

        #endregion

        #region '-- Manage Timesheet --'
        [Authorize(Roles = "Admin")]
        public ActionResult ManageTimesheet(int timesheetId, string userId)
        {
            ViewBag.ControllerName = "ManageTimesheet";
            BindManageTimesheetDropdown();
            if (timesheetId <= 0)
            {
                TimesheetDTO timesheet = new TimesheetDTO();
                timesheet.TimesheetItems = new List<TimesheetItemsDTO>();
                timesheet.TimesheetItems.Add(new TimesheetItemsDTO());
                ViewBag.IsUserSelectPageRequired = true;
                return View(timesheet);
            }
            var timesheetData = _timesheetBLL.GetTimesheetById(timesheetId, userId);
            if (timesheetData.Data == null)
            {
                return RedirectToAction("Timesheet");
            }

            return View(timesheetData.Data);
        }

        [HttpPost]
        public IActionResult SaveTimesheet(TimesheetDTO timesheet)
        {
            BindManageTimesheetDropdown();
            if (!ModelState.IsValid && timesheet.TimesheetItems.Count == 1 && timesheet.TimesheetItems[0].Saved == 0)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    if (modelStateKey != "TimesheetDate" && modelStateKey != "WeekEndingDate")
                    {
                        ModelState.Remove(modelStateKey);
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View("ManageTimesheet", timesheet);
                }
                else
                {
                    ViewBag.NoItemSaved = true;
                    return View("ManageTimesheet", timesheet);
                }
            }

            if (ModelState.IsValid && timesheet.TimesheetItems.Count == 1 && timesheet.TimesheetItems[0].Saved == 0)
            {
                ViewBag.NoItemSaved = true;
                return View("ManageTimesheet", timesheet);
            }
            else if (!ModelState.IsValid)
            {
                for (int i = 0; i < timesheet.TimesheetItems.Count; i++)
                {
                    if (timesheet.TimesheetItems[i].Saved == 0)
                    {
                        timesheet.TimesheetItems.Remove(timesheet.TimesheetItems[i]);
                        ModelState.Remove(string.Format("TimesheetItems[{0}].ActivityTypeId", i));
                        ModelState.Remove(string.Format("TimesheetItems[{0}].ActivityId", i));
                        ModelState.Remove(string.Format("TimesheetItems[{0}].Hours", i));
                        ModelState.Remove(string.Format("TimesheetItems[{0}].Minutes", i));
                    }
                }

                timesheet.TimesheetItems.RemoveAll(t => t.Saved == 0);
                var time = TimeSpan.FromMinutes(timesheet.TimesheetItems.Sum(s => s.Minutes) ?? 0);
                timesheet.TotalMinutes = Convert.ToInt32(time.Minutes);
                timesheet.TotalHours = timesheet.TimesheetItems.Sum(s => s.Hours) + time.Hours + (time.Days * 24);
                if (timesheet.TotalHours >= 24)
                {
                    ViewBag.TimeLimit = true;
                    return View("ManageTimesheet", timesheet);
                }
                if (!ModelState.IsValid)
                {
                    return View("ManageTimesheet", timesheet);
                }
            }

            timesheet.TimesheetItems.RemoveAll(t => t.Saved == 0);
            var response = _timesheetBLL.SaveTimesheet(timesheet);
            if (response.IsSuccess)
            {
                TempData["SavedSuccessfully"] = true;
                return RedirectToAction("ManageTimesheet");
            }
            return View(timesheet);
        }

        #endregion

        #region '-- Private --'
        private void BindManageTimesheetDropdown()
        {
            BindActivityDropDown();
            BindActivityTypeDropDown();
            BindHoursDropDown();
            BindMinutesDropDown();
            BindUsersDropDown();
        }

        private void BindDropDown()
        {
            BindActivityDropDown();
            BindActivityTypeDropDown();
            BindHoursDropDown();
            BindMinutesDropDown();
        }
        private void BindEditDropDown(int activityType)
        {
            BindActivityDropDown(activityType);
            BindActivityTypeDropDown();
            BindHoursDropDown();
            BindMinutesDropDown();
        }
        private void BindUsersDropDown()
        {
            #region Get All Users Data
            var users = _userBLL.GetActiveUser();
            List<SelectListItem> UsersList;
            UsersList = users.DataList.Select(a => new SelectListItem()
            {
                Text = $"{a.FirstName} {a.LastName}",
                Value = a.Id
            }).ToList();
            UsersList.Insert(0, new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });
            ViewBag.UsersList = UsersList;

            #endregion
        }

        private void BindActivityDropDown(int activityType)
        {
            #region Get Activity Data
            var activityList = _activityBLL.GetAllActivities();
            List<SelectListItem> ActivityList = new List<SelectListItem>();

            ActivityList = activityList.DataList.Where(s => s.ActivityTypeId == activityType)
                .Select(a => new SelectListItem()
                {
                    Text = a.ActivityName,
                    Value = a.ActivityId.ToString()
                }).ToList();

            ActivityList.Insert(0, new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });

            ViewBag.ActivityList = ActivityList;

            #endregion
        }

        private void BindActivityDropDown()
        {
            #region Get Activity Data
            var activityList = _activityBLL.GetAllActivities();
            List<SelectListItem> ActivityList = new List<SelectListItem>();
            ActivityList.Insert(0, new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });
            ActivityList = activityList.DataList.Select(a => new SelectListItem()
            {
                Text = a.ActivityName,
                Value = a.ActivityId.ToString()
            }).ToList();

            ViewBag.ActivityList = activityList.DataList;


            var emptyActivityList = new List<SelectListItem>();

            emptyActivityList.Add(new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });

            ViewBag.EmptyActivityList = emptyActivityList;
            #endregion
        }
        private void BindActivityTypeDropDown()
        {

            #region Get Activity Type
            var activityType = _activityBLL.GetAllActivityType();
            List<SelectListItem> ActivityTypeList;
            ActivityTypeList = activityType.DataList.Select(a => new SelectListItem()
            {
                Text = a.ActivityTypeName,
                Value = a.ActivityTypeId.ToString()
            }).ToList();
            ActivityTypeList.Insert(0, new SelectListItem
            {
                Text = "-- Select --",
                Value = null,
                Disabled = true
            });
            ViewBag.ActivityTypeList = ActivityTypeList;
            #endregion
        }
        private void BindHoursDropDown()
        {

            #region Hours 
            List<SelectListItem> HoursList = new List<SelectListItem>();
            for (var i = 1; i <= 24; i++)
            {
                HoursList.Insert(i - 1, new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == 1
                });
            }
            HoursList.Insert(0, new SelectListItem
            {
                Text = "0",
                Value = null,
                Disabled = true
            });
            ViewBag.HoursList = HoursList;
            #endregion
        }
        private void BindMinutesDropDown()
        {
            #region Minutes
            List<SelectListItem> MinutesList = new List<SelectListItem>();
            for (var i = 1; i <= 60; i++)
            {
                MinutesList.Insert(i - 1, new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == 1
                });
            }
            MinutesList.Insert(0, new SelectListItem
            {
                Text = "0",
                Value = null,
                Disabled = true
            });
            ViewBag.MinutesList = MinutesList;
            #endregion
        }

        private Tuple<bool, int?, int?> GetExistingTimeSheet(DateTime timesheetDate)
        {
            var weekTimesheetData = _timesheetBLL.GetTimesheetByDate(timesheetDate, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (weekTimesheetData.DataList.Any())
            {
                return new Tuple<bool, int?, int?>(true, weekTimesheetData.Data.TotalHours, weekTimesheetData.Data.TotalMinutes);
            }
            return new Tuple<bool, int?, int?>(false, 0, 0);
        }


        private string RenderViewAsync(Controller controller, string viewName, object model, bool partial = false)
        {
            try
            {
                if (string.IsNullOrEmpty(viewName))
                {
                    viewName = controller.ControllerContext.ActionDescriptor.ActionName;
                }

                controller.ViewData.Model = model;

                using (var writer = new StringWriter())
                {
                    IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                    ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                    ViewContext viewContext = new ViewContext(
                        controller.ControllerContext,
                        viewResult.View,
                        controller.ViewData,
                        controller.TempData,
                        writer,
                        new HtmlHelperOptions()
                    );

                    viewResult.View.RenderAsync(viewContext);

                    return writer.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        #endregion

        #endregion

    }
}
