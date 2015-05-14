namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.ObjectModel.JsonHome;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity;
    using Rackspace.Net;
    using Rackspace.Threading;

    using System.Diagnostics;


    /// <summary>
    /// This class provides a default implementation of <see cref="IContentDeliveryService"/> suitable for connecting to
    /// OpenStack-compatible installations of the Content Delivery Service V1.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ContentDeliveryClient : ServiceClient, IContentDeliveryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentDeliveryClient"/> class with the specified
        /// authentication service, default region, and value indicating whether an internal or public endpoint should
        /// be used for communicating with the service.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="defaultRegion">The preferred region for the service. Unless otherwise specified for a specific
        /// client, derived service clients will not use a default region if this value is <see langword="null"/> (i.e.
        /// only region-less or "global" service endpoints will be considered acceptable).</param>
        /// <param name="internalUrl"><see langword="true"/> to access the service over a local network; otherwise,
        /// <see langword="false"/> to access the service over a public network (the Internet).</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="authenticationService"/> is <see langword="null"/>.
        /// </exception>
        public ContentDeliveryClient(IAuthenticationService authenticationService, string defaultRegion, bool internalUrl)
            : base(authenticationService, defaultRegion, internalUrl)
        {
        }

        #region IContentDeliveryService Members

        /// <inheritdoc/>
        public virtual Task<GetHomeApiCall> PrepareGetHomeAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate(string.Empty);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken, "application/json"))
                .Select(task => new GetHomeApiCall(CreateJsonApiCall<HomeDocument>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<GetHealthApiCall> PrepareGetHealthAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<GetSubsystemHealthApiCall> PrepareGetSubsystemHealthAsync(SubsystemId subsystemId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<PingApiCall> PreparePingAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("ping");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken, "application/json"))
                //.Select(RemoveAcceptHeader) **DLS**
                .Select(task => new PingApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListServicesApiCall> PrepareListServicesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("services");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Service>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                JObject resultObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray servicesArray = resultObject["services"] as JArray;

                                IList<Service> list = servicesArray.ToObject<Service[]>();
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<Service>>>> prepareGetNextPageAsync = null;

                                JArray linksArray = resultObject["links"] as JArray;
                                Uri nextPage = null;
                                Link[] links = linksArray != null ? linksArray.ToObject<Link[]>() : null;
                                if (links != null)
                                {
                                    Link nextPageLink = links.FirstOrDefault(i => string.Equals("next", i.Relation, StringComparison.OrdinalIgnoreCase));
                                    nextPage = nextPageLink != null ? nextPageLink.Target : null;
                                }

                                if (nextPage != null)
                                {
                                    prepareGetNextPageAsync =
                                        innerCancellationToken2 =>
                                        {
                                            return PrepareListServicesAsync(innerCancellationToken2)
                                                .WithUri(nextPage)
                                                .Select(_ => _.Result.AsHttpApiCall());
                                        };
                                }

                                ReadOnlyCollectionPage<Service> results = new BasicReadOnlyCollectionPage<Service>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListServicesApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetServiceApiCall> PrepareGetServiceAsync(ServiceId serviceId, CancellationToken cancellationToken)
        {
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");

            UriTemplate template = new UriTemplate("services/{service_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "service_id", serviceId.Value } };
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetServiceApiCall(CreateJsonApiCall<Service>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<AddServiceApiCall> PrepareAddServiceAsync(ServiceData serviceData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("services");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ServiceId>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    string absolutePath = responseMessage.Headers.Location.AbsolutePath;
                    int lastSlash = absolutePath.LastIndexOf('/');
                    return CompletedTask.FromResult(new ServiceId(absolutePath.Substring(lastSlash + 1)));
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, serviceData, cancellationToken))
                .Select(task => new AddServiceApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<UpdateServiceApiCall> PrepareUpdateServiceAsync(ServiceId serviceId, ServiceData updatedServiceData, CancellationToken cancellationToken)
        {
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");



            // At this point, the serviceData parameter contains the NEW (AFTER) version
            // of the service data. Now, we need to retrieve the current (BEFORE) version,
            // then use those to calculate the difference, i.e. the JSON Patch document.
            var getServiceTask = ContentDeliveryServiceExtensions.GetServiceAsync(this, serviceId, cancellationToken);
            getServiceTask.Wait();
            ServiceData originalServiceData = getServiceTask.Result;



            // Here's a serious HACK: Because the current ServiceData includes properties
            // that are not part of a new instance of ServiceData (such as "Id", "Status" and others)
            // we need to create a new instance using the data from the current ServiceData object.
            // That way, when we perform the "DIFF" operation to calculate the JSon Patch object,
            // the extra properties won't interfere.
            ServiceData tempServiceData = new ServiceData(originalServiceData.Name, originalServiceData.FlavorId, originalServiceData.Domains, originalServiceData.Origins, originalServiceData.CachingRules, originalServiceData.Restrictions);



            // Calculate the Json Patch document
            var beforeUpdate = JToken.Parse(JsonConvert.SerializeObject(tempServiceData));
            var afterUpdate = JToken.Parse(JsonConvert.SerializeObject(updatedServiceData));
            var patchDoc = new JsonDiffer().Diff(beforeUpdate, afterUpdate);


            // Another Hack: For some reason (TODO: which needs to be investigated and fixed),
            // we need to deserialize the document in order for it to render a proper Json object.
            var jsonPatchDocument = JsonConvert.DeserializeObject(patchDoc.ToString());


            // Now we can call the PATCH method to update the ServiceData on the server.
            UriTemplate template = new UriTemplate("services/{service_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "service_id", serviceId.Value } };
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(new HttpMethod("PATCH"), "application/json-patch+json", template, parameters, jsonPatchDocument, cancellationToken))
                .Select(task => new UpdateServiceApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<RemoveServiceApiCall> PrepareRemoveServiceAsync(ServiceId serviceId, CancellationToken cancellationToken)
        {
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");

            UriTemplate template = new UriTemplate("services/{service_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "service_id", serviceId.Value } };
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveServiceApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<RemoveAssetApiCall> PrepareRemoveAssetAsync(ServiceId serviceId, CancellationToken cancellationToken, string urlOfAsset = null, bool deleteAll = false)
        {
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");

            UriTemplate template = new UriTemplate("services/{service_id}/assets?url={url_of_asset}&all={delete_all}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { 
                { "service_id", serviceId.Value },
                { "url_of_asset", urlOfAsset },
                { "delete_all", deleteAll.ToString() }
                };
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveAssetApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListFlavorsApiCall> PrepareListFlavorsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("flavors");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Flavor>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage)) 
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                JObject resultObject = JsonConvert.DeserializeObject<JObject>(innerTask.Result);
                                JArray flavorsArray = resultObject["flavors"] as JArray;

                                IList<Flavor> list = flavorsArray.ToObject<Flavor[]>();
                                Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<Flavor>>>> prepareGetNextPageAsync = null;

                                JArray linksArray = resultObject["links"] as JArray;
                                Uri nextPage = null;
                                Link[] links = linksArray != null ? linksArray.ToObject<Link[]>() : null;
                                if (links != null)
                                {
                                    Link nextPageLink = links.FirstOrDefault(i => string.Equals("next", i.Relation, StringComparison.OrdinalIgnoreCase));
                                    nextPage = nextPageLink != null ? nextPageLink.Target : null;
                                }

                                if (nextPage != null)
                                {
                                    prepareGetNextPageAsync =
                                        innerCancellationToken2 =>
                                        {
                                            return PrepareListFlavorsAsync(innerCancellationToken2)
                                                .WithUri(nextPage)
                                                .Select(_ => _.Result.AsHttpApiCall());
                                        };
                                }

                                ReadOnlyCollectionPage<Flavor> results = new BasicReadOnlyCollectionPage<Flavor>(list, prepareGetNextPageAsync);
                                return results;
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListFlavorsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetFlavorApiCall> PrepareGetFlavorAsync(FlavorId flavorId, CancellationToken cancellationToken)
        {
            if (flavorId == null)
                throw new ArgumentNullException("flavorId");

            UriTemplate template = new UriTemplate("flavors/{flavor_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "flavor_id", flavorId.Value } };
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetFlavorApiCall(CreateJsonApiCall<Flavor>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<AddFlavorApiCall> PrepareAddFlavorAsync(FlavorData flavorData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("flavors");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<FlavorId>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    string absolutePath = responseMessage.Headers.Location.AbsolutePath;
                    int lastSlash = absolutePath.LastIndexOf('/');
                    return CompletedTask.FromResult(new FlavorId(absolutePath.Substring(lastSlash + 1)));
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, flavorData, cancellationToken))
                .Select(task => new AddFlavorApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<RemoveFlavorApiCall> PrepareRemoveFlavorAsync(FlavorId flavorId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual TExtension GetServiceExtension<TExtension>(ServiceExtensionDefinition<IContentDeliveryService, TExtension> definition)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            return definition.CreateDefaultInstance(this, this);
        }

        #endregion

        /// <summary>
        /// Removes the <see cref="HttpRequestHeaders.Accept"/> header associated with a request message.
        /// </summary>
        /// <param name="requestMessageTask">A task containing the HTTP request message.</param>
        /// <returns>The modified <see cref="HttpRequestMessage"/> value.</returns>
        protected virtual HttpRequestMessage RemoveAcceptHeader(Task<HttpRequestMessage> requestMessageTask)
        {
            requestMessageTask.Result.Headers.Accept.Clear();
            return requestMessageTask.Result;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method calls <see cref="IAuthenticationService.GetBaseAddressAsync"/> to obtain a URI for the type
        /// <c>cdn</c>. The preferred name is not specified.
        /// </remarks>
        protected override Task<Uri> GetBaseUriAsyncImpl(CancellationToken cancellationToken)
        {
            const string serviceType = "cdn";
            const string serviceName = null;
            return AuthenticationService.GetBaseAddressAsync(serviceType, serviceName, DefaultRegion, InternalUrl, cancellationToken);
        }
    }
}
