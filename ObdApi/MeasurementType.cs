namespace ObdApi
{
    public enum MeasurementType
    {
        None = -1,
        Percentage = 0, // Raw mesures, one way to resolve multiple pids
        KiloPascal,
        Rpm,
        Minutes,
        Kilometers,
        KilometersPerHour,
        DegreesBeforeTDC,
        CelsiusDegrees,
        GramsPerSecond,

        FuelTrimPercentage,
        FuelPressure,
    }
}
