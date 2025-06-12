-- =================================================================
-- V10.1: Approval System, Role System, and Subject Join Code Enhancements
-- Date: 2025-06-12
-- Author: Bobsi01
-- =================================================================

-- Add is_approved flag to enrollments (default 1 for all existing records)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'enrollments' AND COLUMN_NAME = 'is_approved'
)
BEGIN
    -- V10.1: Add is_approved to enrollments
    ALTER TABLE enrollments ADD is_approved BIT NOT NULL DEFAULT 1;
    PRINT 'V10.1: Added is_approved column to enrollments table.';
END
GO

-- V10.1: Create enrollment_approval_log to track approval actions
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'enrollment_approval_log')
BEGIN
    CREATE TABLE enrollment_approval_log (
        log_id INT IDENTITY(1,1) PRIMARY KEY,
        enrollment_id INT NOT NULL,
        action VARCHAR(20) NOT NULL, -- 'approved', 'denied', 'revoked'
        performed_by INT NOT NULL,   -- user_id of the approver
        role VARCHAR(30) NOT NULL,   -- role of the approver at action time
        action_timestamp DATETIME DEFAULT GETDATE(),
        remarks NVARCHAR(255) NULL,
        FOREIGN KEY (enrollment_id) REFERENCES enrollments(enrollment_id),
        FOREIGN KEY (performed_by) REFERENCES users(user_id)
    );
    PRINT 'V10.1: Created enrollment_approval_log table for tracking approvals.';
END
GO

-- V10.1: Create roles table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'roles')
BEGIN
    CREATE TABLE roles (
        role_id INT IDENTITY(1,1) PRIMARY KEY,
        role_name VARCHAR(30) UNIQUE NOT NULL
    );
    PRINT 'V10.1: Created roles table.';
    -- Insert default roles
    INSERT INTO roles (role_name) VALUES
        ('student'), ('class officer'), ('faculty'), ('administrator');
    PRINT 'V10.1: Inserted default roles (student, class officer, faculty, administrator).';
END
GO

-- V10.1: Create user_roles mapping table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'user_roles')
BEGIN
    CREATE TABLE user_roles (
        user_role_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        role_id INT NOT NULL,
        assigned_at DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (user_id) REFERENCES users(user_id),
        FOREIGN KEY (role_id) REFERENCES roles(role_id),
        CONSTRAINT UQ_user_role UNIQUE (user_id, role_id)
    );
    PRINT 'V10.1: Created user_roles mapping table.';
END
GO

-- V10.1: Create subject_join_codes table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_join_codes')
BEGIN
    CREATE TABLE subject_join_codes (
        code_id INT IDENTITY(1,1) PRIMARY KEY,
        subject_id INT NOT NULL,
        school_year_id INT NOT NULL,
        join_code CHAR(6) NOT NULL,
        created_at DATETIME DEFAULT GETDATE(),
        is_active BIT DEFAULT 1,
        CONSTRAINT UQ_subject_code_year UNIQUE(subject_id, school_year_id),
        CONSTRAINT UQ_join_code_year UNIQUE(join_code, school_year_id),
        FOREIGN KEY (subject_id) REFERENCES subjects(subject_id),
        FOREIGN KEY (school_year_id) REFERENCES school_years(school_year_id)
    );
    PRINT 'V10.1: Created subject_join_codes table with unique code per subject per school year.';
END
GO

-- V10.1: Create subject_join_code_audit table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_join_code_audit')
BEGIN
    CREATE TABLE subject_join_code_audit (
        audit_id INT IDENTITY(1,1) PRIMARY KEY,
        code_id INT NOT NULL,
        user_id INT NOT NULL,
        joined_at DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (code_id) REFERENCES subject_join_codes(code_id),
        FOREIGN KEY (user_id) REFERENCES users(user_id)
    );
    PRINT 'V10.1: Created subject_join_code_audit table to track join code usage.';
END
GO

-- V10.1: Set all current enrollments as approved (for fresh system)
UPDATE enrollments SET is_approved = 1 WHERE is_approved IS NULL OR is_approved <> 1;
PRINT 'V10.1: All existing/fresh enrollments marked as approved by default.';
GO

-- V10.1: Add version record to schema_version
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '10.1')
BEGIN
    INSERT INTO schema_version (version_number, description)
    VALUES ('10.1', 'Approval system for enrollments, role management system, subject join code, and related audit/logs');
    PRINT 'V10.1: Added schema_version record for V10.1';
END
GO

-- V10.1: Add version record to schema_version_details
IF NOT EXISTS (SELECT 1 FROM schema_version_details WHERE version_number = '10.1')
BEGIN
    INSERT INTO schema_version_details (version_number, feature_description, release_date, changelog)
    VALUES ('10.1', 'Enrollment approval, flexible roles, subject join codes, and audit logs', GETDATE(), 'is_approved on enrollments, approval log, roles, join codes, and audit tracking');
    PRINT 'V10.1: Added schema_version_details record for V10.1';
END
GO

-- ========================== V10.1 CHANGE SUMMARY ==========================
PRINT '======================================================================';
PRINT 'V10.1 SCHEMA UPDATE SUMMARY';
PRINT '1. Added is_approved column to enrollments for subject/class approval workflow.';
PRINT '2. Added enrollment_approval_log for logging who approved/denied enrollments and when.';
PRINT '3. Implemented a roles and user_roles system for flexible user roles (student, class officer, faculty, administrator).';
PRINT '4. Added subject_join_codes table for unique subject join codes per school year.';
PRINT '5. Added subject_join_code_audit for tracking who joined via join codes.';
PRINT '6. Set all existing enrollments to approved by default (fresh data migration).';
PRINT '7. Updated schema_version and schema_version_details for V10.1 tracking.';
PRINT '======================================================================';
GO