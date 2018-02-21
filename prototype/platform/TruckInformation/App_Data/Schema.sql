-- Fake data for stubbing remote services
create table Users
(
	user_email                         varchar(100) not null,  -- Identifies the user
	truck_id						   integer primary key     -- FK to CompanyInformation table
);

create table TruckInformation
(
	truck_id							integer primary key,
	gross_weight						integer not null,
	empty_weight						integer not null,
	registered_weight					integer not null,
	regulation_weight					integer not null,
	height								integer not null,
	width								integer not null,
	truck_length						integer not null,
	front_overhang						integer not null,
	rear_overhang						integer not null,
	left_overhang						integer not null,
	right_overhang						integer not null,
	diagram								varchar(200) not null
);
