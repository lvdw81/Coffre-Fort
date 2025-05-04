using System.Collections.Concurrent;

namespace coffre_fort_api.Services
{
    public static class A2F
    {
        public static ConcurrentDictionary<string, string> Codes = new();
    }
}
