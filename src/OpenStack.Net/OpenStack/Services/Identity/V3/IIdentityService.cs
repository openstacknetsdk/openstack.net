namespace OpenStack.Services.Identity.V3
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IIdentityService : IBaseIdentityService, IExtensibleService<IIdentityService>
    {
        #region Tokens

        Task<AuthenticateApiCall> PrepareAuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken);

        Task<GetTokenApiCall> PrepareGetTokenAsync(TokenId tokenId, CancellationToken cancellationToken);

        Task<ValidateTokenApiCall> PrepareValidateTokenAsync(TokenId tokenId, CancellationToken cancellationToken);

        Task<RemoveTokenApiCall> PrepareRemoveTokenAsync(TokenId tokenId, CancellationToken cancellationToken);

        #endregion

        #region Service catalog

        Task<AddServiceApiCall> PrepareAddServiceAsync(ServiceData serviceData, CancellationToken cancellationToken);

        Task<ListServicesApiCall> PrepareListServicesAsync(CancellationToken cancellationToken);

        Task<GetServiceApiCall> PrepareGetServiceAsync(ServiceId serviceId, CancellationToken cancellationToken);

        Task<UpdateServiceApiCall> PrepareUpdateServiceAsync(ServiceId serviceId, ServiceData serviceData, CancellationToken cancellationToken);

        Task<RemoveServiceApiCall> PrepareRemoveServiceAsync(ServiceId serviceId, CancellationToken cancellationToken);

        #endregion

        #region Endpoints

        Task<AddEndpointApiCall> PrepareAddEndpointAsync(EndpointRequest request, CancellationToken cancellationToken);

        Task<ListEndpointsApiCall> PrepareListEndpointsAsync(CancellationToken cancellationToken);

        Task<GetEndpointApiCall> PrepareGetEndpointAsync(EndpointId endpointId, CancellationToken cancellationToken);

        Task<UpdateEndpointApiCall> PrepareUpdateEndpointAsync(EndpointId endpointId, EndpointRequest request, CancellationToken cancellationToken);

        Task<RemoveEndpointApiCall> PrepareRemoveEndpointAsync(EndpointId endpointId, CancellationToken cancellationToken);

        #endregion

        #region Domains

        Task<AddDomainApiCall> PrepareAddDomainAsync(DomainRequest request, CancellationToken cancellationToken);

        Task<ListDomainsApiCall> PrepareListDomainsAsync(CancellationToken cancellationToken);

        Task<GetDomainApiCall> PrepareGetDomainAsync(DomainId domainId, CancellationToken cancellationToken);

        Task<UpdateDomainApiCall> PrepareUpdateDomainAsync(DomainId domainId, DomainRequest request, CancellationToken cancellationToken);

        Task<RemoveDomainApiCall> PrepareRemoveDomainAsync(DomainId domainId, CancellationToken cancellationToken);

        Task<ListDomainUserRolesApiCall> PrepareListDomainUserRolesAsync(DomainId domainId, UserId userId, CancellationToken cancellationToken);

        Task<AddDomainUserRoleApiCall> PrepareAddDomainUserRoleAsync(DomainId domainId, UserId userId, RoleId roleId, CancellationToken cancellationToken);

        Task<ValidateDomainUserRoleApiCall> PrepareValidateDomainUserRoleAsync(DomainId domainId, UserId userId, RoleId roleId, CancellationToken cancellationToken);

        Task<RemoveDomainUserRoleApiCall> PrepareRemoveDomainUserRoleAsync(DomainId domainId, UserId userId, RoleId roleId, CancellationToken cancellationToken);

        Task<ListDomainGroupRolesApiCall> PrepareListDomainGroupRolesAsync(DomainId domainId, GroupId groupId, CancellationToken cancellationToken);

        Task<AddDomainGroupRoleApiCall> PrepareAddDomainGroupRoleAsync(DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken);

        Task<ValidateDomainGroupRoleApiCall> PrepareValidateDomainGroupRoleAsync(DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken);

        Task<RemoveDomainGroupRoleApiCall> PrepareRemoveDomainGroupRoleAsync(DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken);

        #endregion

        #region Projects

        Task<AddProjectApiCall> PrepareAddProjectAsync(ProjectRequest request, CancellationToken cancellationToken);

        Task<ListProjectsApiCall> PrepareListProjectsAsync(CancellationToken cancellationToken);

        Task<GetProjectApiCall> PrepareGetProjectAsync(ProjectId projectId, CancellationToken cancellationToken);

        Task<UpdateProjectApiCall> PrepareUpdateProjectAsync(ProjectId projectId, ProjectRequest request, CancellationToken cancellationToken);

        Task<RemoveProjectApiCall> PrepareRemoveProjectAsync(ProjectId projectId, CancellationToken cancellationToken);

        Task<ListProjectUserRolesApiCall> PrepareListProjectUserRolesAsync(ProjectId projectId, UserId userId, CancellationToken cancellationToken);

        Task<AddProjectUserRoleApiCall> PrepareAddProjectUserRoleAsync(ProjectId projectId, UserId userId, RoleId roleId, CancellationToken cancellationToken);

        Task<ValidateProjectUserRoleApiCall> PrepareValidateProjectUserRoleAsync(ProjectId projectId, UserId userId, RoleId roleId, CancellationToken cancellationToken);

        Task<RemoveProjectUserRoleApiCall> PrepareRemoveProjectUserRoleAsync(ProjectId projectId, UserId userId, RoleId roleId, CancellationToken cancellationToken);

        Task<ListProjectGroupRolesApiCall> PrepareListProjectGroupRolesAsync(ProjectId projectId, GroupId groupId, CancellationToken cancellationToken);

        Task<AddProjectGroupRoleApiCall> PrepareAddProjectGroupRoleAsync(ProjectId projectId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken);

        Task<ValidateProjectGroupRoleApiCall> PrepareValidateProjectGroupRoleAsync(ProjectId projectId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken);

        Task<RemoveProjectGroupRoleApiCall> PrepareRemoveProjectGroupRoleAsync(ProjectId projectId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken);

        #endregion

        #region Users

        Task<AddUserApiCall> PrepareAddUserAsync(UserRequest request, CancellationToken cancellationToken);

        Task<ListUsersApiCall> PrepareListUsersAsync(CancellationToken cancellationToken);

        Task<GetUserApiCall> PrepareGetUserAsync(UserId userId, CancellationToken cancellationToken);

        Task<UpdateUserApiCall> PrepareUpdateUserAsync(UserId userId, UserRequest request, CancellationToken cancellationToken);

        Task<RemoveUserApiCall> PrepareRemoveUserAsync(UserId userId, CancellationToken cancellationToken);

        Task<ListUserGroupsApiCall> PrepareListUserGroupsAsync(UserId userId, CancellationToken cancellationToken);

        Task<ListUserProjectsApiCall> PrepareListUserProjectsAsync(UserId userId, CancellationToken cancellationToken);

        Task<ListUserRolesApiCall> PrepareListUserRolesAsync(UserId userId, CancellationToken cancellationToken);

        #endregion

        #region Groups

        Task<AddGroupApiCall> PrepareAddGroupAsync(GroupRequest request, CancellationToken cancellationToken);

        Task<ListGroupsApiCall> PrepareListGroupsAsync(CancellationToken cancellationToken);

        Task<GetGroupApiCall> PrepareGetGroupAsync(GroupId groupId, CancellationToken cancellationToken);

        Task<UpdateGroupApiCall> PrepareUpdateGroupAsync(GroupId groupId, GroupRequest request, CancellationToken cancellationToken);

        Task<RemoveGroupApiCall> PrepareRemoveGroupAsync(GroupId groupId, CancellationToken cancellationToken);

        Task<ListGroupUsersApiCall> PrepareListGroupUsersAsync(GroupId groupId, CancellationToken cancellationToken);

        Task<AddGroupUserApiCall> PrepareAddUserToGroupAsync(GroupId groupId, UserId userId, CancellationToken cancellationToken);

        Task<RemoveGroupUserApiCall> PrepareRemoveUserFromGroupAsync(GroupId groupId, UserId userId, CancellationToken cancellationToken);

        Task<ValidateGroupUserApiCall> PrepareValidateUserInGroupAsync(GroupId groupId, UserId userId, CancellationToken cancellationToken);

        #endregion

        #region Credentials

        Task<AddCredentialApiCall> PrepareAddCredentialAsync(CredentialData credentialData, CancellationToken cancellationToken);

        Task<ListCredentialsApiCall> PrepareListCredentialsAsync(CancellationToken cancellationToken);

        Task<GetCredentialApiCall> PrepareGetCredentialAsync(CredentialId credentialId, CancellationToken cancellationToken);

        Task<UpdateCredentialApiCall> PrepareUpdateCredentialAsync(CredentialId credentialId, CredentialData credentialData, CancellationToken cancellationToken);

        Task<RemoveCredentialApiCall> PrepareRemoveCredentialAsync(CredentialId credentialId, CancellationToken cancellationToken);

        #endregion

        #region Roles

        Task<AddRoleApiCall> PrepareAddRoleAsync(RoleRequest request, CancellationToken cancellationToken);

        Task<ListRolesApiCall> PrepareListRolesAsync(CancellationToken cancellationToken);

        Task<GetRoleApiCall> PrepareGetRoleAsync(RoleId roleId, CancellationToken cancellationToken);

        Task<UpdateRoleApiCall> PrepareUpdateRoleAsync(RoleId roleId, RoleRequest request, CancellationToken cancellationToken);

        Task<RemoveRoleApiCall> PrepareRemoveRoleAsync(RoleId roleId, CancellationToken cancellationToken);

        Task<ListRoleAssignmentsApiCall> PrepareListRoleAssignmentsAsync(CancellationToken cancellationToken);

        #endregion

        #region Policies

        Task<AddPolicyApiCall> PrepareAddPolicyAsync(PolicyData policyData, CancellationToken cancellationToken);

        Task<ListPoliciesApiCall> PrepareListPoliciesAsync(CancellationToken cancellationToken);

        Task<GetPolicyApiCall> PrepareGetPolicyAsync(PolicyId policyId, CancellationToken cancellationToken);

        Task<UpdatePolicyApiCall> PrepareUpdatePolicyAsync(PolicyId policyId, PolicyData policyData, CancellationToken cancellationToken);

        Task<RemovePolicyApiCall> PrepareRemovePolicyAsync(PolicyId policyId, CancellationToken cancellationToken);

        #endregion
    }
}
