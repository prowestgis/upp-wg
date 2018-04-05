-- Table of GUID mappings from external authentication providers to
-- local information.  Any third-party system will use a similar scheme
-- to map UPP authorized users to existing resources.
create table Users
(
	user_id                            varchar(200) not null, -- GUID under control of UPP; mapped to 'upp' claim in JWT
	user_label                         varchar(200),          -- Something for a human to know who this is....
	extra_claims                       varchar(1000)          -- Space-delimited claims added by UPP manager, e.g. upp.admin, law.enforcement
);

-- Table of external id tokens used to track multiple identities associated with 
-- each UPP user
create table ExternalLogins
(
    user_id                            varchar(200) not null,  -- FK to the Users table
	provider_id                        varchar(100) not null,  -- OAuth2 provider FK
	provider_user_id                   varchar(100) not null   -- Identity of the user on the external system
);

