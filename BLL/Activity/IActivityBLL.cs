using DataObjects.DTO;

namespace BLL.Activity
{
    public interface IActivityBLL
    {
        ResultModel<ActivityTypeDto> GetAllActivityType();
        ResultModel<ActivityDto> GetAllActivities();
        ResultModel<ActivityDto> GetActivityByActivityTypeId(int activityTypeId);
        ResultModel<ActivityTypeDto> GetActivityTypeByID(int activityTypeId);
        ResultModel DeleteActivityTypeByID(int activityTypeId);
        ResultModel<ActivityDto> GetActivityByID(int activityId);
        ResultModel DeleteActivityByID(int activityId);
        ResultModel AddActivity(string activityName, int activityType);
        ResultModel UpdateActivity(int activityId, string activityName, int activityType);
        ResultModel AddActivityType(string activityTypeName);
        ResultModel UpdateActivityType(int activityTypeId, string activityTypeName);
    }
}
