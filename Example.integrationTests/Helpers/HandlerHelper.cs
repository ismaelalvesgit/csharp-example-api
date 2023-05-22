namespace Example.integrationTests.Helpers
{
    public static class HandlerHelper
    {
        public static string ToQueryString(object query)
        {
            var parameters = query.GetType().GetProperties().Where(x => x.GetValue(query) != null).ToDictionary(d => d.Name, p => p.GetValue(query));

            return "?" + string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }
    }
}
