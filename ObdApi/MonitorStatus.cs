namespace ObdApi
{
    using System;

    public class MonitorStatus : IEquatable<MonitorStatus>
    {
        private uint _value;

        internal static MonitorStatus Create(uint? value)
        {
            if (value == null)
            { return null; }

            byte a = (byte)(value >> 24);

            return new MonitorStatus
            {
                _value = value.Value,
                IsMilOn = (a & 0x80) > 1,
                DtcCount = a & 0x7f
            };
        }

        public Boolean IsMilOn { get; private set; }
        public Int32 DtcCount { get; private set; }

        public bool Equals(MonitorStatus other)
        {
            return other != null && _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MonitorStatus);
        }

        public override int GetHashCode()
        {
            return unchecked((int)_value);
        }
    }
}
