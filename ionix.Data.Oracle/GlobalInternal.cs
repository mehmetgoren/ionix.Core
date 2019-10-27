namespace Ionix.Data.Oracle
{
    using System;

    internal static class GlobalInternal
    {
        internal const char Prefix = ':';

        internal static readonly string OracleBeginStatement = "BEGIN" + Environment.NewLine;
        internal static readonly string OracleEndStatement = Environment.NewLine + "END;";

    }

    internal sealed class ValueSetter : DbValueSetter
    {
        internal static readonly ValueSetter Instance = new ValueSetter();

        private ValueSetter()
        {

        }

        public override char Prefix => GlobalInternal.Prefix;
    }
}
