-- Database: "AzureFunctions"

-- DROP DATABASE "AzureFunctions";

CREATE DATABASE "AzureFunctions"
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'English_United States.1252'
       LC_CTYPE = 'English_United States.1252'
       CONNECTION LIMIT = -1;
-- Table: public.leavetbl

-- DROP TABLE public.leavetbl;

CREATE TABLE public.leavetbl
(
  leaveid character varying(50) NOT NULL,
  empployeeid integer,
  employeename character varying(100),
  type integer,
  status integer,
  reason character varying(200),
  wfid character varying(50),
  CONSTRAINT leavetbl_pkey PRIMARY KEY (leaveid)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.leavetbl
  OWNER TO postgres;