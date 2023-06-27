namespace COLID.SchedulerService.Jobs.Interfaces
{
    /// <summary>
    /// Job to trigger the execution of stored queries.
    /// </summary>
    public interface IStoredQueriesExecutionJob : IJob
    {
        public void NotifyAllUsersForNewStoredQueryUpdates();
    }
}
