using DataObjects.DTO;
using DataObjects.Models;
using System;
using System.Collections.Generic;

namespace DAL
{
    public interface ITimesheetDAL
    {
        ResultModel AddTimesheet(Timesheet timesheet);
        ResultModel<Boolean> EditTimesheet(Timesheet timesheet);
        List<TimesheetDTO> GetTimesheet(TimesheetDTO timesheet);
        ResultModel<Boolean> SaveTimesheetItems(TimesheetItems timesheetItems);
        ResultModel<Boolean> EditTimesheetItems(TimesheetItems timesheetItems);
        List<TimesheetItemsDTO> GetTimesheetItems(TimesheetItemsDTO timesheetItems);
        ResultModel<ReviewTimesheet> GetTimesheetByEndDate(DateTime endDate, string userId);
        ResultModel<ReviewTimesheetItems> GetTimesheetDetailsByTimesheetId(long timesheetId, DateTime endDate, string userId);
        ResultModel<ReviewTimesheet> GetTimesheetByDate(DateTime date, string userId);
        ResultModel<TimesheetDTO> GetTimesheetDatesByEndDate(DateTime endDate, string userId);
        ResultModel<TimesheetDTO> GetTimesheetById(int timesheetId, string userId);
        ResultModel SaveTimesheet(Timesheet timesheet);
    }
}
