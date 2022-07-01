namespace COLID.SchedulerService.Jobs.Interface
{
    /// <summary>
    /// Job to distribute the deletion of stored messages.
    /// </summary>
    public interface IMessageDeletionJob : IJob
    {
        /// <summary>
        /// Deletes all expired Messages
        /// </summary> 
        public void DeleteExpiredMessages();
    }
     
}
