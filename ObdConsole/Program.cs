using ObdApi.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => { Console.Error.WriteLine(o); Console.WriteLine(e.ExceptionObject); };
            try
            {
                IObdService service = new SerialPortObdService();
                service.Connect();
                Console.WriteLine("ATZ");
                Console.Write(string.Join(Environment.NewLine, service.SendRawCommand("ATZ")));
                Console.WriteLine("ATE0");                                                   
                Console.Write(string.Join(Environment.NewLine, service.SendRawCommand("ATE0")));
                Console.WriteLine("ATL0");                                                   
                Console.Write(string.Join(Environment.NewLine, service.SendRawCommand("ATL0")));

                //for (int i = 0; i < 0x20; i++)
                //{
                //    string command = string.Format("01{0:X2}", i);
                //    Console.WriteLine(command);
                //    Console.Write(string.Join(Environment.NewLine, service.SendRawCommandAsync(command).Result));
                //}
                //Console.ReadLine();

                var s = new ObdApi.ObdEngine(service);
                Console.WriteLine();
                Console.WriteLine(s.Current.EngineRpm);
                Console.WriteLine(s.Current.ThrottlePosition);
                Console.WriteLine(s.Current.EngineCoolantTemperature);
                Console.WriteLine(s.Current.MafAirFlowRate);
                Console.WriteLine(s.Current.VehicleSpeed);
                Console.Write(">");
                var d = s.StoredTroubles.Codes;

                string str = null;
                while ((str = Console.ReadLine()) != "END")
                {
                    Console.Write(string.Join(Environment.NewLine, service.SendRawCommand(str)));
                }

                Console.WriteLine("---");
                Console.ReadLine();
                service.Disconnect();
                service.Dispose();
            }
            catch (ObjectDisposedException ex)
            { Console.Error.WriteLine(ex.ToString()); }
        }
    }
}
