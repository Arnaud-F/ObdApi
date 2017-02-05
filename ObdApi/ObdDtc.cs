namespace ObdApi
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ObdApi.IO;

    public class ObdDtc
    {
        protected IObdService _service;
        protected List<string> _codes;

        internal ObdDtc(IObdService service, byte mode)
        {
            _service = service;
            _codes = new List<string>();
            var lines = service.SendRawCommand(mode.ToString("X2"));

            foreach (var item in lines)
            {
                string data = Regex.Replace(item, "\\s+", string.Empty, RegexOptions.Compiled);
                if (data.StartsWith("43"))
                {
                    data = data.Substring(2);
                    while (data.Length > 0)
                    {
                        string val = data.Substring(0, 4);
                        if (val != string.Empty && val != "0000")
                        { _codes.Add(GetCode(val)); }
                        data = data.Substring(4);
                    }
                }
            }
        }

        public IEnumerable<string> Codes { get; private set; }

        protected string GetCode(string line)
        {
            var value = Convert.ToUInt16(line, 16);
            ushort type = (ushort)(value >> 14);
            string code = (value & 0x3fff).ToString("X4");

            if (type == 0) { return string.Concat("P", code); }
            if (type == 1) { return string.Concat("C", code); }
            if (type == 2) { return string.Concat("B", code); }
            if (type == 3) { return string.Concat("U", code); }

            return string.Empty;
        }
    }
}