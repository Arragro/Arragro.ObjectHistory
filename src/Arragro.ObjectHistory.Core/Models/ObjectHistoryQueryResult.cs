﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResult
    {
        public Guid Folder { get; set; }
        public string RowKey { get; set; }
        public string ApplicationName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ObjectHistoryQueryResult(ObjectHistoryEntity objectHistoryGlobalEntity)
        {
            Folder = objectHistoryGlobalEntity.Folder;
            RowKey = objectHistoryGlobalEntity.RowKey;
            ApplicationName = objectHistoryGlobalEntity.ApplicationName;
            ModifiedBy = objectHistoryGlobalEntity.User;
            ModifiedDate = objectHistoryGlobalEntity.OriginTimestamp;
        }
    }
}
