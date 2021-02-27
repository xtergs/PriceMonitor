#pragma warning disable 8618

namespace FinanceMonitor.Messages
{
    public sealed record UserCreated
    {
        public string UserId { get; init; }
        public string Email { get; init; }
    }
}