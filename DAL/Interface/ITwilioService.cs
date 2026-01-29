using System.Threading.Tasks;

namespace DAL.Interface
{
    /// <summary>
    /// Contract for handling WhatsApp communications via Meta Cloud API
    /// </summary>
    public interface ITwilioService
    {
        /// <summary>
        /// Sends a WhatsApp message using a pre-defined Meta Content Template
        /// </summary>

        /// <param name="userPhoneNumber">The user's mobile number (e.g., +91...)</param>
        /// <param name="otpCode">The 6-digit OTP code to inject into the template</param>
        Task SendOtpWhatsAppAsync(string userPhoneNumber, string otpCode);

        /// <summary>
        /// Sends a status update or notification using a Meta template
        /// </summary>
        /// <param name="userPhoneNumber">Recipient number</param>
        /// <param name="templateName">Name of the template (e.g., status_update)</param>
        /// <param name="parameters">Values for the template placeholders</param>
        Task SendStatusUpdateAsync(string userPhoneNumber, string templateName, string[] parameters);
    }
}

