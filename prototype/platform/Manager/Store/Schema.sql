create table OAuth2Providers
(                         
    provider_id                        varchar(100) not null,
	display_name                       varchar(100) not null,
    client_key                         varchar(256) not null,
    client_secret                      varchar(256) not null,
	scopes                             varchar(256),
    active                             integer not null
);

create table MicroServiceProviders
(
    provider_id                        varchar(100) not null,
	oauth_provider_id                  varchar(100),
	display_name                       varchar(100) not null,
	uri                                varchar(1024) not null,
	service_type                       varchar(100) not null,
	service_priority                   integer not null, 
    active                             integer not null
);

-- Table of GUID mappings from external authentication providers to
-- local information.  Any third-party system will use a similar scheme
-- to map UPP authorized users to existing resources.
create table Users
(
	user_id                            blob not null, -- GUID under control of UPP
);

-- Table of external id tokens used to track multiple identities associated with 
-- each UPP user
create table ExternalLogins
(
    user_id                            blob not null,          -- FK to the Users table
	provider_id                        varchar(100) not null,  -- OAuth2 provider FK
	provider_user_id                   varchar(100) not null   -- Identity of the user on the external system
);

-- Fake data for stubbing remote services
create table CompanyInformation
(
	company_id                         integer primary key,
	company_name                       varchar(100) not null,
	email                              varchar(100) not null,
	contact                            varchar(100) not null,
	phone                              varchar(100) not null,
	fax                                varchar(100) not null,
	cell                               varchar(100) not null,
	bill_to                            varchar(100) not null,
	billing_address                    varchar(100) not null,
);