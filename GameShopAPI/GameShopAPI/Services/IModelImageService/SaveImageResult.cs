namespace GameShopAPI.Services.IModelImageService;

public class SaveImageResult
{
    public string ErrorMessage { get; set; } = string.Empty;
    public bool Success { get; set; } = false;
    public string MainPath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
}
