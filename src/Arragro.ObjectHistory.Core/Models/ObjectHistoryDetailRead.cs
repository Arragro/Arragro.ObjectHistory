using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetailRead : ObjectHistoryDetailBase
    {
        public ObjectHistoryDetailRead(ObjectHistoryDetailRaw objectHistoryDetailRaw)
            : base(objectHistoryDetailRaw.PartitionKey, objectHistoryDetailRaw.RowKey, objectHistoryDetailRaw.ApplicationName, objectHistoryDetailRaw.TimeStamp, 
                   objectHistoryDetailRaw.User, objectHistoryDetailRaw.Folder, objectHistoryDetailRaw.IsAdd, objectHistoryDetailRaw.SubFolder, 
                   objectHistoryDetailRaw.SecurityValidationToken)
        {
            ObjectHistorySettingsBase = objectHistoryDetailRaw.ObjectHistorySettingsBase;
            NewJson = objectHistoryDetailRaw.NewJson;
            OldJson = objectHistoryDetailRaw.OldJson;
            Diff = objectHistoryDetailRaw.Diff;
            Version = objectHistoryDetailRaw.Version;
        }

        protected ObjectHistoryDetailRead() : base() { }

        public ObjectHistorySettingsBase ObjectHistorySettingsBase { get; set; }
        public object NewJson { get; set; }
        public object OldJson { get; set; }
        public object Diff { get; set; }

        public ObjectHistoryDetailRaw GetObjectHistoryDetailRaw()
        {
            try
            {
                if (IsAdd)
                {
                    return new ObjectHistoryDetailRaw(ObjectHistorySettingsBase, PartitionKey, RowKey, ApplicationName, TimeStamp, User, Folder, SecurityValidationToken, SubFolder, IsAdd)
                    {
                        NewJson = NewJson.ToString(),
                        OldJson = null,
                        Diff = null,
                        Version = Version
                    };
                }
                else
                {
                    return new ObjectHistoryDetailRaw(ObjectHistorySettingsBase, PartitionKey, RowKey, ApplicationName, TimeStamp, User, Folder, SecurityValidationToken, SubFolder, IsAdd)
                    {
                        NewJson = NewJson.ToString(),
                        OldJson = OldJson != null ? OldJson.ToString() : null,
                        Diff = Diff != null ? Diff.ToString() : null,
                        Version = Version
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
