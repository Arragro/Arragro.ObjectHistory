﻿using Azure;
using Newtonsoft.Json;
using System;

namespace Arragro.ObjectHistory.AzureStorage
{
    public class ObjectHistoryTableEntity : Azure.Data.Tables.ITableEntity
    {
        public int Verion { get; set; }
        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public string User { get; set; }
        public bool IsAdd { get; set; }
        public string Metadata { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }
    }
}
