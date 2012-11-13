openstack.net
=============

.Net SDK and client side utilities for common openstack providers

Below is a list of required Openstack features and our current progress.  We have split off the progress for the Rackspace OpenStack provider and a generic Openstack provider.
        <div class="table_wrapper"><table class="progress_tbl"><colgroup><col /> <col /> <col /> <col /> <col /> <col /> <col /> <col /> </colgroup><tbody><tr><td>Product</td><td>Feature</td><td>
        Rackspace </td><td>
            Openstack</td></tr><tr><td>General</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>Handle ratelimit failures</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Caches auth token</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Re-authenticates if necessary</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Does NOT store username or API key in persistent storage</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>RAX:KSKEY authentication</td><td>
            Done</td><td>
                N/A</td></tr><tr><td> </td><td>90% unit test coverage</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td><a href="/display/DRG/Smoke+Test">Smoketest with RAX public cloud</a></td><td>
            Ongoing</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Service catalog — select endpoint</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Wait for status change of object</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td> </td><td> </td><td> 
        &nbsp;</td></tr><tr><td>Servers/Compute</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td>
        &nbsp;</td><td>List Servers</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Create Server</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Support scheduler hints</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Server</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Server</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Support AccessIP</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List addresses</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List addresses by network</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Support IPv6</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Support DiskConfig on server create</td><td> </td><td> 
            &nbsp;</td></tr><tr><td> </td><td>Support DiskConfig on image create</td><td> </td><td> 
            &nbsp;</td></tr><tr><td> </td><td>Change Admin Password</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Reboot</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Rebuild</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Resize</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Confirm Resized</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Revert Resized</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Image</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Rescue</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Unrescue</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Volumes</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Attach Volume</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Details</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Images</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Image Details</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Flavors</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Flavor Details</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Server Bandwidth (RAX)</td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>List Metadata</td><td>
        Done</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Set Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td colspan="1">Cloud Networks</td><td colspan="1">Create Network</td><td colspan="1"> </td>
            <td> &nbsp;</td></tr><tr><td colspan="1"> </td><td colspan="1">Delete Network</td><td colspan="1"> </td>
        <td> &nbsp;</td></tr><tr><td colspan="1"> </td><td colspan="1">List Networks</td><td colspan="1"> </td>
        <td> &nbsp;</td></tr><tr><td colspan="1"> </td><td colspan="1">Create Server attached to Cloud Network</td><td colspan="1"> </td>
        <td> &nbsp;</td></tr><tr><td colspan="1"> </td><td colspan="1">Handle special 'public' and 'private' networks seamlessly</td><td colspan="1"> </td>
        <td> &nbsp;</td></tr><tr><td>Files</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>List Containers</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Get Account Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Objects in Container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Object Filters</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update container Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Object</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create/Update object</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Large object support</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Chunking</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Copy object</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>delete object</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get object metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List CDN Containers</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Enable CDN Container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Metadata CDN Container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Purge CDN Container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update CDN container metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>CDN Streaming Container</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Purge CDN Object</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Static Website</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td>Load Balancers</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>List</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Create</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Remove</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Stats</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Nodes</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Add Node</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Modify Node</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Remove Node</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Virtual Ips</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Allowed Domains</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Add IPV6 IP</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Remove Virtual IP</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Usage</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Access List</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Access List</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Dlete Access List</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Monitor Health</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Connections</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>HTTP/HTTPS</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Manage Session Persistence</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Log Connections</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Throttle Connections</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Content Caching</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Protocols</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Algorithms</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update SSL termination</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Add Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Modify Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Remove Metadata</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td>Databases</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>List Instances</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Create Instance</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get instance details</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Instance</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Enable Root User</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Root Users</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Restart Instance</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Resize Instance</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Resize Instance Volume</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Database</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Databases</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Database</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create User</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Users</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Users</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Flavors</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Flavor by ID</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td>DNS</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>List all Limits</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>List Limit Types</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List specific Limit</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Domains</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Domain Details</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Domain Changes</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>export Domain</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>create domain</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>import domain</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>modify domain</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>remove domain</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>list subdomains</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>list records</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>search records</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>list record details</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>add records</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>modify records</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>remove records</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List PTR Records</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List PTR Record Details</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Add PTR</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Modify PTR</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Remove PTR</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td>Identity</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>List Users</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Get user by name</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>get user by id</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>add user</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>update user</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Users</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>list credentials</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>get user credentials</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>list user global roles</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>authenticate token</td><td>
            Done</td><td>
                &nbsp;</td></tr><tr><td> </td><td>get tenants</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td>Monitoring</td><td> </td><td> </td><td> &nbsp;</td></tr><tr><td> </td><td>Limits</td><td>
        &nbsp;</td><td>
            &nbsp;</td></tr><tr><td> </td><td>Get Account</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Account</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Limits</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Audits</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Entity</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Entities</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Entity</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Entity</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Entity</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Check</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Test Check</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Test Check and Include Debug Information</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Test Existing Check</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Checks</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Check</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Checks</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Checks</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Check Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Check Types</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Check Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Check Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Check Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Alarm</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Test Alarm</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Alarms</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Alarm</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Alarm</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Alarm</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Notification Plan</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Notification Plans</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Notification Plan</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Notification Plans</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Notification Plans</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Monitoring Zone</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Monitoring Zones</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Monitoring Zone</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Monitoring Zone</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Monitoring Zone</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Traceroute from Monitoring Zone</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Alarm Notification History</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Alarm Notification History</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Notification</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Test Notification</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Notifications</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Notifications</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Notifications</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Notifications</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Notification Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Notification Types</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Notification Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Notification Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Notification Type</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Alarm Changelogs</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Views Get Overview</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Alarm Examples</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Alarm Example</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Evaluate Alarm Example</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Agents</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Agent</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Agent Connections</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Agent Connection</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Create Agent Token</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>List Agent Tokens</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Get Agent Token</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Update Agent Token</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr><tr><td> </td><td>Delete Agent Token</td><td>
            &nbsp;</td><td>
                &nbsp;</td></tr></tbody></table></div>

    