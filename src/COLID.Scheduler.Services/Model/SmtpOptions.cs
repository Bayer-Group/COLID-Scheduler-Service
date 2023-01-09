namespace COLID.Scheduler.Services.Model
{
    public class SmtpOptions
    {
        /// <summary>
        /// Indicate whether mails should be sent using the SES service by AWS. True = Send via AWS SES SDK, False = Use SMTP
        /// </summary>
        public bool useSES { get; set; }
        /// <summary>
        /// If using SES: specify AWS region, e.g. "eu-central-1"
        /// </summary>
        public string AwsRegion { get; set; }
        /// <summary>
        /// SMTP Server used for sending emails. Only required when useSES = false
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// SMTP Port. Only required when useSES = false
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Use SSL for sending emails. Only required when useSES = false
        /// </summary>
        public bool EnableSsl { get; set; }
        /// <summary>
        /// SMTP User name. Only required when useSES = false
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// SMTP Password. Only required when useSES = false
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Sender email address for the emails. Required.
        /// </summary>
        public string Sender { get; set; }
    }
}
