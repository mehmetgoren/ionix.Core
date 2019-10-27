namespace Ionix.Migration
{
    using System;

    public abstract class DatabaseVersionBase
    {
        private MigrationVersion _version;
        public virtual string Version
        {
            get => this._version;
            set => this._version = value;
        }

        public virtual string Description { get; set; }

        public virtual DateTime? StartedOn { get; set; }

        public virtual DateTime? CompletedOn { get; set; }

        public virtual string Script { get; set; }

        public virtual string Warning { get; set; }

        public virtual string Exception { get; set; }

        public virtual bool? BuiltIn { get; set; }


        public virtual void SetValuesFrom(Migration migration)
        {
            Version = migration.Version;
            StartedOn = DateTime.Now;
            Description = migration.Description;
            Script = migration.GenerateQuery()?.ToString();
            BuiltIn = migration.IsBuiltIn;
        }

        public override string ToString()
        {
            return $"{this.Version} started on {this.StartedOn} completed on {this.CompletedOn}";
        }
    }
}
