create table if not exists test_table (
	id serial primary key,
	name varchar(100) not null,
	created_at timestamp default current_timestamp
);