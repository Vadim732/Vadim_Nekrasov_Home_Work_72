namespace Delivery.ViewModels;

public class UserRolesViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public IList<string> Roles { get; set; }
}