-- Table permit applications
create table Applications
(
    application_id                     integer primary key,   -- ID of this application.  Even under review, this is idempotent
	application_data                   varchar(1024),         -- SQLite does not enforce VARCHAR lengths, so this is unbouded
	application_status                 integer                -- Workflow state
);

-- Table Users
create table Users
(
    user_id                            integer primary key,
	is_active                          integer
);

-- User email addresses
create table Emails
(
    user_id                            integer,
    user_email                         varchar(200) not null -- Email of the person submitting the application
);

-- Join table of users to applications (allow multiple email addresses)
create table UserApplications
(
    user_id                            integer,
	application_id                     integer,

	UNIQUE (user_id, application_id) ON CONFLICT REPLACE
);

-- Table of permit reviews. A permit may undergo multiple reviews and, in fact, multiple reviews can be outstanding at the same time
create table Reviews
(
    review_id                          integer primary key,
	application_id                     integer,                -- FK to Permits table
	review_data                        varchar(1024),
	review_status                      integer
);

-- Save from merge
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
