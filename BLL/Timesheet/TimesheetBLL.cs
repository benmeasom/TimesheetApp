using DAL;
using DataObjects.DTO;
using DataObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class TimesheetBLL : ITimesheetBLL
    {
        #region Variables

        private readonly ITimesheetDAL _timesheetDAL;

        #endregion

        #region Constructor
        public TimesheetBLL(ITimesheetDAL timesheetDAL)
        {
            _timesheetDAL = timesheetDAL;
        }
        #endregion

        #region Timesheet
        public ResultModel AddTimesheet(TimesheetDTO timesheet)
        {
            ResultModel result = new ResultModel();

            try
            {
                result = _timesheetDAL.AddTimesheet(new Timesheet
                {
                    Name = timesheet.Name,
                    TimesheetDate = timesheet.TimesheetDate,
                    TotalHours = timesheet.TotalHours.Value,
                    TotalMinutes = timesheet.TotalMinutes.Value,
                    WeekEndingDate = timesheet.WeekEndingDate,
                    UserId = timesheet.UserId,
                    TimesheetItems = timesheet.TimesheetItems.Select(s => new TimesheetItems
                    {
                        Hours = s.Hours.Value,
                        Minutes = s.Minutes.Value,
                        ActivityId = s.ActivityId.Value,
                        ActivityTypeId = s.ActivityTypeId.Value,
                        Notes = s.Notes
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }
            return result;
        }

        public List<TimesheetDTO> GetTimesheet(TimesheetDTO timesheet)
        {
            return _timesheetDAL.GetTimesheet(timesheet);
        }

        public ResultModel<TimesheetDTO> GetTimesheetDatesByEndDate(DateTime endDate, string userId)
        {
            return _timesheetDAL.GetTimesheetDatesByEndDate(endDate, userId);
        }

        public ResultModel<TimesheetDTO> GetTimesheetById(int timesheetId, string userId)
        {
            return _timesheetDAL.GetTimesheetById(timesheetId, userId);
        }

        #endregion

        #region TimesheetItems
        public ResultModel<Boolean> SaveTimesheetItems(TimesheetItems timesheetItems)
        {
            return _timesheetDAL.SaveTimesheetItems(timesheetItems);
        }

        public ResultModel<Boolean> EditTimesheetItems(TimesheetItems timesheetItems)
        {
            return _timesheetDAL.EditTimesheetItems(timesheetItems);
        }

        public List<TimesheetItemsDTO> GetTimesheetItems(TimesheetItemsDTO timesheetItems)
        {
            return _timesheetDAL.GetTimesheetItems(timesheetItems);
        }

        #endregion

        #region Review Timesheet

        public ResultModel<ReviewTimesheet> GetTimesheetByEndDate(DateTime endDate, string userId)
        {
            return _timesheetDAL.GetTimesheetByEndDate(endDate, userId);
        }

        public ResultModel<ReviewTimesheetItems> GetTimesheetDetailsByTimesheetId(long timesheetId, DateTime endDate, string userId)
        {
            return _timesheetDAL.GetTimesheetDetailsByTimesheetId(timesheetId, endDate, userId);
        }

        public ResultModel<ReviewTimesheet> GetTimesheetByDate(DateTime date, string userId)
        {
            return _timesheetDAL.GetTimesheetByDate(date, userId);
        }

        public ResultModel SaveTimesheet(TimesheetDTO timesheet)
        {
            ResultModel result = new ResultModel();

            try
            {
                result = _timesheetDAL.SaveTimesheet(new Timesheet
                {
                    Name = timesheet.Name,
                    TimesheetDate = timesheet.TimesheetDate,
                    TotalHours = timesheet.TotalHours.Value,
                    TotalMinutes = timesheet.TotalMinutes.Value,
                    WeekEndingDate = timesheet.WeekEndingDate,
                    UserId = timesheet.UserId,
                    TimesheetId = timesheet.TimesheetId,
                    TimesheetItems = timesheet.TimesheetItems.Select(s => new TimesheetItems
                    {
                        TimesheetId = s.TimesheetId,
                        TimesheetItemsId =  s.TimesheetItemsId,
                        Hours = s.Hours.Value,
                        Minutes = s.Minutes.Value,
                        ActivityId = s.ActivityId.Value,
                        ActivityTypeId = s.ActivityTypeId.Value,
                        Notes = s.Notes
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }
            return result;
        }

        #endregion
    }
}
