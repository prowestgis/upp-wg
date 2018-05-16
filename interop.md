# Interoperability Specification

The Unified Permitting Project (UPP) Interoprability Specificaion is intended to define the behavior and interfaces of systems that wish to participate as
peers within the distributed UPP platform.  This document contains a technical, but not a formal, definition and  description of the the specification's
components.  A separate validation suite is provided in order to test the compliance of implemnting systems agais the specification.

# Overview


# Definitions

The goal of the UPP specification is to define correct communication and coordination among a heterogeneous collection of independedn systems.  As such,
there are many UPP domain-specific concepts that are heavily used through the specification that are defined in this section.

## General Terms

Authority
: A UPP Authority is an independent, trusted entity that provides one or more UPP platform services.

Claim
: A claim is a piece of information that is bound to an identity.  Common claims are first and last names, email address and other contact information.  However a claim can be anything and is oftern used to hold system-specific information.

Microservice
:  A microservice is a unit of software that is deployed as a service and provides an independent and complete business capability.

MIME Type
:  The MIME Type is a standardized way to indicate the nature and format of a document that follows the format of `type/subtype`. Each document in UPP has a unique MIME Type assigned.

OAuth
:  An open standard for token-based authentication and authorization on the Internet.

## Authorities

Within the initial design of UPP, authorities may be cities, counties, or state agencies.  Private companies will not be authorities themselves, but may exposed services that implement specific API scopes on behalf of authorities.

Given the limited number of authories, the UPP system defines an exhaustive list of authorities.  In the case of private parties providing services, their trust must be delegated by one of the defined authorities.

### Representation

**_NOTE TO WORKING GOURP:_ Consider the merits of using X.509 [Standard Attribute Types](https://tools.ietf.org/html/rfc4211#appendix-A.2) (C, L, ST, O, OU, CN) to Identify authorities. May be able to leverage against already-issued SSL certificates to verify.**

An authority is identified by a string representation with the format

   `<name>_<org>_<state>`

Where the `name` is a lowercase version of the legal name of the entity with spaces replaced with hyphens.  The `org` is the organization level of the authority represented as a three-letter code. Finally, the `state` identifies the state the authority operates within.

It is important to note that a single legal entity may have multiple UPP Authority names.  The authority is means to designate _operating_ authority; that is, who has ownership of some UPP action within a designated area.

#### Valid Names

All city, county, township, village, ward or other mulciple corporation names may be used as a valid name. If there are naming conflicts such as multiple cities or townships with the same name, the defined convention is to use a forward slash (`/`) to provide extra information to disambiguate the name.

#### Valid Organizational Designations

| Org | Code |
| - | - |
| City | `cty` |
| County| `cou` |
| Town / Township | `twn` |
| Village | `vlg` |
| Multi-Jurisdictional Authority | `mja` |
| State Agency | `agy`

#### Valid States

Currently, only `mn` is considered a valid state identitfier for UPP Authorities

### Authority Examples

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

## Claims

A defined above, a claim is a piece of information that is attatched to an Identity. UPP uses a [Json Web Token](https://tools.ietf.org/html/rfc7519#section-4) (JWT) for its representation of the current claims for a given identity. In addition to the claims defined in [RFC 7519](https://tools.ietf.org/html/rfc7519), UPP defines the following claims

### Defined Claims

| Claim | Description |
| - | - |
| `upp` | An opaque data token that MUST be preserved across all UPP requests
| `email` | One or more email addresses as defined in [RFC 5322](https://tools.ietf.org/html/rfc5322).  Multiple email addresses are separated by spaces.
| `phone` | One or more telephone URIs. Multiple telephone numbers are separated by spaces. Telephone numbers MUST follow the formatting rules defined in [RFC 3966](https://tools.ietf.org/html/rfc3966).<br>A mobile phone number that has been approved for messaging from UPP MUST include a `mobile` parameter with an optional parameter value that defines the type of content that may be sent to this device. Valid values are `sms` and `mms` with `sms` being the default value.<br>UPP also defined two additional parameters that define the interaction model to be used with a telephone number. `fax` indetifies a phone number as a FAX machine numbr  and may be used as a target. `ivr` designates a phone number that should be used for interations via an Interactive Voice Response system.
| `hauler` | User is a hauler and generally entitled to interact with the systems in pursuit of obtaining a OSOW permit.
| `dispatcher` | User is a dispatcher
| `law.enforcement` | User is a member of law enforcement
| `dps` | Department of Public Safety Claim.  Value MAY be a list of specific roles the users holds within DPS, e.g. 'hwp'
| `scopes` | A list of scopes attached to this identity.

Any UPP system is allowed to define its own claims, however claims can only be set by an Identity Provider.

### Claims Examples

A series of email claims

| Claim | Description
| - | - |
| `email: user@example.com` | A simple email address
| `email: user@example.com user@county.gov` | Two email addresses associated with an identity
| `email: user+tag@example.com` | An email address for `user@example.com` that includes a tag, which is ignored (but preserved) by email servers during transport.
A series of telephone claims

| Claim | Description
| - | - |
| `phone: tel:+18888675309` | A global telephone number for an identity
| `phone: tel:867-5309` | A local telephone number for a user that includes optional (but allowed) visual separators
| `phone: tel:8675309 tel:8005551212` | A pair of phone numbers, one in local format and one in global format, that are associated with the identity
| `phone: tel:+18888675309;mobile` | A global phone number with a mobile parameter.  UPP will use this number to sent SMS messages only
| `phone: tel:+18888675309;mobile=mms;ivr tel:+1-800-555-1212;fax` | A global phone number with a mobile parameter that marks it as accepting MMS messages.  This phone number may also be used to call the user and walk them through an IVR workflow. A second number is provided for FAX documents and contains visual separators

## Scopes

A scope define the specific access that a user needs. Each API MAY require specific scopes to access functional endpoints.

| Scope | Description |
| - | - |
| `permit:request` | Grants read/write access to permit requests, read access to permit reviews, and read access to the Support API.
| `permit:review` | Grants read access to permit requests and permit reviews.
| `permit:enforcement` | Grants unrestricted read access to approved permits.

# APIs

This describes the resource servers that comprise the UPP platform. If a system chooses to implement any of the APIs, all of the API methods MUST be implemented.

## Discovery API

The Discovery API allows systems to:

1. Register and describe themselves,
2. Looked up other services based on function

### List services

List the registered services.

```http
GET /services
```

#### Required scopes

None

#### Parameters

| Name | Type | Description |
| - | - | -
| `type` | `string` | Can be `all` or any of the defined service types. Default: `all`
| `scope` | `string` | Can be `all` or any defined UPP scopes. Default: `all`
| `authority` | `string` | Indicates which authority's services should be returned
| `sort` | `string` | Can be one of `name`, `display_name` or `type`. Default: `name`
| `direction` | `string` | Can be one of `asc` or `desc`. Default: `desc`

#### Response

```json
[
    {
    "name": "esri.routing",
    "authority": "upp",
    "display_name": "Esri Premium Routing Service",
    "oauth_id": "agol",
    "url": "https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World",
    "type": "route",
    "scopes": "utility:route",
    "priority": 1
  },
  {
    "name": "esri.geometry",
    "authority": "upp",
    "display_name": "Esri Geometry Service",
    "oauth_id": "",
    "url": "https://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer",
    "type": "geometry",
    "scopes": "utility:geometry",
    "priority": 1
  },
  {
    "name": "_upp_permit.approval.Clearwater",
    "authority": "clearwater_cou_mn",
    "display_name": "UPP Trusted Service Endpoint",
    "oauth_id": null,
    "url": "/clearwater-permits/api/v1/issue",
    "type": "upp",
    "scopes": "permit.approval.Clearwater",
    "priority": 1
  }
]
```

### Get service token

For secured services, this will acquire a short-lived token on behalf of the authenticated user. A list of scopes must be passed as a claim on the current user. Each registered service takes a set of scopes that it recognizes as valid and will only issue a token to users with the appropriate claims.

If a user has a `upp.admin` claim, they are allowed to acquire a token via a GET request.

```http
GET /services/:name/token (Admin Only)
POST /service/:name/token
```

#### Required scopes

None

#### Parameters

None

#### Response

```json
{
  "name": "esri.routing",
  "url": "https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World",
  "token": "sY9VnvLH8MX9clpyMMecwC8uHT6Oa2FFurNG06ozoXPIiZ1M8OYlV5oZ02FIZAmDZNNYxstewQLjzjFg8qHzTHSUHkIgbTGUJjVNV2Uv8Sf322VZzCTdIa6jGh-wkJ2yuECjaNP8p-Q3lZxiwT5Csg..",
  "isSecured": true
}
```

## Data API

The Data API provides a generic source of supporting data relevant to the UPP workflows.  This is an intentionally generic interface which is typically used to provide a gateway into existing data sources.

### List data sources

List the data sources that the user has permission to access

```http
GET /data/sources
```

#### Required scopes

None

#### Parameters

| Name | Type | Description |
| - | - | -
| `type` | `string` | Can be `all` or any user-defined type. Default: `all`
| `sort` | `string` | Can be one of `name`, `display_name` or `type`. Default: `name`
| `direction` | `string` | Can be one of `asc` or `desc`. Default: `desc`

#### Response

```json
[
    {
        "name": "My-Data-Source",
        "display_name": "My data Source",
        "description": "This contains information about vehicle ownership",
        "url": "https://upp.org/data/sources/My-Data-Source",
        "type": "vehicle-registration"
    },
    {
        "name": "Trailer-Inventory",
        "display_name": "OTR Trucking Trailer Inventory",
        "description": "This contains information about all of the trailers registers to OTR Trucking",
        "url": "https://otr.com/inventory/data/sources/Trailer-Inventory",
        "type": "trailer-info"
    }
]
```

## Permit API



### `permit.issuer.<authority>`

This scope defines the API that a permit authority must implement in order to provide permits through its jurisdiction to UPP clients.

## Interoperability Profiles

Each UPP-compliance application need only implement the profiles that it needs to support.  Each profile represents a distinct service unit that acts as a building block of the peer-based, distributed UPP system.

## Service Directory

The Service Directory is responsible for

1. Allowing UPP services to register and describe themselves,
2. Allowing UPP services to be looked up based on function

_The service directory implementation is influenced by Kobernetes but retricted. In Kubernetes terminology, the UPP services are closer to a Pod_

```JSON
POST /api/v1/agent/register
Content-Type: application/vnd.upp.service
{
    "kind": "Service",
    "apiVersion": "v1",
    "metadata": {
        "name": "",
        "namespace": "",
        "uid": <RFC 4122>,

        "labels": { key: value },
        "annotations": { key: value }
    },
    "spec": {
        "type": "ExternalName",
        "externalName": "hostname",
        "path": "/root"
    }
}
```

```JSON
GET /api/v1/hosts?type=<service_type>&scope=<api_scope>
Content-Type: application/vnd.upp.service-description
{
    "name": <service_name>,
    "displayName": <label intended for display>,
    "oAuthId": "",
    "tokenId": "",
    "uri": "",
    "type": "",
    "scopes": "",
    "priority": 
}
```

```JSON
GET /api/v1/hosts/{name}/access
Content-Type: application/vnd.upp.service-access-record
{
    "name": <service_name>,
    "url": "https://service_url/with/base/path",
    "token": access_token,
    "is_secured": boolean
}
```

## Permit Issuer

A permit issues has the authority to approve or deny OSOW permits.

### Required Scopes

A Permit Issuer MUST implment the following scopes

* `permit.issuer.<authority>` where `<authority>` is the unique UPP-defined authority string.

#### GET `{base}/info`

Returns a [Permit Issuer Metadata](#Permit-Issuer-Metadata) record to the client.  This endpoint MAY be secured but is not REQUIRED to be.

#### GET `{base}/permits`

Return a list of [Permit Records](#Permit-Record) filtered by the identified user. This endpoint MUST be secured and MUST filter permits be recognizing the following user claims

* The `hauler` claim SHOULD allow the user to retrieve all of permits that have been issues to this user.  The user SHALL be identified by the contents of an `email` claim, an IdP token or a combination of the two. The implementing system DOES NOT need to return all permits in a single transaction, but MAY provide pagination via a `_links` JSON subrecord that follows the [HAL](http://stateless.co/hal_specification.html) specification.

* The `dps` claim MAY grant the user the ability to view ALL permits issued by the permit authority.

#### POST `{base}/permits`

The primary endpoint that UPP clients should use to request a permit from the authority.  This endpoint MUST be secured and validate that the request comes from a trusted UPP system. The user making the request MUST have the `hauler` claim.

The endpoint MUST accept a [Permit Request](#Permit-Request) document and MAY return any of the following response codes to the client.

##### 201 Created

This response code is used when the permit application is _automatically_ accepted and a permit issued by the specified authority. An [Approved Permit](#Approved-Permit) record is returned to the client which includes the URL of the authority's record of approval.  This URL may be used in the future in order to submit updated information or withdraw the permit entirely.

##### 202 Accepted

This response code is used when the permit application is required to undergo an approval process and a decision of whether to grant or deny the permit will be made at a later time.  A [Pending Permit](#Pending-Permit) record is returned to the client which includes the URL of the transaction which allows the progress of the process to be monitored.  The Pending Permit response MAY include information about the expected service times, availability and alternate submission information.

Returns a [Permit Request Response](#Permit-Request-Response) record to the client. This endpoint MUST be secured and validate that the request comes from a trusted UPP system. The user making the request MUST have the `hauler` claim.

The `receipt` field is an opaque string generated by the permitting system.  The field SHOULD be less than 200 characters in length and is recommended that a [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) or similar data type be used. The permit issuer MUST be able to retrive a permit record if it is provided with a valid receipt in the future.

#### GET `{base}/reviews/{receipt}`

Returns the status of a permit that is under review.

##### 303 See Other

Once the review is complete and the permit approved, attempting to access this endpoint SHOULD result in the client be redirected to the permit record.

#### GET `{base}/permits/{receipt}`

Return a single [Permit Record](#Permit-Record) identified by the `receipt`. This endpoint MUST be secured and the permit authority MUST verify that the passed identity has access to the permit associated with the receipt.

#### POST `{base}/permits/{receipt}`

Allows a user to submit updates to an existing permit that has already been issued.



## UPP Records

The UPP records define the types that can be received and sent among UPP-compliant systems.  Each record is assigned it's own MIME type and the payload follow [jsonapi](http://jsonapi.org) conventions.

### Permit Issuer Metadata (application/vnd.upp.permit-issuer.metadata)

```text
{
    "type": "upp.metadata",
    "id": <authority-name>,
    "attributes": {
    }
}
```

### Permit Request (application/vnd.upp.permit-request)

### Permit Request Response (application/vnd.upp.permit-issuer.response)

```text
{
    "type": "upp.permit-response",
    "id": <UUID>,
    "attributes": {
        "authority": string,
        "timestamp": int64,
        "status": "approved" | "denied" | "no_authority" | "under_review",
    },
    "links": {
    }
}
