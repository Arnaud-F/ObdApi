using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ObdApi.IO
{
    public abstract class ObdService : IObdService
    {
        protected static readonly char[] LineTerminators = new[] { '\r', '\n' };
        protected static readonly Regex IsHexadecimal = new Regex("^(?:[0-9A-Fa-f]{2})+$", RegexOptions.Compiled);
        protected static readonly object Lock = new object();

        protected Stream _stream;

        public ObdProtocolType ObdProtocolType { get; protected set; }
        public bool IsConnected { get; protected set; }


        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract void Dispose();

        public virtual IEnumerable<string> SendRawCommand(string command)
        {
            if (command == null)
            { throw new ArgumentNullException(nameof(command)); }

            StringBuilder sb = new StringBuilder();

            lock (Lock)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(command + '\r');
                _stream.Write(buffer, 0, buffer.Length);

                Thread.Sleep(50);

                char c;
                do
                {
                    c = (char)_stream.ReadByte();
                    sb.Append(c);
                } while (c != '>');
            }

            return sb.ToString().Split(LineTerminators, StringSplitOptions.RemoveEmptyEntries);
        }

        protected string InternalQueryString(byte mode, byte pid)
        {
            string command = string.Format("{0:X2}{1:X2}", mode, pid);
            IEnumerable<string> lines = SendRawCommand(command);

            var data = lines.First().Trim();
            var shortData = Regex.Replace(data, "\\s+", string.Empty, RegexOptions.Compiled);

            if (IsHexadecimal.IsMatch(shortData))
            {
                var ret = mode | 0x40;
                if (data.StartsWith(ret.ToString("X2")))
                {
                    shortData = shortData.Substring(4);
                }
                return shortData;
            }

            return data;
        }

        public Byte? QueryBytePid(byte mode, byte pid)
        {
            UInt32? value = QueryBytePid(mode, pid);
            return (Byte?)(value & 0x000000ff);
        }

        public UInt16? QueryUInt16Pid(byte mode, byte pid)
        {
            UInt32? value = QueryUInt32Pid(mode, pid);
            return (UInt16?)(value & 0x0000ffff);
        }

        public UInt32? QueryUInt32Pid(byte mode, byte pid)
        {
            var value = InternalQueryString(mode, pid);

            if (IsHexadecimal.IsMatch(value))
            {
                return Convert.ToUInt32(value, 16);
            }

            return null;
        }

        public Measurement<T> ReadMeasurement<T>(byte mode, byte pid) where T : IFormattable, IEquatable<T>
        {
            return new Measurement<T>(pid, InternalQueryString(mode, pid));
        }
    }
}
