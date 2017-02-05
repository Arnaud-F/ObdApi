namespace ObdApi.IO
{
    using System.IO.Ports;

    public class SerialPortObdService : ObdService
    {
        private SerialPort _serialPort;

        public override bool Connect()
        {
            var ports = SerialPort.GetPortNames();
            _serialPort = new SerialPort("COM4");

            _serialPort.BaudRate = 9600;
            _serialPort.DataBits = 8;
            _serialPort.NewLine = '\r'.ToString();
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;

            try
            {
                _serialPort.Open();
                _stream = _serialPort.BaseStream;
                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
            }

            return IsConnected;
        }

        public override void Disconnect()
        {
            if (IsConnected)
            { _serialPort.Close(); }

            IsConnected = false;
        }

        public override void Dispose()
        {
            Disconnect();

            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }
    }
}