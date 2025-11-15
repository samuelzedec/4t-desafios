CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE COLLATION IF NOT EXISTS case_insensitive (
    provider = icu,
    locale = 'und-u-ks-level2',
    deterministic = false
);

START TRANSACTION;
CREATE TABLE health_plans (
    id uuid NOT NULL,
    name text NOT NULL,
    ans_registration_code text NOT NULL,
    created_at timestamptz NOT NULL,
    modified_at timestamptz,
    deleted_at timestamptz,
    CONSTRAINT pk_health_plans_id PRIMARY KEY (id)
);

CREATE INDEX ix_ans_registration_code ON health_plans (ans_registration_code);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251113041032_AddHealthPlanEntityTable', '9.0.11');

CREATE TABLE beneficiaries (
    id uuid NOT NULL,
    full_name text NOT NULL,
    cpf text NOT NULL,
    status text NOT NULL,
    birth_date date NOT NULL,
    health_plan_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    modified_at timestamptz,
    deleted_at timestamptz,
    CONSTRAINT pk_beneficiaries_id PRIMARY KEY (id),
    CONSTRAINT fk_beneficiaries_health_plans FOREIGN KEY (health_plan_id) REFERENCES health_plans (id) ON DELETE RESTRICT
);

CREATE INDEX "IX_beneficiaries_health_plan_id" ON beneficiaries (health_plan_id);

CREATE UNIQUE INDEX ix_cpf ON beneficiaries (cpf);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251113042654_AddBeneficiaryEntityTable', '9.0.11');

ALTER TABLE health_plans ALTER COLUMN name TYPE text COLLATE case_insensitive;

ALTER TABLE beneficiaries ALTER COLUMN full_name TYPE text COLLATE case_insensitive;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251114211622_AddCollationInHealthPlanAndBeneficiaryTables', '9.0.11');

COMMIT;

