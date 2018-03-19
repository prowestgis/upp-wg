# Permit Request

This section defines the properties used in the general Permit Request data payload that is passed to the various Authoritative Systems for approval.

_Note_: Some types are listed as _restricted_ utf8-encoded strings.  This is a placeholder designation where it is _assumed_ that the set of legal characters is a well-defined subset of Latin-1 characters but the exact definition has yet to be determined.

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

The company information is provided by a microservice that implements the `information.company` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Company Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.company` API request.

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

The insurance information is provided by a microservice that implements the `information.insurance` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Insurance Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.insurance` API request.

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

  The policy number of the insurance contract held with the insurer.

  | | |
  | -- | --
  | Type | A _restricted_ utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified or set by the applicant

## Vehicle Information

The vehicle information is provided by a microservice that implements the `information.vehicle` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Vehicle Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.vehicle` API request.

* **Vehicle Make**

  The make of the vehicle.

  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

* **Vehicle Type**

  The type of the vehicle. 

  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

* **Vehicle License Number**

  The vehicle license number.

  | | |
  | -- | --
  | Type | A _restricted_ utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

* **Vehicle State**

  The state in which the vehicle is licensed.

  | | |
  | -- | --
  | Type | A two-character United States subdivision code as defined in [ISO 3166-1](https://en.wikipedia.org/wiki/ISO_3166-2:US)
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

* **Vehicle (Truck) Serial Number**

  The serial number of the truck

  | | |
  | -- | --
  | Type | A utf8-encoded alphanumeric string.
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

* **USDOT Number**

  The USDOT number the truck

  | | |
  | -- | --
  | Type | A utf8-encoded alphanumeric string the represents a valid USDOT number.  The number SHOULD be findable via the [Federal Motor Carrier Safety Administration](https://www.fmcsa.dot.gov/) website.
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

* **Vehicle Empty Weight**

  The empty weight of the vehicle in US pounds.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of US pounds
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified

## Overall Truck Information

The truck information is provided by a microservice that implements the `information.truck` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Truck Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.truck` API request.

* **Total Gross Weight**

  The total gross weight of the truck in US pounds.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of US pounds
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Dimension Summary**

  A summary of the overall truck dimensions

  | | |
  | -- | --
  | Type | ???
  | Source | Derived from other properties
  | Mutability | Cannot be modified

* **Overall Dimension Description**

  A description of the overall truck dimensions

  | | |
  | -- | --
  | Type | ???
  | Source | Derived from other properties
  | Mutability | Cannot be modified

* **Height**

  Truck height in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Width**

  Truck width in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Length**

  Truck length in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Front Overhang**

  Truck front overhang in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Rear Overhang**

  Truck rear overhang in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Left Overhang**

  Truck left overhang in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Right Overhang**

  Truck right overhang in inches. The value may be presented to the user and entered in alternative units, e.g. feet+inches, decimal feet, etc., but the value MUST be stored as an integer number of inches.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified
  
* **Truck Diagram**

  A visual representation of the truck dimensions

  | | |
  | -- | --
  | Type | ???
  | Source | Derived from other properties
  | Mutability | Cannot be modified

## Axle Information

The truck information is provided by a microservice that implements the `information.axle` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Axle Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.axle` API request.

* **Weight Per Axle**

  Load weight per axle in US Pounds

  | | |
  | -- | --
  | Type | 32-bit unsigned integer value in US pounds
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Axle Count**

  Number of axles

  | | |
  | -- | --
  | Type | 32-bit unsigned integer value that represents the number of axles
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Axle Length (axle spacing)**

  Axle length in US Inches

  | | |
  | -- | --
  | Type | 32-bit unsigned integer value that represents a number of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Max Axle Width**

  Maximum axle width in US Inches

  | | |
  | -- | --
  | Type | 32-bit unsigned integer value that represents a number of inches
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Max Axle Weight**

  Maximum axle weight in US pounds

  | | |
  | -- | --
  | Type | 32-bit unsigned integer value that represents the maximum axle weight in pounds
  | Source | Derived from other properties
  | Mutability | Cannot be modified

* **Axle Total Weight**

  Total axle weight in US pounds

  | | |
  | -- | --
  | Type | 32-bit unsigned integer value that represents the total axle weight in pounds
  | Source | Derived from other properties
  | Mutability | Cannot be modified

* **Axle Group Tire Type**

  Number of tires per axle

  | | |
  | -- | --
  | Type | ???
  | Source | Entered by the hauler
  | Mutability | Can be modified

## Trailer Information

The trailer information is provided by a microservice that implements the `information.trailer` API scope and is capable of receiving a UPP JSON Web Token (JWT) and responding with a collection of Trailer Information records that are associated with the Identity encoded in the UPP JWT.  Multiple microservices MAY be available and ANY number of them may respond to a UPP `information.trailer` API request.

* **Trailer Make**

  Make of the trailer

  | | |
  | -- | --
  | Type | utf8-encoded string
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Trailer Type**

  Type of the trailer

  | | |
  | -- | --
  | Type | utf8-encoded string
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified; Can by set by the hauler


* **Trailer License Number**

  | | |
  | -- | --
  | Type | ???
  | Source | Entered by the hauler
  | Mutability | Can be modified

* **Trailer State**

  The state in which the trailer is licensed.

  | | |
  | -- | --
  | Type | A two-character United States subdivision code as defined in [ISO 3166-1](https://en.wikipedia.org/wiki/ISO_3166-2:US)
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified; Can be set by the hauler

* **Trailer empty Weight**

  The empty weight of the trailer in US pounds.

  | | |
  | -- | --
  | Type | A 32-bit unsigned integer value in units of US pounds
  | Source | Set by the microservice provider
  | Mutability | Cannot be modified; Can be set by the hauler

## Load Information

The load information is comprosed of a single description field.  As such, it is not backed by any microservice at this time.

* **Load Description**

  Make of the trailer

  | | |
  | -- | --
  | Type | utf8-encoded string
  | Source | Entered by the hauler
  | Mutability | Can be modified

## Movement Information

Details on the movement of the load.

* **Hauling Dates**

  The starting and ending dates for hauling.  

  | | |
  | -- | --
  | Type | 64-bit signed integer representing UNIX time in milliseconds
  | Source | Entered by the applicant. May be entered in any user-friendly manner, including local time, but MUST be persisted as a UNIX timestamp. A default Application Date MAY be provided by a User-Agent, but MUST be editable by the applicant
  | Mutability | Set once by the applicant per application; readonly after approval

* **Hauling Hours**

  Legal hours for hauling the load along the specified route.  

  | | |
  | -- | --
  | Type | ut8-encoded string that represents a collection of valid hourly time range in 24-hour format.  Ranged may be tied to specific dates.<br>The simplest format is simply the string `"8-16"` which represents a time from 8:00am to 4:00pm.<br>Minutes may be specified as well, `"8:30-16:45"`<br>A range of times can be specified like `"8:30-11:30,12:30-17:00"`.<br>Finally, specific times may be bound to specific dates.  Any range without a date qualifier is assumed to apply to all other dates, `"2018/05/14@8:30-11:30,12:30-17:00|2018/05/15@9:00-13:00"`<br>The special range of `"X"` can be used to designate that _no_ travle is allowed on a specific date, `"2018/05/14@8:30-11:30,12:30-17:00|2018/05/15@X"`
  | Source | Returned from the `provisions` microservice.
  | Mutability | Cannot be changed

* **Movement To/From**

  Identifies the physical starting and ending locations to a level of precision that a route can be determined using the `route` microservice.

  | | |
  | -- | --
  | Type | A location suitable for use by the `route` microservice
  | Source | Entered by the applicant. 
  | Mutability | Set once by the applicant per application; readonly after approval

* **Route Description**

  Description of the planned route generated by the `route` microservice.

  | | |
  | -- | --
  | Type | A utf8-encoded string
  | Source | The `route` microservice 
  | Mutability | Cannot by changed

* **Route County Numbers**

  _Definition Needed_

  | | |
  | -- | --
  | Type | 
  | Source | The `route` microservice 
  | Mutability | Cannot by changed

* **Route Miles of County Road**

  Total length of county roads traversed by the proposed route.

  | | |
  | -- | --
  | Type | Positive decimal value with two-points of precision in units of US Miles, e.g. 34.55 miles
  | Source | The `route` microservice 
  | Mutability | Cannot by changed

* **Route Length**

  Total length of the proposed route.

  | | |
  | -- | --
  | Type | Positive decimal value with two-points of precision in units of US Miles, e.g. 34.55 miles
  | Source | The `route` microservice 
  | Mutability | Cannot by changed

* **State Highway Permit Number**

  Permit number of the issues state highway permit.

  | | |
  | -- | --
  | Type | utf8-encoded _restricted_ string.
  | Source | Entered by the hauler
  | Mutability | Set once by the applicant per application; readonly after approval

* **State Highway Permit Number Issues**

  Date that the state highway permit was issued.

  | | |
  | -- | --
  | Type | 64-bit signed integer representing UNIX time in milliseconds
  | Source | Entered by the applicant. May be entered in any user-friendly manner, including local time, but MUST be persisted as a UNIX timestamp. A default Application Date MAY be provided by a User-Agent, but MUST be editable by the applicant
  | Mutability | Set once by the applicant per application; readonly after approval

* **Use of Pilot Care Required**

  Set if a Pilot Car is required by the `provisions` microservice.

  | | |
  | -- | --
  | Type | Boolean value. True if the Pilot Car is required
  | Source | Set by the `provisions` microservice
  | Mutability | Cannot be changed

* **Destination within City Limits**

  Set if the destination is determined to be within a city's limits by the `boundaries` microservice.

  | | |
  | -- | --
  | Type | Boolean value. True if the destination is within a city's limits
  | Source | Set by the `boundaries` microservice
  | Mutability | Cannot be changed

* **Destination within Applying County**

  Set if the destination is determined to be within the applying county by the `boundaries` microservice.

  | | |
  | -- | --
  | Type | Boolean value. True if the destination is within the applying county
  | Source | Set by the `boundaries` microservice
  | Mutability | Cannot be changed
  
# Permit Response