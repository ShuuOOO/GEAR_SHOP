using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;  // <-- thêm
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TL4_SHOP.Extensions
{
    // Bắt lỗi THROW 50001/50002/50003 từ trigger và hiển thị ra UI
    public class SqlErrorToMessageFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled) return;

            var ex = context.Exception;
            var sqlEx =
                   ex as SqlException
                ?? ex.InnerException as SqlException
                ?? (ex as DbUpdateException)?.InnerException as SqlException;

            if (sqlEx != null && (sqlEx.Number == 50001 || sqlEx.Number == 50002 || sqlEx.Number == 50003))
            {
                var msg = sqlEx.Message;

                // Lấy TempData từ DI rồi set message
                var factory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                var temp = factory.GetTempData(context.HttpContext);
                temp["Error"] = msg;

                // Quay về trang trước (hoặc 400 nếu không có Referer)
                var referer = context.HttpContext.Request.Headers["Referer"].ToString();
                if (!string.IsNullOrWhiteSpace(referer))
                    context.Result = new RedirectResult(referer);
                else
                    context.Result = new ContentResult { StatusCode = 400, Content = msg };

                context.ExceptionHandled = true;
            }
        }
    }
}
