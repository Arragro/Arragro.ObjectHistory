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
                if (this.IsAdd)
                {
                    return new ObjectHistoryDetailRaw(ObjectHistorySettingsBase, PartitionKey, RowKey, ApplicationName, TimeStamp, User, Folder, SecurityValidationToken, SubFolder, IsAdd)
                    {
                        NewJson = NewJson.ToString(),
                        OldJson = null,
                        Diff = null
                    };
                }
                else
                {
                    return new ObjectHistoryDetailRaw(ObjectHistorySettingsBase, PartitionKey, RowKey, ApplicationName, TimeStamp, User, Folder, SecurityValidationToken, SubFolder, IsAdd)
                    {
                        NewJson = NewJson.ToString(),
                        OldJson = this.OldJson.ToString(),
                        Diff = Diff.ToString()
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
