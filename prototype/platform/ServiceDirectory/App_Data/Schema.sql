create table OAuth2Providers
(                         
    provider_id                        varchar(100) not null,
	display_name                       varchar(100) not null,
    client_key                         varchar(256) not null,
    client_secret                      varchar(256) not null,
	scopes                             varchar(256),
    active                             integer not null
);

create table TokenProviders
(
    provider_id                        varchar(100) not null,
	display_name                       varchar(100) not null,
    token_url                          varchar(256) not null,
    token_username                     varchar(256) not null,
    token_password                     varchar(256) not null,
	scopes                             varchar(256),
    active                             integer not null    
);

create table MicroServiceProviders
(
    id                         integer primary key,    
    name                       varchar(100) not null,  -- user-defined name; should be a slug -- no spaces
	type                       varchar(100) not null,  -- type of the service as defines in ServiceRegistrationTypes
	authority                  varchar(100) not null,  -- who is the owner of this service
	format                     varchar(100) not null,  -- what format is this data in? feature_service, json_api
	display_name               varchar(100) not null,  -- friendly name
	description                varchar(1024),
	uri                        varchar(1024) not null,
	priority                   integer not null,
    active                     integer not null,
	scopes                     varchar(256),          -- what scopes does this service implement, if any?
	oauth_provider_id          varchar(100),
	token_provider_id          varchar(100),

	UNIQUE(type, authority) ON CONFLICT REPLACE
);
