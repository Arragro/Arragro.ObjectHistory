using System;

namespace Arragro.ObjectHistory.RazorClassLib.Helpers
{
    public static class ObjectHistoryHelper
    {
        public static string GetObjectHistoryFullNameAndId(Type type, string id)
        {
            return $"{type.FullName}-{id}";
        }
    }
}
