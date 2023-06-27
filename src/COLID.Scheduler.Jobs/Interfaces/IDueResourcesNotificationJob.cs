using System.Threading.Tasks;

namespace COLID.SchedulerService.Jobs.Interfaces
{
    /// <summary>
    /// Job to distribute the deletion of stored messages.
    /// </summary>
    public interface IDueResourcesNotificationJob : IJob
    {
        /// <summary>
        /// Triggers Method to retrieve all due resources for review and notifies responsible data stewards 
        /// </summary> 
        Task NotifyDataStewardsForDueResourceReview();
    }
     
}
