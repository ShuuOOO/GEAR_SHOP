using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TL4_SHOP.Filters
{
    public sealed class SetActorContextFilter : IAsyncActionFilter
    {
        private readonly string _conn;
        public SetActorContextFilter(IConfiguration cfg)
        {
            _conn = cfg.GetConnectionString("DefaultConnection")
                    ?? cfg.GetConnectionString("4TL_SHOP")
                    ?? "";
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(_conn))
            {
                await using var con = new SqlConnection(_conn);
                await using var cmd = new SqlCommand("EXEC dbo.SetActorContext @UserId, @Email", con);
                cmd.Parameters.AddWithValue("@UserId", user.Identity!.Name ?? (object)System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", user.Identity!.Name ?? (object)System.DBNull.Value);
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            await next(); // tiếp tục pipeline
        }
    }
}
