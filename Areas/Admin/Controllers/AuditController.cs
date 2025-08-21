using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AuditController : Controller
    {
        private readonly string _conn;
        public AuditController(IConfiguration cfg)
        {
            _conn = cfg.GetConnectionString("DefaultConnection")
                 ?? cfg.GetConnectionString("4TL_SHOP")
                 ?? "";
        }

        public async Task<IActionResult> Index(string? entity = null, string? id = null, int page = 1, int pageSize = 25)
        {
            var list = new List<dynamic>();
            await using var con = new SqlConnection(_conn);
            await using var cmd = new SqlCommand(@"
                WITH x AS(
                  SELECT AuditId, EntityName, EntityId, Action, ActorEmail, OccurredAt, OldValues, NewValues,
                         ROW_NUMBER() OVER(ORDER BY OccurredAt DESC) rn
                  FROM dbo.AuditLog
                  WHERE (@e IS NULL OR EntityName = @e)
                    AND (@i IS NULL OR EntityId   = @i)
                )
                SELECT AuditId, EntityName, EntityId, Action, ActorEmail, OccurredAt, OldValues, NewValues
                FROM x WHERE rn BETWEEN (@p-1)*@ps+1 AND @p*@ps", con);
            cmd.Parameters.AddWithValue("@e", (object?)entity ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@i", (object?)id ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@p", page);
            cmd.Parameters.AddWithValue("@ps", pageSize);

            await con.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                list.Add(new
                {
                    AuditId = (int)r["AuditId"],
                    EntityName = (string)r["EntityName"],
                    EntityId = r["EntityId"]?.ToString(),
                    Action = (string)r["Action"],
                    ActorEmail = r["ActorEmail"]?.ToString(),
                    OccurredAt = ((System.DateTime)r["OccurredAt"]).ToLocalTime(),
                    OldValues = r["OldValues"]?.ToString(),
                    NewValues = r["NewValues"]?.ToString()
                });
            }
            return View(list);
        }
    }
}
