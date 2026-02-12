using Microsoft.EntityFrameworkCore;

namespace events.infra.Model;

[PrimaryKey(nameof(Id))]
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Event>? Events { get; set; } = null!;
}
