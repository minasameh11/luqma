using Luqma.Application.Wapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Luqma.API.Filters
{
    public class ApiResponseFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult okObjectResult && okObjectResult.Value is IResponse response)
            {
                if (!response.IsSucceeded)
                {
                    context.Result = new BadRequestObjectResult(response);
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
