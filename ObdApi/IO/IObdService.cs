namespace ObdApi.IO
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IObdService : IDisposable
    {
        ObdProtocolType ObdProtocolType { get; }

        bool Connect();

        bool IsConnected { get; }

        IEnumerable<string> SendRawCommand(string command);

        Byte? QueryBytePid(byte mode, byte pid);
        UInt16? QueryUInt16Pid(byte mode, byte pid);
        UInt32? QueryUInt32Pid(byte mode, byte pid);
        Measurement<T> ReadMeasurement<T>(byte mode, byte pid) where T : IFormattable, IEquatable<T>;

        void Disconnect();
    }
}
