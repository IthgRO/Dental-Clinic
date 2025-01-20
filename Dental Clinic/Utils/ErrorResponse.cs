using Dental_Clinic.Responses.Error;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic.Utils
{
    public class ErrorResponse
    {
        public static ObjectResult GetErrorResponse(Exception ex)
        {
            return new ObjectResult(new ErrorViewModel
            {
                Message = ex.Message,
            })
            { StatusCode = StatusCodes.Status403Forbidden };
        }
    }
}
