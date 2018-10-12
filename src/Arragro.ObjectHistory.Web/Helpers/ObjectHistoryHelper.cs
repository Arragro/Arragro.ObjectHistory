using System;

namespace Arragro.ObjectHistory.Web.Helpers
{
    public static class ObjectHistoryHelper
    {
        public static string GetObjectHistoryFullNameAndId(Type type, string id)
        {
            return $"{type.FullName}-{id}";
        }
    }
}
