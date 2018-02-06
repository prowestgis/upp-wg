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
