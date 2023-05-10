namespace GameShopAPI.Services.IModelImageService;

public class SaveImageModel
{
    public string Name { get; set; } = string.Empty;
    public string Prefix { get; set; } = "general";
    public IFormFile? File { get; set; }

    public SaveImageModel(string name, string prefix, IFormFile? file)
    {
        Name = name;
        Prefix = prefix;
        File = file;
    }
}
