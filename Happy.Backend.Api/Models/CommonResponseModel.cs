namespace Happy.Backend.Api.Models;

public class CommonResponseModel<T>
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public CommonResponseModel(int status, T? data = default, string? message = null)
    {
        Status = status;
        Data = data;
        Message = message;
    }
}
