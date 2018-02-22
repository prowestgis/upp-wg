-- Table of GUID mappings from external authentication providers to
-- local information.  Any third-party system will use a similar scheme
-- to map UPP authorized users to existing resources.
create table Users
(
	user_id                            blob not null           -- GUID under control of UPP
);

-- Table of external id tokens used to track multiple identities associated with 
-- each UPP user
create table ExternalLogins
(
    user_id                            blob not null,          -- FK to the Users table
	provider_id                        varchar(100) not null,  -- OAuth2 provider FK
	provider_user_id                   varchar(100) not null   -- Identity of the user on the external system
);

