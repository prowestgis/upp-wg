-- Fake data for stubbing remote services
create table Users
(
	user_email                         varchar(100) not null,  -- Identifies the user
	axle_id							   integer primary key     -- FK to AxleInformation table
);

create table AxleInformation
(
	axle_id								integer primary key,
	axle_description					varchar(100) not null,
	weight_per_axle						decimal not null,
    description_summary					varchar(100) not null,
    axle_count							integer not null,
    group_count							integer not null,
    approx_axle_length					decimal not null,
    axle_length							decimal not null,
    max_axle_width						decimal not null,
    axle_group_summary					varchar(100) not null,
	axles_per_group						integer not null,
    axle_group_tire_type				varchar(100) not null,
    axle_group_width					decimal not null,
    axle_operating_weights				decimal not null,
    axle_group_weight					decimal not null,
    axle_group_max_width				decimal not null,
    axle_group_total_weight				decimal not null,
    axle_group_distance					decimal not null
);
