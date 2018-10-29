namespace ionix.Data.Mongo
{
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;
    using System;
    using System.Threading.Tasks;

    public sealed class HostInfo
    {
        public sealed class SystemInfo
        {
            [BsonElement("currentTime")]
            public DateTime CurrentTime { get; set; }

            [BsonElement("hostname")]
            public string Hostname { get; set; }

            [BsonElement("cpuAddrSize")]
            public int CpuAddrSize { get; set; }

            [BsonElement("memSizeMB")]
            public int MemSizeMB { get; set; }

            [BsonElement("numCores")]
            public int NumCores { get; set; }

            [BsonElement("cpuArch")]
            public string CpuArch { get; set; }

            [BsonElement("numaEnabled")]
            public bool NumaEnabled { get; set; }
        }

        public sealed class OsInfo
        {
            [BsonElement("type")]
            public string Type { get; set; }

            [BsonElement("name")]
            public string Name { get; set; }

            [BsonElement("version")]
            public string Version { get; set; }
        }

        public sealed class ExtraInfo
        {
            [BsonElement("versionString")]
            public string VersionString { get; set; }

            [BsonElement("libcVersion")]
            public string LibcVersion { get; set; }

            [BsonElement("versionSignature")]
            public string VersionSignature { get; set; }

            [BsonElement("kernelVersion")]
            public string kernelVersion { get; set; }

            [BsonElement("cpuFrequencyMHz")]
            public string cpuFrequencyMHz { get; set; }

            [BsonElement("cpuFeatures")]
            public string cpuFeatures { get; set; }

            [BsonElement("pageSize")]
            public long PageSize { get; set; }

            [BsonElement("numPages")]
            public int NumPages { get; set; }

            [BsonElement("maxOpenFiles")]
            public int MaxOpenFiles { get; set; }
        }

        [BsonElement("system")]
        public SystemInfo System { get; set; }

        [BsonElement("os")]
        public OsInfo Os { get; set; }

        [BsonElement("extra")]
        public ExtraInfo Extra { get; set; }

        [BsonElement("ok")]
        public double Ok { get; set; }
    }

    partial class MongoAdmin
    {
        private sealed class RetVal<T>
        {
            [BsonElement("retval")]
            internal T Retval { get; set; }

            [BsonElement("ok")]
            public double Ok { get; set; }
        }

        public static HostInfo GetHostInfo(IMongoDatabase db) => ExecuteScript<RetVal<HostInfo>>(db, "db.hostInfo();")?.Retval;
        public static async Task<HostInfo> GetHostInfoAsync(IMongoDatabase db) => (await ExecuteScriptAsync<RetVal<HostInfo>>(db, "db.hostInfo();"))?.Retval;
    }
}
