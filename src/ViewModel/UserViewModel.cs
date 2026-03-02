namespace BlogApp.ViewModel;

public class UserViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsLocked { get; set; }
    public IList<string> UserRoles { get; set; }
}
