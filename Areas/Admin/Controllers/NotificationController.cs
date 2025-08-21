using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TL4_SHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class NotificationController : Controller
    {
        private readonly string _conn;
        public NotificationController(IConfiguration cfg)
        {
            _conn = cfg.GetConnectionString("DefaultConnection")
                 ?? cfg.GetConnectionString("4TL_SHOP")
                 ?? "";
        }

        // GET: /Admin/Notification/Latest?take=20&unreadOnly=true
        [HttpGet]
        public async Task<IActionResult> Latest(int take = 20, bool unreadOnly = false)
        {
            var list = new List<object>();
            await using var con = new SqlConnection(_conn);
            await using var cmd = new SqlCommand(@"
                SELECT TOP (@take) NotificationId, Type, Title, Body, RefEntity, RefId, Severity, CreatedAt, IsRead
                FROM dbo.AppNotification
                WHERE (@unreadOnly = 0 OR IsRead = 0)
                ORDER BY CreatedAt DESC", con);
            cmd.Parameters.AddWithValue("@take", take);
            cmd.Parameters.AddWithValue("@unreadOnly", unreadOnly ? 1 : 0);
            await con.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await r.ReadAsync())
            {
                list.Add(new
                {
                    NotificationId = (int)r["NotificationId"],
                    Type = (string)r["Type"],
                    Title = (string)r["Title"],
                    Body = r["Body"] as string,
                    RefEntity = r["RefEntity"] as string,
                    RefId = r["RefId"]?.ToString(),
                    Severity = r["Severity"] as string,
                    CreatedAt = ((System.DateTime)r["CreatedAt"]).ToLocalTime().ToString("dd/MM HH:mm"),
                    IsRead = (bool)r["IsRead"]
                });
            }
            return Json(list); // ASP.NET Core: không cần JsonRequestBehavior
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            await using var con = new SqlConnection(_conn);
            await using var cmd = new SqlCommand("UPDATE dbo.AppNotification SET IsRead = 1 WHERE NotificationId = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }
    }
}
