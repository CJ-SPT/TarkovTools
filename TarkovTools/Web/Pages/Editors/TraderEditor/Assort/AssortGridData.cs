namespace TarkovTools.Web.Pages.Editors.TraderEditor.Assort;

public record AssortGridDataModel
{
    public required string Id { get; set; }
    public required string TemplateId { get; set; }
    public required string LocalizedName { get; set; }
    public required string ParentId { get; set; }
    public required string SlotId { get; set; }
        
        
    public required bool IsChild { get; set; }
    public List<AssortGridDataModel> ChildItemData { get; set; } = [];
}