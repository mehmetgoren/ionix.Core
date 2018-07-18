namespace SQLog
{
    using System;
    using ionix.Data;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Log")]
    internal class LogEntity
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int Id { get; set; }

        public DateTime OpDate { get; set; }

        public string Type { get; set; }
        public string Method { get; set; }
        public int ThreadId { get; set; }

        public int? Code { get; set; }
        public string LogType { get; set; }
        public string Message { get; set; }
        public string ObjJson { get; set; }

        public long? Elapsed { get; set; }

        public LogEntity Copy()
        {
            LogEntity copy = new LogEntity();

            copy.OpDate = this.OpDate;

            copy.Type = this.Type;
            copy.Method = this.Method;
            copy.ThreadId = this.ThreadId;

            copy.Code = this.Code;
            copy.LogType = this.LogType;
            copy.Message = this.Message;
            copy.ObjJson = this.ObjJson;

            copy.Elapsed = this.Elapsed;

            return copy;
        }

        public void Clear()
        {
            this.Code = null;
            this.LogType = null;
            this.Message = null;
            this.ObjJson = null;
            this.Elapsed = null;
        }

        public static string CreateSql()
        {
            return @"
            CREATE TABLE Log (
                Id       INTEGER        PRIMARY KEY AUTOINCREMENT,
                OpDate   DATETIME       NOT NULL,
                Type     VARCHAR (250)  NOT NULL,
                Method   VARCHAR (250)  NOT NULL,
                ThreadId INT            NOT NULL,
                Code     INT,
                LogType  VARCHAR (7),
                Message  VARCHAR (1000),
                ObjJson  TEXT,
                Elapsed  BIGINT
            );

            CREATE INDEX ix_Log_OpDate ON Log (
                OpDate
            );";
        }
    }
}
