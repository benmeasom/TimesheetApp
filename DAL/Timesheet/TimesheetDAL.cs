using System;
using System.Collections.Generic;
using DataObjects.Context;
using DataObjects.DTO;
using DataObjects.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class TimesheetDAL : ITimesheetDAL
    {
        #region Variables

        private readonly TimesheetDBContext _context;

        #endregion

        #region Constructor
        public TimesheetDAL(TimesheetDBContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods

        #region Timesheet
        public ResultModel AddTimesheet(Timesheet timesheet)
        {
            ResultModel result = new ResultModel();

            try
            {
                _context.Timesheet.Add(timesheet);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.IdentityId = timesheet.TimesheetId;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<Boolean> EditTimesheet(Timesheet timesheet)
        {
            ResultModel<Boolean> result = new ResultModel<Boolean>();

            try
            {
                Timesheet currentData = _context.Timesheet.Find(timesheet.TimesheetId);
                _context.Entry(currentData).CurrentValues.SetValues(timesheet);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.IdentityId = 0;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public List<TimesheetDTO> GetTimesheet(TimesheetDTO timesheet)
        {
            List<TimesheetDTO> result = new List<TimesheetDTO>();

            try
            {
                var _Query = from t in _context.Timesheet

                             select new TimesheetDTO
                             {
                                 TimesheetId = t.TimesheetId,
                                 TimesheetDate = t.TimesheetDate,
                                 UserId = t.UserId
                             };

                result = _Query.OrderBy(x => x.TimesheetId).ToList();

            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public ResultModel<TimesheetDTO> GetTimesheetDatesByEndDate(DateTime endDate, string userId)
        {
            ResultModel<TimesheetDTO> response = new ResultModel<TimesheetDTO>();
            try
            {
                var existingTimesheet = _context.Timesheet.Where(s => s.WeekEndingDate.Date == endDate.Date && s.UserId == userId).Select(s => new TimesheetDTO
                {
                    WeekEndingDate = s.WeekEndingDate,
                    TimesheetDate = s.TimesheetDate
                });
                response.DataList = existingTimesheet.ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Msg = ex.Message;
            }
            return response;
        }

        public ResultModel<TimesheetDTO> GetTimesheetById(int timesheetId, string userId)
        {
            ResultModel<TimesheetDTO> result = new ResultModel<TimesheetDTO>();
            try
            {
                var timesheet = _context.Timesheet.Include(a => a.TimesheetItems).FirstOrDefault(f => f.TimesheetId == timesheetId && f.UserId == userId);
                var user = _context.TimesheetUsers.FirstOrDefault(f => f.Id == userId);
                result.Data = new TimesheetDTO
                {
                    TimesheetId = timesheet.TimesheetId,
                    TimesheetDate = timesheet.TimesheetDate,
                    Name = timesheet.Name,
                    TotalHours = timesheet.TotalHours,
                    TotalMinutes = timesheet.TotalMinutes,
                    UserId = timesheet.UserId,
                    WeekEndingDate = timesheet.WeekEndingDate,
                    TimesheetUser = new UserDto
                    {
                        FirstName = user?.FirstName,
                        LastName = user?.LastName
                    },
                    TimesheetItems = timesheet.TimesheetItems.Select(s => new TimesheetItemsDTO
                    {
                        ActivityId = s.ActivityId,
                        ActivityTypeId = s.ActivityTypeId,
                        Hours = s.Hours,
                        Minutes = s.Minutes,
                        Notes = s.Notes,
                        Saved = 1,
                        TimesheetId = s.TimesheetId,
                        TimesheetItemsId = s.TimesheetItemsId
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }
        #endregion

        #region TimesheetItems
        public ResultModel<Boolean> SaveTimesheetItems(TimesheetItems timesheetItems)
        {
            ResultModel<Boolean> result = new ResultModel<Boolean>();

            try
            {
                _context.TimesheetItems.Add(timesheetItems);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.IdentityId = timesheetItems.TimesheetItemsId;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<Boolean> EditTimesheetItems(TimesheetItems timesheetItems)
        {
            ResultModel<Boolean> result = new ResultModel<Boolean>();

            try
            {
                TimesheetItems currentData = _context.TimesheetItems.Find(timesheetItems.TimesheetItemsId);
                _context.Entry(currentData).CurrentValues.SetValues(timesheetItems);
                _context.SaveChanges();
                result.IsSuccess = true;
                result.IdentityId = 0;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public List<TimesheetItemsDTO> GetTimesheetItems(TimesheetItemsDTO timesheetItems)
        {
            List<TimesheetItemsDTO> result = new List<TimesheetItemsDTO>();

            try
            {
                var _Query = from t in _context.TimesheetItems

                             select new TimesheetItemsDTO
                             {
                                 TimesheetId = t.TimesheetId,
                                 TimesheetItemsId = t.TimesheetItemsId,
                                 ActivityId = t.ActivityId,
                                 Hours = t.Hours,
                                 Minutes = t.Minutes,
                                 Notes = t.Notes,
                             };

                result = _Query.OrderBy(x => x.TimesheetId).ToList();

            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }


        #endregion

        #region Review Timesheet
        public ResultModel<ReviewTimesheet> GetTimesheetByEndDate(DateTime endDate, string userId)
        {
            ResultModel<ReviewTimesheet> result = new ResultModel<ReviewTimesheet>();

            try
            {
                var list = _context.Timesheet.Where(s => s.UserId == userId && s.WeekEndingDate.Date == endDate.Date).Select(s => new ReviewTimesheet
                {
                    Date = s.TimesheetDate,
                    DayName = s.TimesheetDate.DayOfWeek.ToString(),
                    TimeSheetId = s.TimesheetId,
                    TotalHours = s.TotalHours,
                    TotalMinutes = s.TotalMinutes
                }).ToList();

                result.DataList = list.GroupBy(s => s.Date).Select(s => new ReviewTimesheet
                {
                    Date = s.Key,
                    DayName = s.FirstOrDefault().DayName,
                    TimeSheetId = s.FirstOrDefault().TimeSheetId,
                    TotalHours = GetTime(s.Sum(ss => ss.TotalHours) ?? 0, s.Sum(ss => ss.TotalMinutes) ?? 0).Item1,
                    TotalMinutes = GetTime(s.Sum(ss => ss.TotalHours) ?? 0, s.Sum(ss => ss.TotalMinutes) ?? 0).Item2,
                }).ToList();
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.IsSuccess = false;
            }

            return result;
        }

        private Tuple<int, int> GetTime(int totalHours, int totalMinutes)
        {
            var time = TimeSpan.FromMinutes(totalMinutes);
            var TotalMinutes = Convert.ToInt32(time.Minutes);
            var TotalHours = totalHours + time.Hours + (time.Days * 24);
            return new Tuple<int, int>(TotalHours, TotalMinutes);

        }


        public ResultModel<ReviewTimesheetItems> GetTimesheetDetailsByTimesheetId(long timesheetId, DateTime endDate, string userId)
        {
            ResultModel<ReviewTimesheetItems> result = new ResultModel<ReviewTimesheetItems>();

            try
            {
                var date = _context.Timesheet.FirstOrDefault(s => s.TimesheetId == timesheetId)?.TimesheetDate;

                result.DataList = (from timesheetDetails in _context.TimesheetItems
                                   join timesheet in _context.Timesheet on timesheetDetails.TimesheetId equals timesheet.TimesheetId
                                   join activity in _context.Activities on timesheetDetails.ActivityId equals activity.ActivityId
                                   join activityType in _context.ActivityTypes on timesheetDetails.ActivityTypeId equals activityType.ActivityTypeId
                                   where (timesheetId == 0 ? timesheet.WeekEndingDate.Date == endDate.Date : timesheet.TimesheetDate.Date == date.Value.Date) && timesheet.UserId == userId
                                   select new ReviewTimesheetItems
                                   {
                                       Date = timesheet.TimesheetDate,
                                       ActivityName = activity.ActivityName,
                                       ActivityTypeName = activityType.ActivityTypeName,
                                       Hours = timesheetDetails.Hours,
                                       Minutes = timesheetDetails.Minutes,
                                       Notes = timesheetDetails.Notes
                                   }).OrderBy(o => o.Date).ToList();
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<ReviewTimesheet> GetTimesheetByDate(DateTime date, string userId)
        {
            ResultModel<ReviewTimesheet> result = new ResultModel<ReviewTimesheet>();

            try
            {
                result.DataList = _context.Timesheet.Where(s => s.UserId == userId && s.TimesheetDate.Date == date.Date).Select(s => new ReviewTimesheet
                {
                    TotalHours = s.TotalHours,
                    TotalMinutes = s.TotalMinutes
                }).ToList();

                result.Data = new ReviewTimesheet
                {
                    TotalHours = result.DataList.Sum(s => s.TotalHours),
                    TotalMinutes = result.DataList.Sum(s => s.TotalMinutes)
                };
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel SaveTimesheet(Timesheet timesheet)
        {
            ResultModel result = new ResultModel();

            try
            {
                var existingTimesheet = _context.Timesheet.FirstOrDefault(w => w.TimesheetId == timesheet.TimesheetId && w.UserId == timesheet.UserId);
                if (existingTimesheet == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "No timesheet exists.";
                    return result;
                }
                var existingTimesheetItems = _context.TimesheetItems.Where(s => s.TimesheetId == timesheet.TimesheetId);
                _context.TimesheetItems.RemoveRange(existingTimesheetItems);

                existingTimesheet.TotalHours = timesheet.TotalHours;
                existingTimesheet.TotalMinutes = timesheet.TotalMinutes;
                existingTimesheet.TimesheetItems = timesheet.TimesheetItems;
                _context.Timesheet.Update(existingTimesheet);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }

        #endregion

        #endregion
    }
}
