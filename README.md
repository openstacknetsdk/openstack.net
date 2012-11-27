openstack.net
=============

.Net SDK and client side utilities for common openstack providers

Below is a list of required Openstack features and our current progress.  We have split off the progress for the Rackspace OpenStack provider and a generic Openstack provider.
<div class="table_wrapper">
	<table class="progress_tbl">
		<colgroup><col /> <col /> <col /> <col /> <col /> <col /> <col /> <col /> </colgroup>
		<tbody>
			<tr>
				<td colspan="4">
					<div style="float:right">
						<img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" />&nbsp;Completed&nbsp;&nbsp;
						<img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" />&nbsp;Coded:&nbsp;Pending&nbsp;Tests&nbsp;&nbsp;
						<img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_3_sm.png" />&nbsp;Not&nbsp;Supported&nbsp;&nbsp;
					</div>
				</td>
			<tr>				
				<td><strong>Product</strong></td>
				<td><strong>Feature</strong></td>
				<td><strong>Rackspace</strong></td>
				<td><strong>Openstack</strong></td></tr>
			<tr>				
				<td colspan="4">General</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Handle ratelimit failures</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Caches auth token</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Re-authenticates if necessary</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Does NOT store username or API key in persistent storage</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_3_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>RAX:KSKEY authentication</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_3_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>90% unit test coverage</td>
				<td>Ongoing</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td><a href="/display/DRG/Smoke+Test">Smoketest with RAX public cloud</a></td>
				<td>Ongoing</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Service catalog — select endpoint</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>				
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td colspan="4">Servers/Compute</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Servers</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Server</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Support scheduler hints</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Server</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Server</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Support AccessIP</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List addresses</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List addresses by network</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Support IPv6</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Support DiskConfig on server create</td>				
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Support DiskConfig on image create</td>				
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Change Admin Password</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Reboot</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Rebuild</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Resize</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Confirm Resized</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Revert Resized</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Image</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Rescue</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Unrescue</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Volumes</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Attach Volume</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Server Details</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Images</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Image Details</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Image</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Flavors</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Flavor Details</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Server Bandwidth (RAX)</td>				
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Metadata</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_2_sm.png" /></td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Set Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>
				<td colspan="4">Cloud Networks</td></tr>
			<tr>
				<td>&nbsp;</td>
				<td>Create Network</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>
				<td>&nbsp;</td>
				<td>Delete Network</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>
				<td>&nbsp;</td>
				<td>List Networks</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>
				<td>&nbsp;</td>
				<td>Create Server attached to Cloud Network</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>
				<td>&nbsp;</td>
				<td>Handle special 'public' and 'private' networks seamlessly</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td colspan="4">Files</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Containers</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Account Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Objects in Container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Object Filters</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update container Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Object</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create/Update object</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Large object support</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Chunking</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Copy object</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>delete object</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get object metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List CDN Containers</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Enable CDN Container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Metadata CDN Container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Purge CDN Container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update CDN container metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>CDN Streaming Container</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Purge CDN Object</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Static Website</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>
				<td colspan="4">Load Balancers</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Remove</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Stats</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Nodes</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Add Node</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Modify Node</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Remove Node</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Virtual Ips</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Allowed Domains</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Add IPV6 IP</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Remove Virtual IP</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Usage</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Access List</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Access List</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Dlete Access List</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Monitor Health</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Connections</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>HTTP/HTTPS</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Manage Session Persistence</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Log Connections</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Throttle Connections</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Content Caching</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Protocols</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Algorithms</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update SSL termination</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Add Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Modify Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Remove Metadata</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td colspan="4">Databases</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Instances</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Instance</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get instance details</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Instance</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Enable Root User</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Root Users</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Restart Instance</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Resize Instance</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Resize Instance Volume</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Database</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Databases</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Database</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create User</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Users</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Users</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Flavors</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Flavor by ID</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td colspan="4">DNS</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List all Limits</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Limit Types</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List specific Limit</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Domains</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Domain Details</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Domain Changes</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>export Domain</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>create domain</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>import domain</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>modify domain</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>remove domain</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>list subdomains</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>list records</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>search records</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>list record details</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>add records</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>modify records</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>remove records</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List PTR Records</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List PTR Record Details</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Add PTR</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Modify PTR</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Remove PTR</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td colspan="4">Identity</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Users</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get user by name</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>get user by id</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>add user</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>update user</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Users</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>list credentials</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>get user credentials</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>list user global roles</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>authenticate token</td>
				<td><img src="http://c87551950a9ea8b9ff4a-3de1324fa419f1d38a3b4315c44fd19c.r96.cf1.rackcdn.com/status_1_sm.png" /></td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>get tenants</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td colspan="4">Monitoring</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Limits</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Account</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Account</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Limits</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Audits</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Entity</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Entities</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Entity</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Entity</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Entity</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Check</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Test Check</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Test Check and Include Debug Information</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Test Existing Check</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Checks</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Check</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Checks</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Checks</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Check Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Check Types</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Check Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Check Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Check Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Alarm</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Test Alarm</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Alarms</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Alarm</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Alarm</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Alarm</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Notification Plan</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Notification Plans</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Notification Plan</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Notification Plans</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Notification Plans</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Monitoring Zone</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Monitoring Zones</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Monitoring Zone</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Monitoring Zone</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Monitoring Zone</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Traceroute from Monitoring Zone</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Alarm Notification History</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Alarm Notification History</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Notification</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Test Notification</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Notifications</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Notifications</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Notifications</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Notifications</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Notification Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Notification Types</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Notification Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Notification Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Notification Type</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Alarm Changelogs</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Views Get Overview</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Alarm Examples</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Alarm Example</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Evaluate Alarm Example</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Agents</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Agent</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Agent Connections</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Agent Connection</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Create Agent Token</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>List Agent Tokens</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Get Agent Token</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Update Agent Token</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
			<tr>				
				<td>&nbsp;</td>
				<td>Delete Agent Token</td>
				<td>&nbsp;</td>
				<td>&nbsp;</td></tr>
		</tbody>
	</table>
</div>

    