-- Fake data for stubbing remote services
create table Users
(
	user_email                         varchar(100) not null,  -- Identifies the user
	vehicle_id						   integer primary key     -- FK to CompanyInformation table
);

create table VehicleInformation
(
	vehicle_id							integer primary key,
	vehicle_year						varchar(10) not null,
	make								varchar(100) not null,
	model								varchar(100) not null,
	vehicle_type						varchar(100) not null,
	license_number						varchar(100) not null,
	vehicle_state						varchar(10) not null,
	serial_number						varchar(100) not null,
	usdot_number						varchar(100) not null,
	empty_weight						integer not null,
	registered_weight					integer not null
);
