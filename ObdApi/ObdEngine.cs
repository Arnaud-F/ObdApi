using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObdApi.IO;

namespace ObdApi
{
    public class ObdEngine
    {
        public ObdEngine(IObdService service)
        {
            Current = new ObdPid(service, 1);
            Default = new ObdPid(service, 2);
        }

        public ObdPid Current { get; private set; }
        public ObdPid Default { get; private set; }
    }
}
