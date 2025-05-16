using TestLibrus.Models;

namespace TestLibrus.Controllers.Responses;

public class UserRes_get
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public List<itemRes_get> Items { get; set; }
}