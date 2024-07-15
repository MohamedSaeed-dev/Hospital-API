using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalAPI.Features.Pagination
{
    public class PaginationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string pageStr = context.HttpContext.Request.Query["Page"];
            string pageSizeStr = context.HttpContext.Request.Query["PageSize"];

            pageStr = pageStr == null ? "0" : pageStr;
            pageSizeStr = pageSizeStr == null ? "0" : pageSizeStr;

            if (int.TryParse(pageStr, out int page) && int.TryParse(pageSizeStr, out int pageSize))
            {
                page = Math.Max(page, 1);
                pageSize = Math.Max(pageSize, 1);
                int startIndex = (page - 1) * pageSize;
                int endIndex = page * pageSize;
                context.HttpContext.Items["PaginationIndexes"] = new PaginationIndexes
                {
                    Skip = startIndex,
                    Take = endIndex
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
