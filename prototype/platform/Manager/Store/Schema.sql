create table OAuth2Providers
(                         
    provider_id                        varchar(100) not null,
    client_key                         varchar(256) not null,
    client_secret                      varchar(256) not null,
    active                             integer not null
)