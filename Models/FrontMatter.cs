using YamlDotNet.Serialization;

public class FrontMatter
{
    [YamlMember(Alias = "title")]
    public string Title { get; set; }
    [YamlMember(Alias = "date")]
    public DateTime PublishDate { get; set; }
    [YamlMember(Alias = "category")]
    public string Category { get; set; }
}
