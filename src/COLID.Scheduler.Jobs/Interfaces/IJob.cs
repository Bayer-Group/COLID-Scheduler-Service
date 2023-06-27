using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace COLID.SchedulerService.Jobs.Interfaces
{
    /// <summary>
    /// Base interface for jobs.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Main execution function for all jobs. 
        /// </summary>
        /// <param name="token">the cancellation token</param>
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        Task ExecuteAsync(CancellationToken token);
    }
}
