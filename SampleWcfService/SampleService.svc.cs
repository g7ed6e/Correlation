namespace SampleWcfService
{
    public class SampleService : ISampleService
    {
        public string GetData()
        {
            return Correlation.CorrelationContext.Current.RequestId;
        }
    }
}
