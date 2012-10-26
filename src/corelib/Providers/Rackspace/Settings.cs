namespace net.openstack.Providers.Rackspace
{
    public class Settings
    {
        public const string USIdentityUrlBase = "https://identity.api.rackspacecloud.com";
        public const string LONIdentityUrlBase = "https://lon.identity.api.rackspacecloud.com";
        public const string GetAllRolesUrlFormat = "v2.0/OS-KSADM/roles";
        public const string GetRolesByUserUrlFormat = "v2.0/users/{userID}/roles";
        public const string GetUserByNameUrlFormat = "v2.0/users/?name={name}";
        public const string AddRoleToUserUrlFormat = "v2.0/users/{userID}/roles/OS-KSADM/{roleID}";
        public const string GetUserImpersonationTokenUrlFormat = "v2.0/RAX-AUTH/impersonation-tokens";
        public const string DeleteRoleFromUserUrlFormat = "v2.0/users/{userID}/roles/OS-KSADM/{roleID}";
        public const string GetTokenInfoUrlFormat = "v2.0/tokens";

        public static string DFWComputeUrlBase = "https://dfw.servers.api.rackspacecloud.com";
        public static string ORDComputeUrlBase = "https://ord.servers.api.rackspacecloud.com";
        public static string LONComputeUrlBase = "https://lon.servers.api.rackspacecloud.com";
        public static string GetMetadataUrlFormat = "v2/{cloudDDIAccout}/servers/{apiServerID}/metadata";
        public static string GetDetailsUrlFormat = "v2/{cloudDDIAccout}/servers/{apiServerID}";
        public static string CreateServerUrlFormat = "v2/{cloudDDIAccout}/servers";
        public static string DeleteServerUrlFormat = "v2/{cloudDDIAccout}/servers/{cloudServerID}";
    }
}
