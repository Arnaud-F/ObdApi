namespace ObdApi
{
    using ObdApi.IO;

    public class ObdEngine
    {
        public ObdEngine(IObdService service)
        {
            if (!service.IsConnected)
            {
                service.Connect();
            }

            Current = new ObdData(service, 0x01);
            FreezeOfLastDTC = new ObdData(service, 0x02);
            StoredTroubles = new ObdDtc(service, 0x03);
            PendingTroubles = new ObdDtc(service, 0x07);
            PermanentTroubles = new ObdDtc(service, 0x0a);
        }

        public ObdData Current { get; private set; }
        public ObdData FreezeOfLastDTC { get; private set; }
        public ObdDtc StoredTroubles { get; private set; }
        public ObdDtc PendingTroubles { get; private set; }
        public ObdDtc PermanentTroubles { get; private set; }
    }
}
