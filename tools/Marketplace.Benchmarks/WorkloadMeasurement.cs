public sealed record WorkloadMeasurement(string Name, double P50Milliseconds, double P95Milliseconds, double P99Milliseconds, double RequestsPerSecond, int ErrorCount, int TimeoutCount, double[] SamplesMilliseconds);
public sealed record CheckoutConsistencyResult(int Attempted, int Created, int ExpectedOutOfStock, int FinalStock, double DurationMilliseconds);
