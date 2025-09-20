using UrlShortener.Core.Factories.Interfaces;
using UrlShortener.Data.Entities.Permissions;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Factories
{
    public class PermissionFactory : IPermissionFactory
    {
        public enPermission Create(string permission) => permission switch
        {
            "Create" => enPermission.Create,
            "Read"   => enPermission.Read    ,
            "Update" => enPermission.Update,
            "Delete" => enPermission.Delete,
            _        => throw new InvalidCastException("'GetPermissionFromString' Invalid Casting of Permission")
        };


        public enPermission GetByID(int permissionID) => permissionID switch
        {
            1 => enPermission.Create,
            2 => enPermission.Read,
            3 => enPermission.Update,
            4 => enPermission.Delete,
            _ => throw new InvalidCastException("'GetPermissionFromString' Invalid Casting of Permission")
        };
    }

}
