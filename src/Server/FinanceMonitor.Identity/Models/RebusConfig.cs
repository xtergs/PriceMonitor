namespace FinanceMonitor.Identity.Models
{
    public class RebusConfig
    {
        public static string Section => "Rebus";
        public string? RabbitMQConnection { get; set; }
    }
}