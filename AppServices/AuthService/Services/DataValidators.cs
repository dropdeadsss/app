using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AuthService.Services
{
    public static class DataValidators
    {
        public static bool ValidateEmail(string? email)
        {
            if (email == null)
                return false;

            try
            {
                MailAddress mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidatePhone(string? phone)
        {
            string pattern = @"^\+7\d{10}$";

            if (phone == null)
                return false;

            if (Regex.IsMatch(phone, pattern))
                return true;
            else
                return false;
        }
    }
}
