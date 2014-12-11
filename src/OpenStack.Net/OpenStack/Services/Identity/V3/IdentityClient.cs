namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.Security.Authentication;
    using Rackspace.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides the default implementation of the OpenStack Identity Service V3, with the core operations
    /// defined in the <see cref="IIdentityService"/> interface.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class IdentityClient : BaseIdentityClient, IIdentityService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityClient"/> class with the specified fixed base address.
        /// </summary>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public IdentityClient(Uri baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityClient"/> class with the specified fixed base address.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="baseAddress"/> is not an absolute URI.
        /// </exception>
        public IdentityClient(IAuthenticationService authenticationService, Uri baseAddress)
            : base(CreateAuthenticationService(authenticationService, baseAddress), baseAddress)
        {
        }

        /// <summary>
        /// Create an authentication service instance which distinguishes between the authenticated and unauthenticated
        /// HTTP API calls in the Identity Service.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="baseAddress">The base address of the Identity Service.</param>
        /// <returns>The authentication service to use for this client.</returns>
        private static IdentityClientAuthenticationService CreateAuthenticationService(IAuthenticationService authenticationService, Uri baseAddress)
        {
            IdentityClientAuthenticationService result = authenticationService as IdentityClientAuthenticationService;
            if (result != null)
                return result;

            return new IdentityClientAuthenticationService(baseAddress, authenticationService, new PassThroughAuthenticationService(baseAddress));
        }

        #region Tokens

        /// <inheritdoc/>
        public virtual Task<AuthenticateApiCall> PrepareAuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/auth/tokens");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<Tuple<TokenId, AuthenticateResponse>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                IEnumerable<string> tokenValues;
                                if (!responseMessage.Headers.TryGetValues("X-Subject-Token", out tokenValues))
                                    tokenValues = null;

                                TokenId tokenId = null;
                                if (tokenValues != null)
                                    tokenId = new TokenId(tokenValues.First());

                                AuthenticateResponse authenticateResponse = null;
                                if (!string.IsNullOrEmpty(innerTask.Result))
                                    authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(innerTask.Result);

                                return Tuple.Create(tokenId, authenticateResponse);
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AuthenticateApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetTokenApiCall> PrepareGetTokenAsync(TokenId tokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/auth/tokens");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ValidateTokenApiCall> PrepareValidateTokenAsync(TokenId tokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/auth/tokens");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveTokenApiCall> PrepareRemoveTokenAsync(TokenId tokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/auth/tokens");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        #endregion

        #region Service catalog

        /// <inheritdoc/>
        public virtual Task<AddServiceApiCall> PrepareAddServiceAsync(ServiceData serviceData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/services");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListServicesApiCall> PrepareListServicesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/services");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetServiceApiCall> PrepareGetServiceAsync(ServiceId serviceId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/services/{service_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "service_id", serviceId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateServiceApiCall> PrepareUpdateServiceAsync(ServiceId serviceId, ServiceData serviceData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/services/{service_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "service_id", serviceId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveServiceApiCall> PrepareRemoveServiceAsync(ServiceId serviceId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/services/{service_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "service_id", serviceId.Value } };

            throw new NotImplementedException();
        }

        #endregion

        #region Endpoints

        /// <inheritdoc/>
        public virtual Task<AddEndpointApiCall> PrepareAddEndpointAsync(EndpointRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/endpoints");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListEndpointsApiCall> PrepareListEndpointsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/endpoints");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetEndpointApiCall> PrepareGetEndpointAsync(EndpointId endpointId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/endpoints/{endpoint_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "endpoint_id", endpointId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateEndpointApiCall> PrepareUpdateEndpointAsync(EndpointId endpointId, EndpointRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/endpoints/{endpoint_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "endpoint_id", endpointId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveEndpointApiCall> PrepareRemoveEndpointAsync(EndpointId endpointId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/endpoints/{endpoint_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "endpoint_id", endpointId.Value } };

            throw new NotImplementedException();
        }

        #endregion

        #region Domains

        /// <inheritdoc/>
        public virtual Task<AddDomainApiCall> PrepareAddDomainAsync(DomainRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListDomainsApiCall> PrepareListDomainsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetDomainApiCall> PrepareGetDomainAsync(DomainId domainId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "domain_id", domainId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateDomainApiCall> PrepareUpdateDomainAsync(DomainId domainId, DomainRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "domain_id", domainId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveDomainApiCall> PrepareRemoveDomainAsync(DomainId domainId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "domain_id", domainId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListDomainUserRolesApiCall> PrepareListDomainUserRolesAsync(DomainId domainId, UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/users/{user_id}/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "domain_id", domainId.Value }, { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<AddDomainUserRoleApiCall> PrepareAddDomainUserRoleAsync(DomainId domainId, UserId userId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/users/{user_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "domain_id", domainId.Value },
                { "user_id", userId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ValidateDomainUserRoleApiCall> PrepareValidateDomainUserRoleAsync(DomainId domainId, UserId userId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/users/{user_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "domain_id", domainId.Value },
                { "user_id", userId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveDomainUserRoleApiCall> PrepareRemoveDomainUserRoleAsync(DomainId domainId, UserId userId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/users/{user_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "domain_id", domainId.Value },
                { "user_id", userId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListDomainGroupRolesApiCall> PrepareListDomainGroupRolesAsync(DomainId domainId, GroupId groupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/group/{group_id}/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "domain_id", domainId.Value }, { "group_id", groupId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<AddDomainGroupRoleApiCall> PrepareAddDomainGroupRoleAsync(DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/group/{group_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "domain_id", domainId.Value },
                    { "group_id", groupId.Value },
                    { "role_id", roleId.Value }
                };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ValidateDomainGroupRoleApiCall> PrepareValidateDomainGroupRoleAsync(DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/group/{group_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "domain_id", domainId.Value },
                    { "group_id", groupId.Value },
                    { "role_id", roleId.Value }
                };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveDomainGroupRoleApiCall> PrepareRemoveDomainGroupRoleAsync(DomainId domainId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/domains/{domain_id}/group/{group_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "domain_id", domainId.Value },
                    { "group_id", groupId.Value },
                    { "role_id", roleId.Value }
                };

            throw new NotImplementedException();
        }

        #endregion

        #region Projects

        /// <inheritdoc/>
        public virtual Task<AddProjectApiCall> PrepareAddProjectAsync(ProjectRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListProjectsApiCall> PrepareListProjectsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetProjectApiCall> PrepareGetProjectAsync(ProjectId projectId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "project_id", projectId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateProjectApiCall> PrepareUpdateProjectAsync(ProjectId projectId, ProjectRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "project_id", projectId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveProjectApiCall> PrepareRemoveProjectAsync(ProjectId projectId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "project_id", projectId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListProjectUserRolesApiCall> PrepareListProjectUserRolesAsync(ProjectId projectId, UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/users/{user_id}/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "user_id", userId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<AddProjectUserRoleApiCall> PrepareAddProjectUserRoleAsync(ProjectId projectId, UserId userId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/users/{user_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "user_id", userId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ValidateProjectUserRoleApiCall> PrepareValidateProjectUserRoleAsync(ProjectId projectId, UserId userId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/users/{user_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "user_id", userId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveProjectUserRoleApiCall> PrepareRemoveProjectUserRoleAsync(ProjectId projectId, UserId userId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/users/{user_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "user_id", userId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListProjectGroupRolesApiCall> PrepareListProjectGroupRolesAsync(ProjectId projectId, GroupId groupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/groups/{group_id}/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "group_id", groupId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<AddProjectGroupRoleApiCall> PrepareAddProjectGroupRoleAsync(ProjectId projectId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/groups/{group_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "group_id", groupId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ValidateProjectGroupRoleApiCall> PrepareValidateProjectGroupRoleAsync(ProjectId projectId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/groups/{group_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "group_id", groupId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveProjectGroupRoleApiCall> PrepareRemoveProjectGroupRoleAsync(ProjectId projectId, GroupId groupId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/projects/{project_id}/groups/{group_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "project_id", projectId.Value },
                { "group_id", groupId.Value },
                { "role_id", roleId.Value }
            };

            throw new NotImplementedException();
        }

        #endregion

        #region Users

        /// <inheritdoc/>
        public virtual Task<AddUserApiCall> PrepareAddUserAsync(UserRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListUsersApiCall> PrepareListUsersAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetUserApiCall> PrepareGetUserAsync(UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users/{user_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateUserApiCall> PrepareUpdateUserAsync(UserId userId, UserRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users/{user_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveUserApiCall> PrepareRemoveUserAsync(UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users/{user_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListUserGroupsApiCall> PrepareListUserGroupsAsync(UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users/{user_id}/groups");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListUserProjectsApiCall> PrepareListUserProjectsAsync(UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users/{user_id}/projects");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListUserRolesApiCall> PrepareListUserRolesAsync(UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/users/{user_id}/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        #endregion

        #region Groups

        /// <inheritdoc/>
        public virtual Task<AddGroupApiCall> PrepareAddGroupAsync(GroupRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListGroupsApiCall> PrepareListGroupsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetGroupApiCall> PrepareGetGroupAsync(GroupId groupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateGroupApiCall> PrepareUpdateGroupAsync(GroupId groupId, GroupRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveGroupApiCall> PrepareRemoveGroupAsync(GroupId groupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListGroupUsersApiCall> PrepareListGroupUsersAsync(GroupId groupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}/users");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<AddGroupUserApiCall> PrepareAddUserToGroupAsync(GroupId groupId, UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}/users/{user_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value }, { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveGroupUserApiCall> PrepareRemoveUserFromGroupAsync(GroupId groupId, UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}/users/{user_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value }, { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ValidateGroupUserApiCall> PrepareValidateUserInGroupAsync(GroupId groupId, UserId userId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/groups/{group_id}/users/{user_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "group_id", groupId.Value }, { "user_id", userId.Value } };

            throw new NotImplementedException();
        }

        #endregion

        #region Credentials

        /// <inheritdoc/>
        public virtual Task<AddCredentialApiCall> PrepareAddCredentialAsync(CredentialData credentialData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/credentials");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListCredentialsApiCall> PrepareListCredentialsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/credentials");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetCredentialApiCall> PrepareGetCredentialAsync(CredentialId credentialId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/credentials/{credential_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "credential_id", credentialId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateCredentialApiCall> PrepareUpdateCredentialAsync(CredentialId credentialId, CredentialData credentialData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/credentials/{credential_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "credential_id", credentialId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveCredentialApiCall> PrepareRemoveCredentialAsync(CredentialId credentialId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/credentials/{credential_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "credential_id", credentialId.Value } };

            throw new NotImplementedException();
        }

        #endregion

        #region Roles

        /// <inheritdoc/>
        public virtual Task<AddRoleApiCall> PrepareAddRoleAsync(RoleRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListRolesApiCall> PrepareListRolesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/roles");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetRoleApiCall> PrepareGetRoleAsync(RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "role_id", roleId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdateRoleApiCall> PrepareUpdateRoleAsync(RoleId roleId, RoleRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "role_id", roleId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemoveRoleApiCall> PrepareRemoveRoleAsync(RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "role_id", roleId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListRoleAssignmentsApiCall> PrepareListRoleAssignmentsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/role_assignments");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        #endregion

        #region Policies

        /// <inheritdoc/>
        public virtual Task<AddPolicyApiCall> PrepareAddPolicyAsync(PolicyData policyData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/policies");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<ListPoliciesApiCall> PrepareListPoliciesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/policies");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetPolicyApiCall> PrepareGetPolicyAsync(PolicyId policyId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/policies/{policy_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "policy_id", policyId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<UpdatePolicyApiCall> PrepareUpdatePolicyAsync(PolicyId policyId, PolicyData policyData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/policies/{policy_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "policy_id", policyId.Value } };

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<RemovePolicyApiCall> PrepareRemovePolicyAsync(PolicyId policyId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/policies/{policy_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "policy_id", policyId.Value } };

            throw new NotImplementedException();
        }

        #endregion

        /// <inheritdoc/>
        public virtual TExtension GetServiceExtension<TExtension>(ServiceExtensionDefinition<IIdentityService, TExtension> definition)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            return definition.CreateDefaultInstance(this, this);
        }

        /// <summary>
        /// This class extends the support of <see cref="IdentityClientAuthenticationService"/> to include support
        /// for <see cref="IdentityClient"/> calls.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        protected class IdentityClientAuthenticationService : BaseIdentityClientAuthenticationService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IdentityClientAuthenticationService"/> class with the
            /// specified delegate authentication service implementations to use for authenticating calls made from a
            /// client.
            /// </summary>
            /// <inheritdoc/>
            public IdentityClientAuthenticationService(Uri baseAddress, IAuthenticationService authenticatedCallsService, IAuthenticationService unauthenticatedCallsService)
                : base(baseAddress, authenticatedCallsService, unauthenticatedCallsService)
            {
            }

            /// <inheritdoc/>
            /// <remarks>
            /// <para>The OpenStack Identity Service V3 does not currently contain any unauthenticated calls aside from
            /// the ones defined in <see cref="IBaseIdentityService"/>.</para>
            /// </remarks>
            protected override bool? IsAuthenticatedCall(HttpRequestMessage requestMessage)
            {
                return base.IsAuthenticatedCall(requestMessage);
            }
        }
    }
}
