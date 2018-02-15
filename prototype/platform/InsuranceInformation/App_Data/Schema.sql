-- Fake data for stubbing remote services
create table Users
(
	user_email                         varchar(100) not null,  -- Identifies the user
	insurance_id                       integer primary key     -- FK to CompanyInformation table
);

create table InsuranceInformation
(
	insurance_id                       integer primary key,
	provider_name                      varchar(100) not null,
	agency_address                     varchar(100) not null,
	policy_number                      varchar(100) not null,
	insured_amount                     real not null

);
