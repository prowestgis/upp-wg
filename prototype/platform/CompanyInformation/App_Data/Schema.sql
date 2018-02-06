-- Fake data for stubbing remote services
create table Users
(
	user_email                         varchar(100) not null,  -- Identifies the user
	company_id                         integer primary key     -- FK to CompanyInformation table
);

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
	billing_address                    varchar(100) not null
);
