namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListRoleAssignmentsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<RoleAssignment>>
    {
        public ListRoleAssignmentsApiCall(IHttpApiCall<ReadOnlyCollectionPage<RoleAssignment>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
