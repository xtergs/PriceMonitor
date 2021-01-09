using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FinanceMonitor.Api.Extensions
{
    public static class ControllerExtensions
    {
        public static string UserId(this ControllerBase controller)
        {
            var userId = controller.User.FindFirstValue("sub");
            return userId;
        }
    }
}