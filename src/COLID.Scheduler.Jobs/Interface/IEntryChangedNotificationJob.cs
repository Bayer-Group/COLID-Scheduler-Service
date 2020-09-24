namespace COLID.SchedulerService.Jobs.Interface
{
    /// <summary>
    /// Job to distribute the notification in case an entry, to which a user subscribed, changed.
    /// </summary>
    public interface IEntryChangedNotificationJob : IJob
    {
    }
}
