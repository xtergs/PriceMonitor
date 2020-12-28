namespace FinanceMonitor.Api.Models
{
    public class RebusConfig
    {
        public static string Section => "Rebus";
        public string RabbitMQConnection { get; set; }
    }
}