using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace FunctionHandler.Infrastructure.Http
{
    public static class Responses
    {
        public static string Ok()
        {
            var response = new
            {
                status = StatusCodes.Status200OK
            };

            return JsonConvert.SerializeObject(response);
        }

        public static string InternalServerError(string message)
        {
            var response = new ProblemDetails
            {
                Detail = message,
                Status = StatusCodes.Status500InternalServerError,
                Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError)
            };

            return JsonConvert.SerializeObject(response);
        }

        public static string InternalServerError(Exception ex)
        {
            var response = new ProblemDetails
            {
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
                Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError)
            };

            response.Extensions.Add("Stack Trace", ex.Demystify());

            return JsonConvert.SerializeObject(response);
        }
    }
}
