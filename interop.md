# UPP Interoperability Specification

The Unified Permitting Project (UPP) Interoprability Specificaion is intended to define the behavior and interfaces of systems that wish to participate as peers within the distributed UPP platform.  This document contains a technical, but not a formal, definition and  description of the the specification's components.  A separate validation suite is provided in order to test the compliance of implemnting systems agais the specification.

## Overview

Today, in order to apply for an oversize/overweight permit, members of the hauling industry need toapply for these types of permits for large loads through each individual roadway authority they will be traveling through. MnDOT, counties and cities all administer permits for their own roadways. This requires several different permit applications and processes from each roadway authority for an individual hauler.

Local government agencies and MnDOT each use different software or paper/FAX systems to issue
permits. The process of setting requirements to issue a permit is a common element between the various systems whether digital or paper. Each agency has set information requirements for issuing permits, some include vehicle registration details, weight, axle details, dimensions (width, height, length, overhang), trailer details, route description, other permits, and hauling dates.

The purpose of this specification is to define a feasible implementation of a unified permitting process for oversize/overweight vehicle permit applications across all roadway authorities.

## Definitions

The goal of the UPP specification is to define correct communication and coordination among a heterogeneous collection of independent systems.  As such, there are many UPP domain-specific concepts that are heavily used through the specification that are defined in this section.

### General Terms

Authentication
: The act of confirming the identity of a principal through a validation mechanism

Authorization
: The function of evaluating and enforcing a set of access rights of a principal to resources

Authority
: A UPP Authority is an independent, trusted entity that provides one or more UPP platform services.

Claim
: A claim is a piece of information that is bound to an identity.  Common claims are first and last names, email address and other contact information.  However a claim can be anything and is oftern used to hold system-specific information.

Identity
: A collection of information (attributes) that represent an external agent. An identity typically refers to information about an individual person, such as their name and contact information.

Identity Provider (IdP)
: A system entity that creates, maintains, and manages identity information for principals while providing authentication services to relying party applications within a federation or distributed network.

Microservice
:  A microservice is a unit of software that is deployed as a service and provides an independent and complete business capability.

MIME Type
:  The MIME Type is a standardized way to indicate the nature and format of a document that follows the format of `type/subtype`. Each document in UPP has a unique MIME Type assigned.

OAuth
:  An open standard for token-based authentication and authorization on the Internet.

Pricipal
: An entity that can be authenticated by a computer system or network. This is typically a individual person, but devices and other systems may also be principals.

Resource
: A resource can be anything that has identity and is addressable by a Uniform Resource Locator (URL).  A resource can remain constant even when its content changes over time

Scope
: A way for a priciple to expressthe desired level of access to a resource during the authorization process. Scopes are allowed or rejected based on the Identity of the principal and allow for granular access to resources.

### Authorities

Within the initial design of UPP, authorities may be cities, counties, or state agencies.  Private companies will not be authorities themselves, but may exposed services that implement specific API scopes on behalf of authorities.

Given the limited number of authories, the UPP system defines an exhaustive list of authorities.  In the case of private parties providing services, their trust must be delegated by one of the defined authorities.

#### Representation

An authority is identified by a string representation with the format

   `<name>_<org>_<state>`

Where the `name` is a lowercase version of the legal name of the entity with spaces replaced with hyphens.  The `org` is the organization level of the authority represented as a three-letter code. Finally, the `state` identifies the state the authority operates within.

It is important to note that a single legal entity may have multiple UPP Authority names.  The authority is means to designate _operating_ authority; that is, who has ownership of some UPP action within a designated area.

##### Valid Names

All city, county, township, village, ward or other mulciple corporation names may be used as a valid name. If there are naming conflicts such as multiple cities or townships with the same name, the defined convention is to use a forward slash (`/`) to provide extra information to disambiguate the name.

##### Valid Organizational Designations

| Org | Code |
| - | - |
| City | `cty` |
| County| `cou` |
| Town / Township | `twn` |
| Village | `vlg` |
| Multi-Jurisdictional Authority | `mja` |
| State Agency | `agy`

##### Valid States

Currently, only `mn` is considered a valid state identitfier for UPP Authorities

#### Authority Examples

The following is a non-exhaustive list of valid UPP Authority identifiers.

| Authority Identifier | Authority Name |
| - | - |
| `st-louis_cou_mn` | St. Louis County |
| `duluth_cty_mn` | City of Duluth |
| `beatty_twn_mn` | Beatty Township |
| `dps_agy_mn` | Minnesota Department of Public Safety |
| `dvs_agy_mn` | Minnesota Driver and Vehicle Services |
| `red-river_mja_mn` | (Hypothetical) Red Revier Valley Hauling Authority |
| `akron/wi_twn_mn` | Akron Township in Wilkin County |
| `akron/bs_twn_mn` | Akron Township in Big Stone County |

### Claims

As defined above, a claim is a piece of information that is attatched to an Identity. UPP uses a [Json Web Token](https://tools.ietf.org/html/rfc7519#section-4) (JWT) for its representation of the current claims for a given identity. In addition to the claims defined in [RFC 7519](https://tools.ietf.org/html/rfc7519), UPP defines the following claims

#### Defined Claims

| Claim | Description |
| - | - |
| `upp` | An opaque data token that MUST be preserved across all UPP requests
| `email` | One or more email addresses as defined in [RFC 5322](https://tools.ietf.org/html/rfc5322).  Multiple email addresses are separated by spaces.
| `phone` | One or more telephone URIs. Multiple telephone numbers are separated by spaces. Telephone numbers MUST follow the formatting rules defined in [RFC 3966](https://tools.ietf.org/html/rfc3966).<br>A mobile phone number that has been approved for messaging from UPP MUST include a `mobile` parameter with an optional parameter value that defines the type of content that may be sent to this device. Valid values are `sms` and `mms` with `sms` being the default value.<br>UPP also defined two additional parameters that define the interaction model to be used with a telephone number. `fax` indetifies a phone number as a FAX machine numbr  and may be used as a target. `ivr` designates a phone number that should be used for interations via an Interactive Voice Response system.
| `roles` | The roles this user has with regard to UPP workflows.  The list of defined roles are listed below.  Each Identity Provider MAY provide additional roles as well and any UPP system MUST propagate these roles in delegated requests.
| `scopes` | A list of scopes requested and approved by this identity.
| `tokens` | A list of all tokens provided by the Identity Providers.  This claim can be used by UPP authorities to verify if a user was authenticated by a specific Identity Provider and provide an extended set of capabilities, e.g. allow employees of that organization to perform additional functions suited to their internal position.

Any UPP system is allowed to define its own claims, however claims can only be set by an Identity Provider.

#### Claims Examples

A series of email claims

| Claim | Description
| - | - |
|`user@example.com` | A simple email address
|`user@example.com user@county.gov` | Two email addresses associated with an identity
|`user+tag@example.com` | An email address for `user@example.com` that includes a tag, which is ignored (but preserved) by email servers during transport.

A series of telephone claims

| Claim | Description
| - | - |
|`tel:+18888675309` | A global telephone number for an identity
|`tel:867-5309` | A local telephone number for a user that includes optional (but allowed) visual separators
|`tel:8675309 tel:8005551212` | A pair of phone numbers, one in local format and one in global format, that are associated with the identity
|`tel:+18888675309;mobile` | A global phone number with a mobile parameter.  UPP will use this number to send SMS messages only
|`tel:+18888675309;mobile=mms;ivr tel:+1-800-555-1212;fax` | A global phone number with a mobile parameter that marks it as accepting MMS messages.  This phone number may also be used to call the user and walk them through an IVR workflow. A second number is provided for FAX documents and contains visual separators

### Scopes

A scope provides a mechnism for limiting access to specific UPP resources.  Every identity will have certain lpatform scopes that it is allowed to access, but it is possible for each identify to request tokens with a limited scope for security reasons.

For example, a user that is allowed to edit Permit requests may choose to request a token with only `permit:review` in order to query the permit data and generate reports.  This way, they are assured that a misconfigured reports will not inadvertently cause a Permit to be altered.

Each API MAY require specific scopes to access its functional endpoints, but is not REQUIRED to do so.

| Scope | Description |
| - | - |
| `permit:request` | Grants read/write access to permit requests, read access to permit reviews, and read access to the Support API.
| `permit:review` | Grants read access to permit requests and permit reviews.
| `permit:enforcement` | Grants unrestricted read access to approved permits.
| `services:write` | Grants the ability to add/remove/update service registrations.
| `services:read` | Grants the ability to query service registrations.

### Roles

The roles claim provides a place to accumulate all of the functional roles held by an identity.  Typically these identify the department or organizational role that is held by the person.

| Role | Description |
| - | - |
| `hauler` | User is a hauler and generally entitled to interact with the systems in pursuit of obtaining a OSOW permit.
| `dispatcher` | User is a dispatcher and may be interating with the system on behalf of a third-party
| `enforcement` | User is involved inthe enforcement of permits in some capacity
| `issuer` | User is involved in issuing permits
| _`authority`_ | Any of the defined authorities may be included within the role claim to indicate that the user is officially affilliated with that authority for the purposes of UPP.
| `upp_admin` | Grants federated administrative authority to _any_ UPP system.<br><br>This is not an unchecked authority, however. Each UPP system is able to define for itself what resources fall under the authority of the `upp_admin` claim and can reserve sensitive, internal administrative access to other scopes which may be set only by Identity Providers trusted by that specific UPP system

## APIs

This describes the resource servers that comprise the UPP platform. If a system chooses to implement any of the APIs, all of the API methods MUST be implemented.

### Discovery API

The Discovery API allows systems to:

1. Register and describe themselves,
2. Look up other services based on type, scope and authority

#### List services

List the registered services.

```http
GET /services
```

##### Within Scopes

```text
services:read
```

##### Required Claims

* None

##### Parameters

| Name | Type | Description |
| - | - | -
| `type` | `string` | Can be `all` or any of the defined service types. Default: `all`
| `scope` | `string` | Can be `all` or any defined UPP scopes. Default: `all`
| `authority` | `string` | Indicates which authority's services should be returned
| `sort` | `string` | Can be one of `name`, `display_name` or `type`. Default: `name`
| `direction` | `string` | Can be one of `asc` or `desc`. Default: `desc`

The valid UPP Service Type are

* `route` for Esri-compatible routing services
* `geometry` for Esri-compatible geometry services
* `boundaries.county` for County boundaries used to identify a route's authorities
* `boundaries.city` for City boundaries used to identify a route's authorities
* `upp` for generic UPP data services
* `upp.information.axle` for UPP axle information
* `upp.information.company` for UPP company information
* `upp.information.insurance` for UPP insurance information
* `upp.information.trailer` for UPP trailer information
* `upp.information.truck` for UPP truck information
* `upp.information.vehicle` for UPP vehicle information

The valid UPP Service Data Formats are

* `esri_map_service` for Esri-compatible Map Layers Services
* `esri_feature_service` for Esri-compatible Feature Services
* `esri_geometry_service` for Esri-compatible Geometry Services
* `esri_network_service` for Esri-compatible Network Routing Services
* `json_api` for generic data in JSON API format

##### Response

The endpoint MUST return a collection of [UPP Microservice Configuration Records](interop/mimetypes/vnd.upp.microservice-config.json) in JSON API format. Each service is uniquely identified by its authority name concatenaded with the service type.

This allows any authority to provide, at most, one instance of each service but permits the same service to be provided by multiple authorities.  There are three common situations where multiple authorities will provide an implementation of the same service

1. Each authority needs to implment its own business processes against a service, e.g. permit application
1. Each authority may provide a partial source of knowledge, e.g. vehicle information
1. To provide redundency or consensus for dynamic data, e.g. replicated data sources or majority-rules query resolution.

```json
Content-Type: application/vnd.api+json

{
  "data": [
    {
      "id": "esri_com+route",
      "type": "microservice-config",
      "attributes": {
        "authority": "esri_com",
        "description": null,
        "displayName": "Esri Premium Routing Service",
        "format": "esri_network_service",
        "name": "esri.routing",
        "oAuthId": "agol",
        "priority": 1,
        "scopes": "route.information",
        "tokenId": "",
        "type": "route",
        "uri": "https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World"
      }
    },
    {
      "id": "esri_com+geometry",
      "type": "microservice-config",
      "attributes": {
        "authority": "esri_com",
        "description": null,
        "displayName": "Esri Geometry Service",
        "format": "esri_geometry_service",
        "name": "esri.geometry",
        "oAuthId": "",
        "priority": 1,
        "scopes": "geometry.services",
        "tokenId": "",
        "type": "geometry",
        "uri": "https://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer"
      }
    },
    {
      "id": "esri_com+county.boundaries",
      "type": "microservice-config",
      "attributes": {
        "authority": "esri_com",
        "description": null,
        "displayName": "Esri County Boundaries",
        "format": "esri_feature_service",
        "name": "esri.county.boundaries",
        "oAuthId": "",
        "priority": 1,
        "scopes": "county.boundaries",
        "tokenId": "",
        "type": "county.boundaries",
        "uri": "https://services.arcgis.com/BG6nSlhZSAWtExvp/arcgis/rest/services/counties_MN/FeatureServer/0"
      }
    },    
    {
      "id": "upp_mja_mn+upp.information.vehicle",
      "type": "microservice-config",
      "attributes": {
        "authority": "upp_mja_mn",
        "description": "A Vehicle Information provider that returns data in JSON API format.  This API supports UPP JWT authorization.",
        "displayName": "UPP Vehicle Information",
        "format": "json_api",
        "name": "_vehicle._upp",
        "oAuthId": "vendor",
        "priority": 1,
        "scopes": "information.vehicle",
        "tokenId": "vendor",
        "type": "upp.information.vehicle",
        "uri": "https://staging-upp.example.com/upp/data.php?page=vehicle"
      }
    }
  ],
  "meta": {},
  "jsonapi": {}
}
```

#### Register service

Add a new service to the services directory.

_The service directory implementation is influenced by Kubernetes but restricted. In Kubernetes terminology, the UPP services are closer to a Pod_

```http
POST /services
```

##### Within Scopes

```text
services:write
```

##### Required Claims

* None

##### Parameters

Registration MUST be passed in as a [UPP Microservice Registration Records](interop/mimetypes/vnd.upp.microservice-registration.json). Most of the parameters of the `service registration` record have a direct correspondence to the values returned by the registered services endpoint.

| Name | Type | Description |
| - | - | -
| `metadata.type` | `string` | The machine-friendly name of the service endppint.  A SVC-style name following the conventions of [Service DNS Records](https://en.wikipedia.org/wiki/SRV_record) and hanve the general form of `_service._proto`.  For example, `_insurance._upp`.
| `metadata.uid` | `string` | A valid unique identifier for the service. May be autogenerated.
| `labels.friendlyName` | `string` | A short, human-readable name for the service that will be used to label the service.  There is no limit, but it is recommended that the friendly name be kept to 32 characters or less.
| `labels.scopes` | `string` | A space-separated list of UPP service scopes implemented by this endpoint.
| `labels.authority` | `string` | The UPP Authority that is responsible for providing this service instance.
| `labels.type` | `string` | The UPP service type that is implemented by this service.
| `label.format` | `string` | The format of the data provided by this service endpoint.  Valid values are `esri_map_service`, `esri_feature_service`, `esri_geometry_service`, `esri_network_service` and `json_api`
|`annotations.description` | `string` | A longer description of the service.  This should include information about usage and suitability of the data service, if possible.
| `annotations.priority` | `number` | A value that sets the relative priority of the service.<br><br>If multiple services of the same type are registered, any UPP system MUST attempt to use higher priority data source before ones of lower priority.  Additionally, if two data sources with different priorities return data for the same entity, a UPP system MUST accept the data from the source of higher priority and discard the lower priority information.<br><br>It is service-defined whether lower-priority data responses can be merged on a per-record or per-attribute basis.
| `annotations.oAuthId` | `string` | References a specific set of cached OAuth credentials that should be unsed to aquire an access token from the service on behalf of the UPP client.  The method of acquiring the token is implementation-defined.
| `annotations.tokenId` | `string` | References a specific set of cached  credentials that should be unsed to aquire a generic access token from the service on behalf of the UPP client.  The method of acquiring the token is implementation-defined.


##### Request

```json
POST /api/v1/agent/register
Content-Type: application/vnd.upp.microservice-registration
{
    "kind": "Service",
    "apiVersion": "v1",
    "metadata": {
        "name": "<SVC-style DNS name>",
        "uid": "<RFC 4122>",
        "labels": {
            "friendlyName": "UPP Service Information",
            "scopes": "foo.bar baz",
            "authority": "<Authority>",
            "type": "<UPP Service Type>",
            "format":"<UPP Service Format>"
        },
        "annotations": {
            "description":"A long description about the service",
            "priority": 1,
            "oAuthId": null,
            "tokenId":null
        }
    },
    "spec": {
        "type": "ExternalName",
        "externalName": "https://my.host.com:port",
        "path":"/path/to/api/root"
    }
}
```

##### Response

Regardless of whether the registration succees or not, a 200 OK response will be sent to the client as a [UPP Microservice Response]((interop/mimetypes/vnd.upp.microservice-response.json) with a list of the current set of service changes made in response to the request.

If a service failed to register for any known reason, it will be listed in the `failed` array.

```json
Content-Type: application/vnd.upp.microservice-response

{
    "added": [ "upp_mja_mn+upp.information.vehicle"],
    "updated": [],
    "removed": [],
    "failed": [ "esri_com+county.boundaries"]
}
```

Unhandled errors will return is a JSON response of

```json
{
    "success": false
}
```

#### Get service token

For secured services, this will acquire a short-lived token on behalf of the authenticated user. A list of scopes must be passed as a claim on the current user. Each registered service takes a set of scopes that it recognizes as valid and will only issue a token to users with the appropriate claims.

If a user has a `upp_admin` role claim, they are allowed to acquire a token via a GET request.

```http
GET /services/:name/token (Admin Only)
POST /service/:name/token
```

The response is returned as a [UPP Token Response](interop/mimetypes/vnd.upp.token-response.json).

##### Within Scopes

None

##### Required Claims

None

##### Parameters

None

##### Response

```json
Content-Type: application/vnd.upp.token-response

{
  "name": "esri.routing",
  "url": "https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World",
  "token": "sY9VnvLH8MX9clpyMMecwC8uHT6Oa2FFurNG06ozoXPIiZ1M8OYlV5oZ02FIZAmDZNNYxstewQLjzjFg8qHzTHSUHkIgbTGUJjVNV2Uv8Sf322VZzCTdIa6jGh-wkJ2yuECjaNP8p-Q3lZxiwT5Csg..",
  "is_secured": true
}
```

### Permit API

The permit API defines an interface for requesting, issuing and inspecting OSOW permits across a federated collection of conforming systems.

#### Concepts

The federated sharing of permits across organizational boundaries is build on top of the populat git distributed repository ptotocol. All permits are instantiated as local repositories on the originating system, and URLs to the source repository are passed as part of the permit record types.

Remote systems are required to pull, commit and push changes to the repository as local actions are taken.  The originating repository is defined as the System of Record for a given permit.  This is an important design aspect of the UPP system -- there is no _single_ System of Record, rather each system if the authroity over the record that it was responsible for creating.

The permit repository MUST conform the the following structure

```text
/
|- permit.json
|- attachments 
```

Any system may place additional files in the repository as long as they are placed within a dotted-folder with the name of the authority that created it.  For example, this is a valid UPP repoitory structure

```text
/
|- .duluth_cty_mn
|   |- file.txt
|   +- image.png
|
|- permit.json
+- attachments 
```

However, adding additional, non-dotted files to the top level of the respository is INVALID.

#### Permit Document Structure

The `permit.json` document has the following structure with MIME Type [application/vnd.upp.permit](interop/mimetypes/vnd.upp.permit.json)

```json
{
  "meta": {
    "upp": {
      "version": "1.0.0"
    }
  },
  "links": {
    "origin": "http://localhost:8000/git/ffcf7842-4857-44ab-a08f-f51bef31da85"
  },
  "data": {
    "type": "permit-application",
    "id": "ffcf7842-4857-44ab-a08f-f51bef31da85",
    "meta": {},
    "attributes": {
      "form-data": {},
      "route": {},
      "authorities": {}
    },
    "relationships":{
      "bridges":[],
      "weather":{}
    }
  }
}
```

| Name | Description |
| - | -
| `meta.upp.version` | MUST be the string "1.0.0"
| `links.origin` | An accessible URL that other UPP systems may use to clone the permit repository.  Each system SHOULD apply appropriate `claims` checks to any repository request to ensure that the remote system is using an identity with the appropriate permissions.
| `data.type` | MUST be the string "permit-application"
| `data.id`   | A unique string that identifies this permit among all UPP permits.  It is RECOMMENDED that an appropriate UUID be used.
| `data.meta` | Metadata about the permit application.
| `data.attributes.form-data` | A collection of properties the represent the core permit information as determined by the UPP Standards Committee.  This data may be modified over time as a permit is revised.  The UPP system relies on the git history for auditing and inspectibility purposes.
| `data.attributes.route` | The defined route that will be taken by the application. This data MUST be in the format returned by an Esri-complatible Routing Service and include all _stops_ and _barriers_.
| `data.attributes.authorities` | This property provides a place for each authority to store local information with the permit.  The authority properties are filled in at the time of permit creation by identifying all of the jurisdictions that a proposed route passes through.  An authority MUST NOT add new properties to this object.<br><br>If their authority's name does not exists, it is an error condition.  An authority MAY create a dotted folder within the repository as a fall-back to communicate information that must be resolved later.<br><br>The reason for this requirement is to enforce a structure within the JSON file such that merge conflicts are structurally impossible and the UPP system can rely on merge operations succeeding in all, non-error cases.
| `relationships` | This object contains a list of read-only propreties that hold contextual information about the permit.  Each relationship items is defined is its own specification addendum and all relationships are OPTIONAL.

### Required Scopes

A system implementing the Permit API MUST recognize the following scopes

* `permit:request`
* `permit:review`
* `permit:enforcement`

### Resource Endpoints

#### GET `{base}/info`

Returns a [Permit Issuer Metadata](#Permit-Issuer-Metadata) record to the client.  This endpoint MAY be secured but is not REQUIRED to be.

##### Within Scopes

* None

##### Required Claims

* None

#### GET `{base}/permits`

Return a list of [Permit Records](interop/mimetypes/vnd.upp.permit.json) filtered by the identified user. This endpoint MUST be secured and MUST filter permits by recognizing the following role claims

* The `hauler` claim SHOULD allow the user to retrieve all of permits that have been issues to this user.  The user SHALL be identified by the contents of an `email` claim, an IdP token from the `tokens` claim, or a combination of the two. The implementing system DOES NOT need to return all permits in a single transaction, but MAY provide pagination via a `_links` JSON subrecord that follows the [HAL](http://stateless.co/hal_specification.html) specification.

* The `permit:enforcement` scope grant MAY grant the user the ability to view ALL permits issued by the permit authority.

##### Within Scopes

* `permit:request`
* `permit:review`
* `permit:enforcement`

##### Required Claims

* A `hauler` or `dispatcher` MUST be limited to finding ONLY permits that they have submitted.

##### Response

TODO

#### POST `{base}/permits`

The primary endpoint that UPP clients should use to request a permit from the authority.  This endpoint MUST be secured and validate that the request comes from a trusted UPP system. The user making the request MUST have the `hauler` or `dispatcher` role claim.

##### Within Scopes

* `permit:request`

##### Required Claims

* None

##### Response

The response will be returned as a JSON API resource identifier document of type [Permit Application](interop/mimetypes/vnd.upp.permit.json) with an assigned GUID. A `links` section is defined that contains the URL of the new permit application and a git origin URL that other UPP systems can use to create a local checkout of the permit application files.

```json
{
  "data": {
    "type": "permit-application",
    "id": "c916b8cc-a464-418c-bcbe-0876e8b86701",
    "links": {
      "self": "/upp/api/permits/c916b8cc-a464-418c-bcbe-0876e8b86701",
      "origin": "https://upp.example.com/git/c916b8cc-a464-418c-bcbe-0876e8b86701"
    }
  }
}
```

#### GET `{base}/permits/{guid}`

Retrive a specific permit application from a UPP authority. Only permits that originated from a given host need to be accessible via this endpoint.

##### Within Scopes

* `permit:request`
* `permit:review`
* `permit:enforcement`

##### Required Claims

* A `hauler` or `dispatcher` MUST be limited to finding ONLY permits that they have submitted. A 404 response MUST be returned otherwise.

##### Response

The response is a full[Permit Application](interop/mimetypes/vnd.upp.permit.json) JSON API record.

```json
{
  "meta": {
    "upp": {
      "version": "1.0.0"
    }
  },
  "links": {
    "origin": "http://localhost:8000/git/c454c9cd-e754-4956-aa66-ec9f9278852c"
  },
  "data": {
    "type": "permit-application",
    "id": "c454c9cd-e754-4956-aa66-ec9f9278852c",
    "meta": {},
    "attributes": {
      "form-data": {},
      "route": {},
      "authorities": {}
    },
    "relationships":{
      "bridges":[],
      "weather":{}
    }
  }
}
```

#### POST `{base}/permits/{guid}/patch`

The `patch` endpoint allows sections of the permit application record to be updated by UPP clients.  Clients MUST use this endpoint to modify the permit document and not modify the document directly.

##### Within Scopes

* `permit:request`
* `permit:review`

##### Required Claims

* A `hauler` or `dispatcher` MUST be limited to updating ONLY permits that they have submitted.  Further, they MUST only be allowed to change the `form-data` and `route` sections of the [Permit Request](#Permit-Request). A 404 response MUST be returned otherwise.

##### Parameters

| Name | Type | Description |
| - | - | -
| `section` | `string` | Designated the section of the permit application that will be updated from the POST body.  Allowed values are `form-data`, `bridges`, `route`, `authorities`, `authority`
| `authority` | `string` | Used in conjuction with the `section` value of `authority`.  Identifies the specific authority's section to be updated.  The request must come from the authority named in the parameter.

The distinction between the `authorities` section and `authority` is that the former will set the list of all valid authorities with an interest in this permit application, while the latter allowed an authority to update its own section of the permit application.

##### Response

The endpoint will return an empty `200 OK` on success and a `400 Bad Request` if any of the passed parameters are invalid, or if the POST body is inappropriate for the sepcified section.

#### POST `{base}/permits/{guid}/package`

##### Within Scopes

##### Required Claims

##### Parameters

##### Response

The endpoint MUST accept a [Permit Request](#Permit-Request) document and MAY return any of the following response codes to the client.

##### 201 Created

This response code is used when the permit application is _automatically_ accepted and a permit issued by the specified authority. An [Approved Permit](#Approved-Permit) record is returned to the client which includes the URL of the authority's record of approval.  This URL may be used in the future in order to submit updated information or withdraw the permit entirely.

##### 202 Accepted

This response code is used when the permit application is required to undergo an approval process and a decision of whether to grant or deny the permit will be made at a later time.  A [Pending Permit](#Pending-Permit) record is returned to the client which includes the URL of the transaction which allows the progress of the process to be monitored.  The Pending Permit response MAY include information about the expected service times, availability and alternate submission information.

Returns a [Permit Request Response](#Permit-Request-Response) record to the client. This endpoint MUST be secured and validate that the request comes from a trusted UPP system. The user making the request MUST have the `hauler` claim.

The `receipt` field is an opaque string generated by the permitting system.  The field SHOULD be less than 200 characters in length and is recommended that a [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) or similar data type be used. The permit issuer MUST be able to retrive a permit record if it is provided with a valid receipt in the future.

#### GET `{base}/permits/{guid}/attachments`

##### Within Scopes

None

##### Required Claims

None

##### Parameters

None

##### Response

TBD

#### POST `{base}/permits/{guid}/attachments`

##### Within Scopes

None

##### Required Claims

None

##### Parameters

None

##### Response

TBD

#### GET `{base}/permits/{guid}/attachments/{name}`

##### Within Scopes

None

##### Required Claims

None

##### Parameters

None

##### Response

TBD

#### GET `{base}/reviews/{receipt}`

Returns the status of a permit that is under review.

##### 303 See Other

Once the review is complete and the permit approved, attempting to access this endpoint SHOULD result in the client be redirected to the permit record.

#### GET `{base}/permits/{receipt}`

Return a single [Permit Record](#Permit-Record) identified by the `receipt`. This endpoint MUST be secured and the permit authority MUST verify that the passed identity has access to the permit associated with the receipt.

#### PUT `{base}/permits/{receipt}`

Allows a user to submit updates to an existing permit that has already been issued.

## Provisions

#### GET `{base}/provisions/catalog`

Returns a collection of all provisions that the UPP authority may apply.

#### POST `{base}/provisions/apply`

Apply the provisions to the permit record.
