using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TL4_SHOP.Hubs;

namespace TL4_SHOP.Filters
{
    public sealed class NotifyPingFilter : IAsyncActionFilter
    {
        private readonly IHubContext<NotificationHub> _hub;
        public NotifyPingFilter(IHubContext<NotificationHub> hub) => _hub = hub;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            if (result.Exception == null)
            {
                var method = context.HttpContext.Request.Method;
                if (method == "POST" || method == "PUT" || method == "DELETE")
                {
                    await _hub.Clients.All.SendAsync("notifyNew");
                }
            }
        }
    }
}
