namespace UrlShortener.Data.Entities.Users
{
    public class UserRoleMapping
    {
        public int                   UserId          { get; set; }
        public User                  User            { get; set; }
        public string                RoleId          { get; set; }
        public UserRoleType          RoleType        { get; set; }
    }
}
