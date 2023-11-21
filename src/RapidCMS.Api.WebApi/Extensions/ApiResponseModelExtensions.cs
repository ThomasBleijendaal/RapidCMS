using Microsoft.AspNetCore.Mvc;
using RapidCMS.Api.Core.Models;

namespace RapidCMS.Api.WebApi.Extensions;

public static class ApiResponseModelExtensions
{
    public static ContentResult ToContentResult(this ApiResponseModel model)
    {
        if (model.ResponseBody == null)
        {
            return new ContentResult
            {
                StatusCode = (int)model.StatusCode
            };
        }

        return new ContentResult
        {
            Content = model.ResponseBody,
            ContentType = "application/json",
            StatusCode = (int)model.StatusCode
        };
    }
}
