using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Ergasia_WebApp.Data;

public class ServiceResult<T>
{
    private ServiceResult() { }
    public T? Data { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    
    [MemberNotNullWhen(true, nameof(Data))]
    public bool IsSuccess { get; init; }
    
    public static class Build
    {
        public static ServiceResult<T> Success(T data, HttpStatusCode statusCode)
        {
            return new ServiceResult<T>
            {
                Data = data,
                StatusCode = statusCode,
                IsSuccess = true,
            };
        }
    
        public static ServiceResult<T> Failure(HttpStatusCode statusCode)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,
                Data = default,
                IsSuccess = false,
            };
        }
    }
}