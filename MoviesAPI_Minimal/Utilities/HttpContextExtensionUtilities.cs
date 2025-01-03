namespace MoviesAPI_Minimal.Utilities
{
    public static class HttpContextExtensionUtilities
    {
        public static T ExtractValueOrDefault<T>(this HttpContext context, string field, T defaultValue) 
            where T : IParsable<T>
        {
            var value = context.Request.Query[field];
            if (!value.Any())
            {
                return defaultValue;
            }
            return T.Parse(value!, null);
        }
    }
}
