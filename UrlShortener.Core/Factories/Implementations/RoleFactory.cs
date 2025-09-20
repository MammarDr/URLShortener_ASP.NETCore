using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Factories
{
    public class RoleFactory
    {
        public enRole Create(string role) => role switch
        {
            "user" =>  enRole.User,
            "mod"  =>  enRole.Moderator,
            "admin" => enRole.Adminstrator
        };

        public string GetRoleID(enRole role) => role switch
        {
            enRole.User         => "user",
            enRole.Moderator    => "mod",
            enRole.Adminstrator => "admin"
        };
    }

}
