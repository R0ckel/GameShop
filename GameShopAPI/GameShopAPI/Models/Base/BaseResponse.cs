namespace GameShopAPI.Models.Base;

public class BaseResponse<T>
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
    public int ValueCount { get; set; } = 0;
    public List<T> Values { get; set; } = new List<T>();
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public int PageCount { get; set; } = 0;
}
