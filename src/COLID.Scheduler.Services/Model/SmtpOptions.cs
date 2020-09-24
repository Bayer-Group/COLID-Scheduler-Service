namespace COLID.Scheduler.Services.Model
{
    public class SmtpOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
    }
}
