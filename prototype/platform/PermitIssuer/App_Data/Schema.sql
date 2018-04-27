create table Permits
(
	permit_id                   varchar(200) not null, -- GUID under control of UPP; mapped to 'upp' claim in JWT
	application_date            varchar(200) not null,
	permit_status				varchar(50) not null,
	hauler_name					varchar(200),
	hauler_email				varchar(200),
	hauler_phone				varchar(20),
	hauler_fax					varchar(20),
	company_name				varchar(200),
	company_address				varchar(200),
	company_email				varchar(200),
	company_contact				varchar(200),
	company_phone				varchar(20),
	company_fax					varchar(20),
	insurance_provider			varchar(200),
	insurance_agency_address	varchar(200),
	insurance_policy_number		varchar(200),

	vehicle_make				varchar(200),
	vehicle_type				varchar(200),
	vehicle_license_number		varchar(200),
	vehicle_state				varchar(200),
	vehicle_truck_serial_number varchar(200),
	vehicle_usdot_number		varchar(200),
	vehicle_empty_weight		integer,

	trailer_make				varchar(200),
	trailer_type				varchar(200),
	trailer_license_number		varchar(200),
	trailer_state				varchar(20),
	trailer_empty_weight		integer,

	total_gross_weight			integer,
	height						decimal,
	width						decimal,
	combined_length				decimal,
	overhang_front				integer,
	overhang_rear				integer,
	overhang_left				integer,
	overhang_right				integer,

	load_description			varchar(1000)

);



