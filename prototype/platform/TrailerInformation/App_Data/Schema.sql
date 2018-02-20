-- Fake data for stubbing remote services
create table Users
(
	user_email                         varchar(100) not null,  -- Identifies the user
	trailer_id						   integer primary key     -- FK to CompanyInformation table
);

create table TrailerInformation
(
	trailer_id							integer primary key,
	make								varchar(100) not null,
	model								varchar(100) not null,
	trailer_type						varchar(100) not null,
	serial_number						varchar(100) not null,
	license_number						varchar(100) not null,
	trailer_state						varchar(10) not null,
	trailer_description					varchar(200) not null,
	empty_weight						integer not null,
	registered_weight					integer not null,
	regulation_weight					integer not null
);
