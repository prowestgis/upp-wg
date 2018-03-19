# Permit Request

This section defines the properties used in the general Permit Request data payload that is passed to the various Authoritative Systems for approval.

## Hauler Information

* **Applicant Name**

  The name of the applicant requesting an OSOW permit. This can be the name of an individual, or company representative.

  | | |
  | -- | --
  | Type | utf8-encoded string
  | Source | Set by the Identity Provider. An applicant MAY have multiple names 
  | Mutability | Cannot be changed
  
* **Application Date**

  The name of the applicant requesting an OSOW permit. This can be the name of an individual, or company representative.

  | | |
  | -- | --
  | Type | 64-bit signed integer representing UNIX time in milliseconds
  | Source | Entered by the applicant. May be entered in any user-friendly manner, including local time, but MUST be persisted as a UNIX timestamp. A default Application Date MAY be provided by a User-Agent, but MUST be editable by the applicant
  | Mutability | Set once by the applicant per application; readonly afterward

* **Application Email**

  The email address of the applicant.

  | | |
  | -- | --
  | Type | A utf8-encoded email address as defined by the _add-spec_ in [Section 3.4 of RFC 5322](https://tools.ietf.org/html/rfc5322#section-3.4)
  | Source | Set by the Identity Provider. An applicant MAY have multiple email addresses
  | Mutability | Cannot be changed

* **Application Phone**

  The phone number of the applicant.

  | | |
  | -- | --
  | Type | A utf8-encoded telephone number as defined by the _telephone-subscriber_ in [Section 5 of RFC 3966](https://tools.ietf.org/html/rfc3966#section-5).
  | Source | Set by the Identity Provider. An applicant MAY have multiple telephone numbers
  | Mutability | Cannot be changed

* **Application Fax**

  The FAX number of the applicant.

  | | |
  | -- | --
  | Type | A utf8-encoded telephone number as defined by the _telephone-subscriber_ in [Section 5 of RFC 3966](https://tools.ietf.org/html/rfc3966#section-5)
  | Source | Set by the Identity Provider. An applicant MAY have multiple FAX numbers.
  | Mutability | Can be modified or set by the applicant

## Company Information

The company information if provided by a microservice that implements the `information.company` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Company Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.company` API request.

See _TBD_ on the process of resolving inconsistent information returned by multiple company information microservice providers.

* **Company Name**

  The legal name of the company. This name SHOULD match the name as it appears of any relevant business registrations that the company holds. An OPTIONAL dba (Doing Business As) property exists on the Company Information record that allows DBA aliases to be associated with a primary record.
  
  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider.
  | Mutability | Cannot be modified or set by the applicant

* **Company Address**

  The registered address of the company. This name SHOULD match the address as it appears of any relevant business registrations that the company holds.
  
  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified or set by the applicant

* **Company Email**

  A primary email contact for the company that has knowlege and oversight of the permit applications requested by the Hauler.
  
  | | |
  | -- | --
  | Type | A utf8-encoded email address as defined by the _add-spec_ in [Section 3.4 of RFC 5322](https://tools.ietf.org/html/rfc5322#section-3.4)
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified or set by the applicant
  
* **Company Phone**

  The phone number of the company.

  | | |
  | -- | --
  | Type | A utf8-encoded telephone number as defined by the _telephone-subscriber_ in [Section 5 of RFC 3966](https://tools.ietf.org/html/rfc3966#section-5)
  | Source | Set by the microservice provider
  | Mutability | Cannot be changed

* **Company Fax**

  The FAX number of the applicant.

  | | |
  | -- | --
  | Type | A utf8-encoded telephone number as defined by the _telephone-subscriber_ in [Section 5 of RFC 3966](https://tools.ietf.org/html/rfc3966#section-5)
  | Source | MAY be set by the microservice provider
  | Mutability | Can be modified or set by the applicant

## Insurance Information

The insurance information if provided by a microservice that implements the `information.insurance` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Company Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.insurance` API request.

See _TBD_ on the process of resolving inconsistent information returned by multiple insurance information microservice providers.

* **Insurance Provider**

  Name of the insurer.
  
  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider.
  | Mutability | Cannot be modified or set by the applicant

* **Insurance Agency Address**

  The address of the agent for the insurer.
  
  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified or set by the applicant

* **Insurance Policy Number**

  The policy number of the insurance contract help with the insurer.
    
  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified or set by the applicant

# Permit Response