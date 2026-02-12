using System.ComponentModel.DataAnnotations.Schema;
using events.infra.Model;
using Microsoft.EntityFrameworkCore;

namespace events.infra;

[PrimaryKey(nameof(Id))]
public class Event
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime EventStart { get; set; }
    public DateTime EventEnd { get; set; }
    public Category? Category { get; set; } = null!;
    public ICollection<string>? Covers { get; set; } = [];
    public ICollection<Feature>? Features { get; set; } = [];

}
