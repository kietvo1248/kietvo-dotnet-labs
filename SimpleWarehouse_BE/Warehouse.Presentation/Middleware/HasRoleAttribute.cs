using System;


namespace Warehouse.Application.Middleware
{
    public class HasRoleAttribute : Attribute
    {
        public string[] AllowedRoles { get; }
        public HasRoleAttribute(params string[] roles)
        {
            AllowedRoles = roles;
        }
    }
}
