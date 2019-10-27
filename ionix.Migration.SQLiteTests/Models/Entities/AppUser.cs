namespace Ionix.Migration.SQLiteTests.Models
{
    using Ionix.Data;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [MigrationVersion(Migration100.VersionNo)]
    [Table("AppUser")]
    [TableIndex("Username", Unique = true)]
    [TableForeignKey("RoleId", "Role", "RoleId")]
    public class AppUser
    {
        [DbSchema(IsKey = true, DatabaseGeneratedOption = StoreGeneratedPattern.Identity)]
        public int AppUserId { get; set; }

        public int? OpUserId { get; set; }

        [DbSchema(ColumnName = "op_date", DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime? OpDate { get; set; }

        [DbSchema(MaxLength = 15)]
        public string OpIp { get; set; }

        [DbSchema(MaxLength = 15)]
        public string IpAddress { get; set; }

        public int RoleId { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50)]
        public string Username { get; set; }

        [DbSchema(MaxLength = 150)]
        public string Password { get; set; }

        public long? LoginCount { get; set; }

        [DbSchema(MaxLength = 50)]
        public string Title { get; set; }

        [Precision(14, 2)]
        public decimal LineerScore { get; set; }

        public double LineerBiasScore { get; set; }

        public string this_added_newly { get; set; }

        [DbSchema(ColumnName = "This_Added_Nonnullable", DefaultValue = "0")]
        public int this_added_nonnullable { get; set; }
    }
}
