namespace AuthService.Models
{
    public static class Errors
    {
        public static string GetError(int code)
        {
            Dictionary<int,string> errors = new Dictionary<int, string>();
            errors.Add(503,"Не удается получить доступ к сервису.");

            return errors.ContainsKey(code) ? errors[code] : string.Empty;
        }
    }
}
