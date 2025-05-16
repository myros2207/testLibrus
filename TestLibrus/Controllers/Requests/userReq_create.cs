namespace TestLibrus.Controllers.Requests;

public class UserReq_create
{
    public string name { get; set; }
    public DateTime birthDate { get; set; }
    public DateTime? deathDate { get; set; }
}