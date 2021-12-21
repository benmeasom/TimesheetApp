using DataObjects.Context;
using DataObjects.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DAL.Activities
{
    public class ActivityDAL : IActivityDAL
    {
        #region Variables

        private readonly TimesheetDBContext _context;

        #endregion

        #region Constructor
        public ActivityDAL(TimesheetDBContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods
        public ResultModel<ActivityTypeDto> GetAllActivityType()
        {
            ResultModel<ActivityTypeDto> result = new ResultModel<ActivityTypeDto>();

            try
            {
                var response = _context.ActivityTypes.Where(s => s.Status == 1);
                result.DataList = response.Select(s => new ActivityTypeDto
                {
                    ActivityTypeId = s.ActivityTypeId,
                    ActivityTypeName = s.ActivityTypeName
                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<ActivityDto> GetAllActivities()
        {
            ResultModel<ActivityDto> result = new ResultModel<ActivityDto>();

            try
            {
                var response = _context.Activities.Include(a => a.ActivityType).Where(s => s.ActivityType.Status == 1).Where(s => s.Status == 1);

                result.DataList = response.Select(s => new ActivityDto
                {
                    ActivityId = s.ActivityId,
                    ActivityTypeId = s.ActivityTypeId,
                    ActivityName = s.ActivityName,
                    ActivityTypeName = s.ActivityType.ActivityTypeName
                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<ActivityDto> GetActivityByActivityTypeId(int activityTypeId)
        {
            ResultModel<ActivityDto> result = new ResultModel<ActivityDto>();

            try
            {
                var response = _context.Activities.Where(w => w.ActivityTypeId == activityTypeId && w.Status == 1);

                result.DataList = response.Select(s => new ActivityDto
                {
                    ActivityId = s.ActivityId,
                    ActivityTypeId = s.ActivityTypeId,
                    ActivityName = s.ActivityName
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<ActivityTypeDto> GetActivityTypeByID(int activityTypeId)
        {
            ResultModel<ActivityTypeDto> result = new ResultModel<ActivityTypeDto>();

            try
            {
                var response = _context.ActivityTypes.FirstOrDefault(w => w.ActivityTypeId == activityTypeId && w.Status == 1);

                result.Data = new ActivityTypeDto
                {
                    ActivityTypeId = response.ActivityTypeId,
                    ActivityTypeName = response.ActivityTypeName
                };

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel DeleteActivityTypeByID(int activityTypeId)
        {
            ResultModel result = new ResultModel();

            try
            {
                var activityType = _context.ActivityTypes.FirstOrDefault(s => s.ActivityTypeId == activityTypeId);
                activityType.Status = 0;
                _context.ActivityTypes.Update(activityType);
                _context.SaveChanges();
                result.IdentityId = activityTypeId;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel<ActivityDto> GetActivityByID(int activityId)
        {
            ResultModel<ActivityDto> result = new ResultModel<ActivityDto>();

            try
            {
                var response = _context.Activities.FirstOrDefault(w => w.ActivityId == activityId && w.Status == 1);

                result.Data = new ActivityDto
                {
                    ActivityId = response.ActivityId,
                    ActivityName = response.ActivityName,
                    ActivityTypeId = response.ActivityTypeId
                };

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel DeleteActivityByID(int activityId)
        {
            ResultModel result = new ResultModel();

            try
            {
                var activityType = _context.Activities.FirstOrDefault(s => s.ActivityId == activityId);
                activityType.Status = 0;
                _context.Activities.Update(activityType);
                _context.SaveChanges();
                result.IdentityId = activityId;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel AddActivity(string activityName, int activityType)
        {
            ResultModel result = new ResultModel();

            try
            {
                DataObjects.Models.Activity data = new DataObjects.Models.Activity
                {
                    ActivityName = activityName,
                    ActivityTypeId = activityType,
                    Status = 1
                };
                _context.Activities.Add(data);
                _context.SaveChanges();
                result.IdentityId = data.ActivityId;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel UpdateActivity(int activityId, string activityName, int activityType)
        {
            ResultModel result = new ResultModel();

            try
            {
                var activity = _context.Activities.FirstOrDefault(s => s.ActivityId == activityId);
                if (activity == null)
                {
                    result.Msg = "Invalid Activity Id.";
                    result.IsSuccess = false;
                    return result;
                }
                activity.ActivityName = activityName;
                activity.ActivityTypeId = activityType;
                _context.Activities.Update(activity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel AddActivityType(string activityTypeName)
        {
            ResultModel result = new ResultModel();

            try
            {
                DataObjects.Models.ActivityType data = new DataObjects.Models.ActivityType
                {
                    ActivityTypeName = activityTypeName,
                    Status = 1
                };
                _context.ActivityTypes.Add(data);
                _context.SaveChanges();
                result.IdentityId = data.ActivityTypeId;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultModel UpdateActivityType(int activityTypeId, string activityTypeName)
        {
            ResultModel result = new ResultModel();

            try
            {
                var activityType = _context.ActivityTypes.FirstOrDefault(s => s.ActivityTypeId == activityTypeId);
                if (activityType == null)
                {
                    result.Msg = "Invalid Activity Id.";
                    result.IsSuccess = false;
                    return result;
                }
                activityType.ActivityTypeName = activityTypeName;
                _context.ActivityTypes.Update(activityType);
                _context.SaveChanges();
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
