namespace ObdApi
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using ObdApi.IO;
    using OBDApi;

    public class ObdPid : INotifyPropertyChanged
    {
        protected delegate object ObdPidDelegate(ObdPid my, byte pid);

        protected static readonly Dictionary<byte, ObdPidDelegate> Pids = new Dictionary<byte, ObdPidDelegate>
        {
            { 0x01, (t, p) => t.MonitorStatusDTC = MonitorStatus.Create(t.ReadUInt32(p)) },
            { 0x04, (t, p) => t.CalculatedEngineLoad = t.ReadMeasurement<Int32>(p) },
            { 0x05, (t, p) => t.EngineCoolantTemperature = t.ReadMeasurement<Int32>(p) },
            { 0x06, (t, p) => t.ShortTermFuelTrimBank1 = t.ReadMeasurement<Double>(p) },
            { 0x07, (t, p) => t.LongTermFuelTrimBank1 = t.ReadMeasurement<Double>(p) },
            { 0x08, (t, p) => t.ShortTermFuelTrimBank2 = t.ReadMeasurement<Double>(p) },
            { 0x09, (t, p) => t.LongTermFuelTrimBank2 = t.ReadMeasurement<Double>(p) },
            { 0x0a, (t, p) => t.FuelPressure = t.ReadMeasurement<Double>(p) },
            { 0x0b, (t, p) => t.IntakeManifoldAbsolutePressure = t.ReadMeasurement<Int32>(p) },
            { 0x0c, (t, p) => t.EngineRpm = t.ReadMeasurement<Int32>(p) },
            { 0x0d, (t, p) => t.VehicleSpeed = t.ReadMeasurement<Int32>(p) },
            { 0x0e, (t, p) => t.TimingAdvance = t.ReadMeasurement<Double>(p) },
            { 0x0f, (t, p) => t.IntakeAirTemperature = t.ReadMeasurement<Int32>(p) },
            { 0x10, (t, p) => t.MafAirFlowRate = t.ReadMeasurement<Double>(p) },
            { 0x11, (t, p) => t.ThrottlePosition = t.ReadMeasurement<Int32>(p) },
            { 0x1c, (t, p) => t.StandardType = (ObdStandardType)(t.ReadUInt32(p) ?? 0) }
        };

        protected IObdService _service;
        protected Dictionary<byte, IEnumerable<bool>> _supportedPids = new Dictionary<byte, IEnumerable<bool>>();

        public event PropertyChangedEventHandler PropertyChanged;

        /*internal protected*/
        public ObdPid(IObdService service, byte mode)
        {
            _service = service;
            Mode = mode;

            // Check supported PIDs
            for (byte i = 0x00; i <= 0xa0; i += 0x20)
            {
                uint val = service.QueryUInt32Pid(Mode, i) ?? 0;
                var bits = Convert.ToString(val, 2).PadLeft(32, '0').Select(b => b == '1');
                _supportedPids.Add(i, bits);
            }

            // First initialization
            Update();
        }

        public byte Mode { get; private set; } = 1;

        public void Update()
        {
            foreach (var item in Pids)
            {
                item.Value(this, item.Key);
            }
        }

        protected MonitorStatus _monitorStatus;
        /// <summary>0x01</summary>
        public MonitorStatus MonitorStatusDTC
        {
            get { return _monitorStatus; }
            set { CompareAndRaiseChange(ref _monitorStatus, value, nameof(MonitorStatusDTC)); }
        }


        protected Measurement<Int32> _calculatedEngineLoad;
        ///<summary>0x04</summary>
        public Measurement<Int32> CalculatedEngineLoad
        {
            get { return _calculatedEngineLoad; }
            set { CompareAndRaiseChange(ref _calculatedEngineLoad, value, nameof(CalculatedEngineLoad)); }
        }

        protected Measurement<Int32> _engineCoolantTemperature;
        /// <summary>0x05</summary>
        public Measurement<Int32> EngineCoolantTemperature
        {
            get { return _engineCoolantTemperature; }
            set { CompareAndRaiseChange(ref _engineCoolantTemperature, value, nameof(EngineCoolantTemperature)); }
        }

        protected Measurement<Double> _shortTermFuelTrimBank1;
        /// <summary>0x06</summary>
        public Measurement<Double> ShortTermFuelTrimBank1
        {
            get { return _shortTermFuelTrimBank1; }
            set { CompareAndRaiseChange(ref _shortTermFuelTrimBank1, value, nameof(ShortTermFuelTrimBank1)); }
        }

        protected Measurement<Double> _longTermFuelTrimBank1;
        ///<summary>0x07</summary>
        public Measurement<Double> LongTermFuelTrimBank1
        {
            get { return _longTermFuelTrimBank1; }
            set { CompareAndRaiseChange(ref _longTermFuelTrimBank1, value, nameof(LongTermFuelTrimBank1)); }
        }

        protected Measurement<Double> _shortTermFuelTrimBank2;
        ///<summary>0x08</summary>
        public Measurement<Double> ShortTermFuelTrimBank2
        {
            get { return _shortTermFuelTrimBank2; }
            set { CompareAndRaiseChange(ref _shortTermFuelTrimBank2, value, nameof(ShortTermFuelTrimBank2)); }
        }

        protected Measurement<Double> _longTermFuelTrimBank2;
        ///<summary>0x09</summary>
        public Measurement<Double> LongTermFuelTrimBank2
        {
            get { return _longTermFuelTrimBank2; }
            set { CompareAndRaiseChange(ref _longTermFuelTrimBank2, value, nameof(LongTermFuelTrimBank2)); }
        }

        protected Measurement<Double> _fuelPressure;
        ///<summary>0x0a/summary>
        public Measurement<Double> FuelPressure
        {
            get { return _fuelPressure; }
            set { CompareAndRaiseChange(ref _fuelPressure, value, nameof(FuelPressure)); }
        }

        protected Measurement<Int32> _intakeManifoldAbsolutePressure;
        ///<summary>0x0b</summary>
        public Measurement<Int32> IntakeManifoldAbsolutePressure
        {
            get { return _intakeManifoldAbsolutePressure; }
            set { CompareAndRaiseChange(ref _intakeManifoldAbsolutePressure, value, nameof(IntakeManifoldAbsolutePressure)); }
        }

        private Measurement<Int32> _engineRpm = null;
        /// <summary>0x0c</summary>
        public Measurement<Int32> EngineRpm
        {
            get { return _engineRpm; }
            private set { CompareAndRaiseChange(ref _engineRpm, value, nameof(EngineRpm)); }
        }

        protected Measurement<Int32> _vehicleSpeed;
        /// <summary>0x0d</summary>
        public Measurement<Int32> VehicleSpeed
        {
            get { return _vehicleSpeed; }
            set { CompareAndRaiseChange(ref _vehicleSpeed, value, nameof(VehicleSpeed)); }
        }

        protected Measurement<Double> _timingAdvance;
        ///<summary>0x0e</summary>
        public Measurement<Double> TimingAdvance
        {
            get { return _timingAdvance; }
            set { CompareAndRaiseChange(ref _timingAdvance, value, nameof(TimingAdvance)); }
        }

        protected Measurement<Int32> _intakeAirTemperature;
        ///<summary>0x0f</summary>
        public Measurement<Int32> IntakeAirTemperature
        {
            get { return _intakeAirTemperature; }
            set { CompareAndRaiseChange(ref _intakeAirTemperature, value, nameof(IntakeAirTemperature)); }
        }

        protected Measurement<Double> _mafAirFlowRate;
        ///<summary>0x10</summary>
        public Measurement<Double> MafAirFlowRate
        {
            get { return _mafAirFlowRate; }
            set { CompareAndRaiseChange(ref _mafAirFlowRate, value, nameof(MafAirFlowRate)); }
        }

        protected Measurement<Int32> _throttlePosition;
        ///<summary>0x11</summary>
        public Measurement<Int32> ThrottlePosition
        {
            get { return _throttlePosition; }
            set { CompareAndRaiseChange(ref _throttlePosition, value, nameof(ThrottlePosition)); }
        }


        protected ObdStandardType _standardType;
        ///<summary>0x1c</summary>
        public ObdStandardType StandardType
        {
            get { return _standardType; }
            set { CompareAndRaiseChange(ref _standardType, value, nameof(StandardType)); }
        }
        
        #region Read
        protected Measurement<T> ReadMeasurement<T>(byte pid) where T : IFormattable, IEquatable<T>
        {
            return _service.ReadMeasurement<T>(Mode, pid);
        }

        protected UInt32? ReadUInt32(byte pid)
        {
            if (IsSupported(pid))
            {
                return _service.QueryUInt32Pid(Mode, pid);
            }

            return null;
        }
        #endregion

        protected bool IsSupported(byte pid)
        {
            var key = (pid / 0x20) * 0x20;
            return _supportedPids[(byte)key].ElementAt(--pid % 0x20);
        }

        protected void CompareAndRaiseChange<T>(ref T old, T @new, string propertyName)
        {
            if (ReferenceEquals(old, @new) == false
                || old.Equals(@new) == false)
            {
                old = @new;

                var handle = PropertyChanged;
                if (handle != null)
                {
                    handle(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
