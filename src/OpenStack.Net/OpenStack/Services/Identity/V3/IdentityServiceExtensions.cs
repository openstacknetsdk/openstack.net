namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using OpenStack.Net;
    using Rackspace.Net;
    using Rackspace.Threading;

    public static class IdentityServiceExtensions
    {
        #region Tokens

        public static Task<Tuple<TokenId, AuthenticateResponse>> AuthenticateAsync(this IIdentityService client, AuthenticateData authenticateData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAuthenticateAsync(new AuthenticateRequest(authenticateData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Tuple<TokenId, AuthenticateResponse>> GetTokenAsync(this IIdentityService client, TokenId tokenId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetTokenAsync(tokenId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task ValidateTokenAsync(this IIdentityService client, TokenId tokenId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareValidateTokenAsync(tokenId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task RemoveTokenAsync(this IIdentityService client, TokenId tokenId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveTokenAsync(tokenId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Service catalog

        public static Task<Service> AddServiceAsync(this IIdentityService client, ServiceData serviceData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddServiceAsync(serviceData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Service));
        }

        public static Task<ReadOnlyCollectionPage<Service>> ListServicesAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListServicesAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Service> GetServiceAsync(this IIdentityService client, ServiceId serviceId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetServiceAsync(serviceId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Service));
        }

        public static Task<Service> UpdateServiceAsync(this IIdentityService client, ServiceId serviceId, ServiceData serviceData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateServiceAsync(serviceId, serviceData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Service));
        }

        public static Task RemoveServiceAsync(this IIdentityService client, ServiceId serviceId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveServiceAsync(serviceId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Endpoints

        public static Task<Endpoint> AddEndpointAsync(this IIdentityService client, EndpointData endpointData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddEndpointAsync(new EndpointRequest(endpointData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Endpoint));
        }

        public static Task<ReadOnlyCollectionPage<Endpoint>> ListEndpointsAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListEndpointsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Endpoint> GetEndpointAsync(this IIdentityService client, EndpointId endpointId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetEndpointAsync(endpointId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Endpoint));
        }

        public static Task<Endpoint> UpdateEndpointAsync(this IIdentityService client, EndpointId endpointId, EndpointData endpointData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateEndpointAsync(endpointId, new EndpointRequest(endpointData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Endpoint));
        }

        public static Task RemoveEndpointAsync(this IIdentityService client, EndpointId endpointId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveEndpointAsync(endpointId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Domains

        public static Task<Domain> AddDomainAsync(this IIdentityService client, DomainData domainData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddDomainAsync(new DomainRequest(domainData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Domain));
        }

        public static Task<ReadOnlyCollectionPage<Domain>> ListDomainsAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListDomainsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Domain> GetDomainAsync(this IIdentityService client, DomainId domainId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetDomainAsync(domainId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Domain));
        }

        public static Task<Domain> UpdateDomainAsync(this IIdentityService client, DomainId domainId, DomainData domainData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateDomainAsync(domainId, new DomainRequest(domainData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Domain));
        }

        public static Task RemoveDomainAsync(this IIdentityService client, DomainId domainId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveDomainAsync(domainId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Role>> ListDomainUserRolesAsync(this IIdentityService client, DomainId domainId, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListDomainUserRolesAsync(domainId, userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<Role>> ListDomainGroupRolesAsync(this IIdentityService client, DomainId domainId, GroupId groupId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListDomainGroupRolesAsync(domainId, groupId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task AddDomainGroupRoleAsync(this IIdentityService client, DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddDomainGroupRoleAsync(domainId, groupId, roleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task ValidateDomainGroupRoleAsync(this IIdentityService client, DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareValidateDomainGroupRoleAsync(domainId, groupId, roleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task RemoveDomainGroupRoleAsync(this IIdentityService client, DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveDomainGroupRoleAsync(domainId, groupId, roleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Projects

        public static Task<Project> AddProjectAsync(this IIdentityService client, ProjectData projectData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddProjectAsync(new ProjectRequest(projectData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Project));
        }

        public static Task<ReadOnlyCollectionPage<Project>> ListProjectsAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListProjectsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Project> GetProjectAsync(this IIdentityService client, ProjectId projectId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetProjectAsync(projectId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Project));
        }

        public static Task<Project> UpdateProjectAsync(this IIdentityService client, ProjectId projectId, ProjectData projectData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateProjectAsync(projectId, new ProjectRequest(projectData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Project));
        }

        public static Task RemoveProjectAsync(this IIdentityService client, ProjectId projectId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveProjectAsync(projectId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Role>> ListProjectGroupRolesAsync(this IIdentityService client, ProjectId projectId, GroupId groupId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListProjectGroupRolesAsync(projectId, groupId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        #endregion

        #region Users

        public static Task<User> GetUserAsync(this IIdentityService client, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetUserAsync(userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.User));
        }

        public static Task<User> UpdateUserAsync(this IIdentityService client, UserId userId, UserData userData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateUserAsync(userId, new UserRequest(userData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.User));
        }

        public static Task RemoveUserAsync(this IIdentityService client, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveUserAsync(userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Group>> ListUserGroupsAsync(this IIdentityService client, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListUserGroupsAsync(userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<Project>> ListUserProjectsAsync(this IIdentityService client, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListUserProjectsAsync(userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<Role>> ListUserRolesAsync(this IIdentityService client, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListUserRolesAsync(userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        #endregion

        #region Groups

        public static Task<ReadOnlyCollectionPage<User>> ListUsersInGroupAsync(this IIdentityService client, GroupId groupId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListUsersInGroupAsync(groupId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task AddUserToGroupAsync(this IIdentityService client, GroupId groupId, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddUserToGroupAsync(groupId, userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task RemoveUserFromGroupAsync(this IIdentityService client, GroupId groupId, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveUserFromGroupAsync(groupId, userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task ValidateUserInGroupAsync(this IIdentityService client, GroupId groupId, UserId userId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareValidateUserInGroupAsync(groupId, userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Credentials

        public static Task<Credential> AddCredentialAsync(this IIdentityService client, CredentialData credentialData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddCredentialAsync(credentialData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<Credential>> ListCredentialsAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListCredentialsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Credential> GetCredentialAsync(this IIdentityService client, CredentialId credentialId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetCredentialAsync(credentialId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Credential> UpdateCredentialAsync(this IIdentityService client, CredentialId credentialId, CredentialData credentialData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateCredentialAsync(credentialId, credentialData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task RemoveCredentialAsync(this IIdentityService client, CredentialId credentialId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveCredentialAsync(credentialId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Roles

        public static Task<Role> AddRoleAsync(this IIdentityService client, RoleData roleData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddRoleAsync(new RoleRequest(roleData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Role> GetRoleAsync(this IIdentityService client, RoleId roleId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetRoleAsync(roleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Role));
        }

        public static Task<Role> UpdateRoleAsync(this IIdentityService client, RoleId roleId, RoleData roleData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdateRoleAsync(roleId, new RoleRequest(roleData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Role));
        }

        public static Task RemoveRoleAsync(this IIdentityService client, RoleId roleId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemoveRoleAsync(roleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Role>> ListRolesAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListRolesAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<RoleAssignment>> ListRoleAssignmentsAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListRoleAssignmentsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        #endregion

        #region Policies

        public static Task<Policy> AddPolicyAsync(this IIdentityService client, PolicyData policyData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareAddPolicyAsync(policyData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<Policy>> ListPoliciesAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareListPoliciesAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Policy> GetPolicyAsync(this IIdentityService client, PolicyId policyId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareGetPolicyAsync(policyId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Policy> UpdatePolicyAsync(this IIdentityService client, PolicyId policyId, PolicyData policyData, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareUpdatePolicyAsync(policyId, policyData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task RemovePolicyAsync(this IIdentityService client, PolicyId policyId, CancellationToken cancellationToken)
        {
            return TaskBlocks.Using(
                () => client.PrepareRemovePolicyAsync(policyId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Filters

        public static Task<ListServicesApiCall> WithType(this Task<ListServicesApiCall> task, string serviceType)
        {
            return task.WithQueryParameter("type", serviceType);
        }

        public static Task<ListServicesApiCall> WithPageSize(this Task<ListServicesApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListEndpointsApiCall> WithInterface(this Task<ListEndpointsApiCall> task, EndpointInterface endpointInterface)
        {
            return task.WithQueryParameter("interface", endpointInterface.Name);
        }

        public static Task<ListEndpointsApiCall> WithService(this Task<ListEndpointsApiCall> task, ServiceId serviceId)
        {
            return task.WithQueryParameter("service_id", serviceId.Value);
        }

        public static Task<ListEndpointsApiCall> WithPageSize(this Task<ListEndpointsApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListDomainsApiCall> WithName(this Task<ListDomainsApiCall> task, string name)
        {
            return task.WithQueryParameter("name", name);
        }

        public static Task<ListDomainsApiCall> WithEnabled(this Task<ListDomainsApiCall> task, bool enabled)
        {
            return task.WithQueryParameter("enabled", enabled.ToString());
        }

        public static Task<ListDomainsApiCall> WithPageSize(this Task<ListDomainsApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListProjectsApiCall> WithDomain(this Task<ListProjectsApiCall> task, DomainId domainId)
        {
            return task.WithQueryParameter("domain_id", domainId.Value);
        }

        public static Task<ListProjectsApiCall> WithName(this Task<ListProjectsApiCall> task, string name)
        {
            return task.WithQueryParameter("name", name);
        }

        public static Task<ListProjectsApiCall> WithEnabled(this Task<ListProjectsApiCall> task, bool enabled)
        {
            return task.WithQueryParameter("enabled", enabled.ToString());
        }

        public static Task<ListProjectsApiCall> WithPageSize(this Task<ListProjectsApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListGroupUsersApiCall> WithDomain(this Task<ListGroupUsersApiCall> task, DomainId domainId)
        {
            return task.WithQueryParameter("domain_id", domainId.Value);
        }

        public static Task<ListGroupUsersApiCall> WithDescription(this Task<ListGroupUsersApiCall> task, string description)
        {
            return task.WithQueryParameter("description", description);
        }

        public static Task<ListGroupUsersApiCall> WithEmail(this Task<ListGroupUsersApiCall> task, string email)
        {
            return task.WithQueryParameter("email", email);
        }

        public static Task<ListGroupUsersApiCall> WithName(this Task<ListGroupUsersApiCall> task, string name)
        {
            return task.WithQueryParameter("name", name);
        }

        public static Task<ListGroupUsersApiCall> WithEnabled(this Task<ListGroupUsersApiCall> task, bool enabled)
        {
            return task.WithQueryParameter("enabled", enabled.ToString());
        }

        public static Task<ListGroupUsersApiCall> WithPageSize(this Task<ListGroupUsersApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListCredentialsApiCall> WithPageSize(this Task<ListCredentialsApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListRolesApiCall> WithName(this Task<ListRolesApiCall> task, string name)
        {
            return task.WithQueryParameter("name", name);
        }

        public static Task<ListRolesApiCall> WithPageSize(this Task<ListRolesApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        public static Task<ListPoliciesApiCall> WithType(this Task<ListPoliciesApiCall> task, string serviceType)
        {
            return task.WithQueryParameter("type", serviceType);
        }

        public static Task<ListPoliciesApiCall> WithPageSize(this Task<ListPoliciesApiCall> task, int pageSize)
        {
            return task.WithQueryParameter("per_page", pageSize.ToString());
        }

        private static Task<TCall> WithQueryParameter<TCall>(this Task<TCall> task, string parameter, string value)
            where TCall : IHttpApiRequest
        {
            return task.Select(
                innerTask =>
                {
                    Uri requestUri = innerTask.Result.RequestMessage.RequestUri;
                    requestUri = UriUtility.RemoveQueryParameter(requestUri, parameter);
                    string originalQuery = requestUri.Query;

                    UriTemplate queryTemplate;
                    if (string.IsNullOrEmpty(originalQuery))
                    {
                        // URI does not already contain query parameters
                        queryTemplate = new UriTemplate("{?" + parameter + "}");
                    }
                    else
                    {
                        // URI already contains query parameters
                        queryTemplate = new UriTemplate("{&" + parameter + "}");
                    }

                    var parameters = new Dictionary<string, string> { { parameter, value } };
                    Uri queryUri = queryTemplate.BindByName(parameters);
                    innerTask.Result.RequestMessage.RequestUri = new Uri(requestUri.OriginalString + queryUri.OriginalString, UriKind.Absolute);
                    return innerTask.Result;
                });
        }

        #endregion
    }
}
