namespace ObdApi
{
    using System;
    using System.Collections.Generic;
    using System.Resources;

    public class Measurement<T> : IFormattable
        where T : IFormattable, IEquatable<T>
    {
        #region Supported PID
        protected static readonly Dictionary<byte, MeasurementType> Pids = new Dictionary<byte, MeasurementType>
        {
            { 0x02, MeasurementType.None },
            //{ 0x03, MeasurementType.None },
            { 0x04, MeasurementType.Percentage },
            { 0x05, MeasurementType.CelsiusDegrees },
            { 0x06, MeasurementType.FuelTrimPercentage },
            { 0x07, MeasurementType.FuelTrimPercentage },
            { 0x08, MeasurementType.FuelTrimPercentage },
            { 0x09, MeasurementType.FuelTrimPercentage },
            { 0x0a, MeasurementType.FuelPressure },
            { 0x0b, MeasurementType.KiloPascal },
            { 0x0c, MeasurementType.Rpm },
            { 0x0d, MeasurementType.KilometersPerHour },
            { 0x0e, MeasurementType.DegreesBeforeTDC },
            { 0x0f, MeasurementType.CelsiusDegrees },
            { 0x10, MeasurementType.GramsPerSecond },
            { 0x11, MeasurementType.Percentage }
        };
        #endregion
        #region Formulas
        protected static readonly Func<Byte, Byte> ToCelsiusDegreesFunc = a => (byte)(a - 40);
        protected static readonly Func<Byte, Double> FuelTrimFunc = a => Math.Round(a / 1.28d - 100, 1);
        protected static readonly Func<Byte, Byte, UInt16> ToRpmFunc = (a, b) => (ushort)Math.Round(((256 * a) + b) / 4d);
        protected static readonly Func<Byte, Byte, Double> ToGramsSecFunc = (a, b) => Math.Round(((256 * a) + b) / 100d, 2);
        protected static readonly Func<Byte, Byte> ToPercentageFunc = (a) => (byte)Math.Round(a / 2.55d);
        protected static readonly Func<Byte, Byte> ToThreeKpaFunc = (a) => (byte)(3 * a);
        protected static readonly Func<Byte, Double> ToDegreesBeforeTDC = (a) => Math.Round(a / 2.0 - 40, 1);
        #endregion

        protected string _data;
        protected uint _value;

        internal protected Measurement(byte pid)
        {
            if (!Pids.ContainsKey(pid))
            { throw new ArgumentException(string.Format("PID {0} not supported", pid)); }

            IsSupported = false;
            _data = "?";
            ResourceManager rm = new ResourceManager(typeof(UnitResource));
            Unit = rm.GetString(Pids[pid].ToString());
        }

        internal protected Measurement(byte pid, string data) : this(pid)
        {
            _data = (data ?? string.Empty).Trim();
            Pid = pid;
            IsSupported = _data != "?";
            HasData = !(_data.Contains("NO DATA") || _data.Contains("SEARCHING"));

            if (IsSupported && HasData)
            {
                try
                {
                    _value = Convert.ToUInt32(data, 16);
                }
                catch
                { }

                try
                {
                    ApplyFormula();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        protected void ApplyFormula()
        {
            byte a = (byte)(_value >> 24 & 0x000000ff);
            byte b = (byte)(_value >> 16 & 0x000000ff);
            byte c = (byte)(_value >> 08 & 0x000000ff);
            byte d = (byte)(_value >> 00 & 0x000000ff);
            dynamic value = default(T);

            switch (Pids[Pid])
            {
                case MeasurementType.Percentage:
                    value = ToPercentageFunc(d);
                    break;
                case MeasurementType.Rpm:
                    value = ToRpmFunc(c, d);
                    break;
                case MeasurementType.Minutes:
                    break;
                case MeasurementType.Kilometers:
                    break;
                case MeasurementType.KilometersPerHour:
                    value = d;
                    break;
                case MeasurementType.DegreesBeforeTDC:
                    value = ToDegreesBeforeTDC(d);
                    break;
                case MeasurementType.CelsiusDegrees:
                    value = ToCelsiusDegreesFunc(d);
                    break;
                case MeasurementType.GramsPerSecond:
                    value = ToGramsSecFunc(c, d);
                    break;
                case MeasurementType.FuelTrimPercentage:
                    //value = To
                    break;
                case MeasurementType.FuelPressure:
                    value = ToThreeKpaFunc(d);
                    break;
                default:
                    value = _value;
                    break;
            }

            Value = value;
        }

        public bool IsSupported { get; internal protected set; }
        public bool HasData { get; internal protected set; }
        public byte Pid { get; internal protected set; }
        public T Value { get; protected set; }
        public string Unit { get; internal protected set; }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (!HasData || !IsSupported)
            { return _data; }

            if (Value != null)
                return $"{Value} {Unit}";
            else
                return $"? {Unit}";
        }
    }
}
