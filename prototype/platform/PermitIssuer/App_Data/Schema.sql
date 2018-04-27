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

