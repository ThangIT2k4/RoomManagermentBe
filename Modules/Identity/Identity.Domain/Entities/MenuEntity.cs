using Identity.Domain.Common;
using Identity.Domain.Exceptions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.Entities;

public sealed class MenuEntity : AggregateRoot<Guid>
{
    public MenuCode Code { get; private set; } 
    public MenuLabel Label { get; private set; }
    public MenuPath? Path { get; private set; }
    public string? Icon { get; private set; }
    public int OrderIndex { get; private set; }
    public Guid? ParentId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private MenuEntity() {}

    private MenuEntity( Guid id, MenuCode code, MenuLabel label, int orderIndex, MenuPath? path, string? icon, Guid? parentId, bool isActive)
    {
        Id = id;
        Code = code;
        Label = label;
        OrderIndex = orderIndex;
        Path = path;
        Icon = icon;
        ParentId = parentId;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }

    public static MenuEntity Create( Guid id, MenuCode code, MenuLabel label, int orderIndex, MenuPath? path = null, string? icon = null, Guid? parentId = null, bool isActive = true)
    {
        return new MenuEntity(id, code, label, orderIndex, path, icon, parentId, isActive);
    }

    public void Update(MenuLabel label, MenuPath? path, string? icon, int orderIndex, DateTime updatedAt)
    {
        Label = label;
        Path = path;
        Icon = icon;
        OrderIndex = orderIndex;
    }

    public void SetActive(bool isActive, DateTime updatedAt)
    {
        IsActive = isActive;
        UpdatedAt = updatedAt;
    }
}
