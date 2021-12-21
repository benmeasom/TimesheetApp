using DAL.Activities;
using DataObjects.DTO;

namespace BLL.Activity
{
    public class ActivityBLL : IActivityBLL
    {
        private readonly IActivityDAL _activityDAL;
        public ActivityBLL(IActivityDAL activityDAL)
        {
            _activityDAL = activityDAL;
        }

        public ResultModel<ActivityDto> GetActivityByActivityTypeId(int activityTypeId)
        {
            return _activityDAL.GetActivityByActivityTypeId(activityTypeId);
        }

        public ResultModel<ActivityDto> GetAllActivities()
        {
            return _activityDAL.GetAllActivities();
        }

        public ResultModel<ActivityTypeDto> GetAllActivityType()
        {
            return _activityDAL.GetAllActivityType();
        }
        public ResultModel<ActivityTypeDto> GetActivityTypeByID(int activityTypeId)
        {
            return _activityDAL.GetActivityTypeByID(activityTypeId);
        }

        public ResultModel DeleteActivityTypeByID(int activityTypeId)
        {
            return _activityDAL.DeleteActivityTypeByID(activityTypeId);
        }

        public ResultModel<ActivityDto> GetActivityByID(int activityId)
        {
            return _activityDAL.GetActivityByID(activityId);
        }

        public ResultModel DeleteActivityByID(int activityId)
        {
            return _activityDAL.DeleteActivityByID(activityId);
        }

        public ResultModel AddActivity(string activityName, int activityType)
        {
            return _activityDAL.AddActivity(activityName, activityType);
        }

        public ResultModel UpdateActivity(int activityId, string activityName, int activityType)
        {
            return _activityDAL.UpdateActivity(activityId, activityName, activityType);
        }

        public ResultModel AddActivityType(string activityTypeName)
        {
            return _activityDAL.AddActivityType(activityTypeName);
        }

        public ResultModel UpdateActivityType(int activityTypeId, string activityTypeName)
        {
            return _activityDAL.UpdateActivityType(activityTypeId, activityTypeName);
        }
    }
}
