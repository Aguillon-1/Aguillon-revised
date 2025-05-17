/*
Classroom Management System - Consolidated Schema
Version: 10.0
Date: 2025-04-10
Description: Comprehensive consolidated schema that combines:
             - V8.2: Base tables with automatic enrollment in default subjects, system configuration,
                    and support for semester-specific default subjects.
             - V9.0: Post system with attachments, comments, reactions, and moderation functionality.
             - V9.1: Calendar system for events and announcements with visibility controls.
Author: Bobsi01
*/

-- Set error handling to continue execution on errors
SET XACT_ABORT OFF;
SET NOCOUNT ON;
GO

PRINT '======================================================================';
PRINT 'CLASSROOM MANAGEMENT SYSTEM - V10 CONSOLIDATED SCHEMA INSTALLATION';
PRINT 'Date: 2025-04-10';
PRINT 'This schema combines V8.2, V9.0, and V9.1 into a single installation script';
PRINT '======================================================================';
GO

-- Create a versioning table if it doesn't exist
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'schema_version')
    BEGIN
        CREATE TABLE schema_version (
            version_id INT IDENTITY(1,1) PRIMARY KEY,
            version_number VARCHAR(10) NOT NULL,
            applied_date DATETIME DEFAULT GETDATE(),
            description VARCHAR(500) NOT NULL
        );
        PRINT 'Created schema_version table.';
    END
    ELSE
        PRINT 'schema_version table already exists.';

    -- Insert version records if they don't exist
    IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '8.2')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('8.2', 'Base tables with default subject enrollment and system configuration capability');
        PRINT 'Added schema version 8.2 record.';
    END

    IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '9.0')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('9.0', 'Added post system with file attachments, commenting functionality, announcement and moderation flags');
        PRINT 'Added schema version 9.0 record.';
    END

    IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '9.1')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('9.1', 'Added calendar and event announcement system');
        PRINT 'Added schema version 9.1 record.';
    END

    IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '10.0')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('10.0', 'Consolidated schema combining V8.2 base tables, V9.0 post system, and V9.1 calendar system');
        PRINT 'Added schema version 10.0 record.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error handling schema_version table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create schema_version_details table for more detailed information
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'schema_version_details')
    BEGIN
        CREATE TABLE schema_version_details (
            detail_id INT IDENTITY(1,1) PRIMARY KEY,
            version_number VARCHAR(10) NOT NULL,
            feature_description VARCHAR(500) NOT NULL,
            release_date DATE NOT NULL,
            changelog TEXT NULL,
            created_at DATETIME DEFAULT GETDATE()
        );
        PRINT 'Created schema_version_details table.';
        
        -- Insert detailed descriptions for each version
        INSERT INTO schema_version_details (version_number, feature_description, release_date, changelog)
        VALUES 
        ('8.2', 'Enhanced schema with automatic enrollment in default subjects, system configuration, and support for semester-specific default subjects', '2025-04-09', 
         'Added default subject enrollment and system configuration capability. Includes fixes for missing columns and tables. Added enrollment history tracking and student promotion functionality.'),
        
        ('9.0', 'Post system with header/body text, file attachments, and commenting functionality', '2025-04-10', 
         'Added tables for posts, post attachments, comments, reactions, and views. Includes announcement capability and moderation system. Added stored procedures for post management.'),
        
        ('9.1', 'Calendar enhancement for announcements and events with visibility controls', '2025-04-10', 
         'Added event scheduling system with calendar integration. Supports targeting events to specific groups. Includes file attachments, reminders, recurrence settings, and RSVP functionality.'),
        
        ('10.0', 'Consolidated schema combining all features from V8.2, V9.0, and V9.1', '2025-04-10', 
         'Complete consolidated version with all tables, constraints, and stored procedures. Includes base system, post system, and calendar system in one comprehensive schema.');
        
        PRINT 'Added schema version details.';
    END
    ELSE
    BEGIN
        -- Check if V10.0 details exist and add if missing
        IF NOT EXISTS (SELECT 1 FROM schema_version_details WHERE version_number = '10.0')
        BEGIN
            INSERT INTO schema_version_details (version_number, feature_description, release_date, changelog)
            VALUES ('10.0', 'Consolidated schema combining all features from V8.2, V9.0, and V9.1', '2025-04-10', 
                   'Complete consolidated version with all tables, constraints, and stored procedures. Includes base system, post system, and calendar system in one comprehensive schema.');
            PRINT 'Added V10.0 version details.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error handling schema_version_details table: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== BASE SYSTEM TABLES (FROM V8.2) ===========

-- Check for users table (required for foreign keys)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
    BEGIN
        PRINT 'Creating users table...';
        CREATE TABLE users (
            user_id INT IDENTITY(1,1) PRIMARY KEY,
            username VARCHAR(50) NULL,
            password_hash VARCHAR(255) NOT NULL,
            email VARCHAR(100) UNIQUE NOT NULL,
            first_name VARCHAR(50) NOT NULL,
            middle_name VARCHAR(50) NULL,
            last_name VARCHAR(50) NOT NULL,
            user_type VARCHAR(20) NOT NULL,
            birthday DATE NULL,
            sex VARCHAR(10) NULL,
            address VARCHAR(500) NULL,
            contact_number VARCHAR(20) NULL,
            enrollment_date DATE NULL,
            school_year_id INT NULL,
            is_archived BIT DEFAULT 0,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        PRINT 'Created users table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking users table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for school_years table (required for foreign keys)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
    BEGIN
        PRINT 'Creating school_years table...';
        
        CREATE TABLE school_years (
            school_year_id INT IDENTITY(1,1) PRIMARY KEY,
            year_name VARCHAR(20) NOT NULL,
            is_current BIT DEFAULT 0,
            start_date DATE NULL,
            end_date DATE NULL,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert at least one school year
        INSERT INTO school_years (year_name, is_current, start_date, end_date) 
        VALUES ('2025-2026', 1, '2025-06-01', '2026-05-31');
        
        PRINT 'Created school_years table with default data.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking school_years table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for semesters table (required for foreign keys)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'semesters')
    BEGIN
        PRINT 'Creating semesters table...';
        
        CREATE TABLE semesters (
            semester_id INT IDENTITY(1,1) PRIMARY KEY,
            semester_name VARCHAR(50) NOT NULL,
            semester_code VARCHAR(5) NOT NULL,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert default semesters
        INSERT INTO semesters (semester_name, semester_code) VALUES 
        ('First Semester', '1ST'), 
        ('Second Semester', '2ND'), 
        ('Summer', 'SUM');
        
        PRINT 'Created semesters table with default data.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking semesters table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for year_levels table (required for foreign keys)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'year_levels')
    BEGIN
        PRINT 'Creating year_levels table...';
        
        CREATE TABLE year_levels (
            year_level_id INT IDENTITY(1,1) PRIMARY KEY,
            year_name VARCHAR(20) NOT NULL,
            description VARCHAR(500) NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert default year levels
        INSERT INTO year_levels (year_name, is_active) VALUES 
        ('Year 1', 1), ('Year 2', 1), ('Year 3', 1), ('Year 4', 1), ('Year 5', 1);
        
        PRINT 'Created year_levels table with default data.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking year_levels table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for programs table (required for foreign keys)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'programs')
    BEGIN
        PRINT 'Creating programs table...';
        
        CREATE TABLE programs (
            program_id INT IDENTITY(1,1) PRIMARY KEY,
            program_name VARCHAR(100) NOT NULL,
            program_code VARCHAR(20) NULL,
            description VARCHAR(500) NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert default programs
        INSERT INTO programs (program_name, program_code, description, is_active) VALUES 
        ('BS Computer Science', 'BSCS', 'Bachelor of Science in Computer Science', 1),
        ('BS Information Technology', 'BSIT', 'Bachelor of Science in Information Technology', 1);
        
        PRINT 'Created programs table with default data.';
    END
    ELSE
    BEGIN
        -- Add missing columns if they don't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'program_code')
        BEGIN
            ALTER TABLE programs ADD program_code VARCHAR(20) NULL;
            PRINT 'Added program_code column to programs table.';
            
            -- Update existing programs with default code values
            UPDATE programs SET program_code = 
                CASE 
                    WHEN program_name LIKE '%Computer Science%' THEN 'BSCS'
                    WHEN program_name LIKE '%Information Technology%' THEN 'BSIT'
                    WHEN program_name LIKE '%Information System%' THEN 'BSIS'
                    ELSE LEFT(REPLACE(program_name, ' ', ''), 4)
                END
            WHERE program_code IS NULL;
        END
            
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'description')
        BEGIN
            ALTER TABLE programs ADD description VARCHAR(500) NULL;
            PRINT 'Added description column to programs table.';
        END
            
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE programs ADD is_active BIT DEFAULT 1;
            PRINT 'Added is_active column to programs table.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error updating programs table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for sections table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'sections')
    BEGIN
        PRINT 'Creating sections table...';
        
        CREATE TABLE sections (
            section_id INT IDENTITY(1,1) PRIMARY KEY,
            section_name VARCHAR(20) NOT NULL,
            year_level_id INT NOT NULL,
            program_id INT NOT NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (year_level_id) REFERENCES year_levels(year_level_id),
            FOREIGN KEY (program_id) REFERENCES programs(program_id)
        );

        -- Insert default sections
        IF OBJECT_ID('year_levels', 'U') IS NOT NULL AND OBJECT_ID('programs', 'U') IS NOT NULL
        BEGIN
            INSERT INTO sections (section_name, year_level_id, program_id)
            SELECT 
                s.section_name,
                yl.year_level_id,
                p.program_id
            FROM 
                (VALUES ('A'), ('B'), ('C')) AS s(section_name)
            CROSS JOIN year_levels yl
            CROSS JOIN programs p
            WHERE yl.year_level_id <= 4  -- Only for years 1-4
            AND p.program_id <= 2;       -- Only for first two programs
        END
        
        PRINT 'Created sections table with default data.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking sections table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for subjects table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
    BEGIN
        PRINT 'Creating subjects table...';
        
        CREATE TABLE subjects (
            subject_id INT IDENTITY(1,1) PRIMARY KEY,
            subject_code VARCHAR(20) UNIQUE NOT NULL,
            subject_name VARCHAR(100) NOT NULL,
            description VARCHAR(500) NULL,
            units INT NOT NULL DEFAULT 3,
            lecture_units INT NOT NULL DEFAULT 3,
            lab_units INT NOT NULL DEFAULT 0,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert sample subjects
        INSERT INTO subjects (subject_code, subject_name, units, lecture_units, lab_units, is_active) VALUES 
        ('CS101', 'Introduction to Computer Science', 3, 3, 0, 1),
        ('CS102', 'Programming Fundamentals', 3, 2, 1, 1);
        
        PRINT 'Created subjects table with sample data.';
    END
    ELSE
    BEGIN
        -- Add missing columns if they don't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'units')
        BEGIN
            ALTER TABLE subjects ADD units INT DEFAULT 3;
            PRINT 'Added units column to subjects table.';
        END
            
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lecture_units')
        BEGIN
            ALTER TABLE subjects ADD lecture_units INT DEFAULT 3;
            PRINT 'Added lecture_units column to subjects table.';
        END
            
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lab_units')
        BEGIN
            ALTER TABLE subjects ADD lab_units INT DEFAULT 0;
            PRINT 'Added lab_units column to subjects table.';
        END
            
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE subjects ADD is_active BIT DEFAULT 1;
            PRINT 'Added is_active column to subjects table.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error updating subjects table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for student_profiles table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'student_profiles')
    BEGIN
        PRINT 'Creating student_profiles table...';
        
        CREATE TABLE student_profiles (
            user_id INT PRIMARY KEY,
            student_id VARCHAR(20) UNIQUE NOT NULL,
            program_id INT NOT NULL,
            year_level_id INT NOT NULL,
            section_id INT NULL,
            school_year_id INT NULL,
            enrollment_date DATE DEFAULT GETDATE(),
            student_status VARCHAR(20) DEFAULT 'regular',
            academic_status VARCHAR(20) DEFAULT 'active',
            admission_date DATE DEFAULT GETDATE(),
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            FOREIGN KEY (program_id) REFERENCES programs (program_id),
            FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
            FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id)
        );
        
        -- Add section foreign key if sections table exists
        IF OBJECT_ID('sections', 'U') IS NOT NULL
            ALTER TABLE student_profiles ADD CONSTRAINT FK_student_profiles_section
            FOREIGN KEY (section_id) REFERENCES sections(section_id);
        
        PRINT 'Created student_profiles table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking student_profiles table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for faculty_profiles table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'faculty_profiles')
    BEGIN
        PRINT 'Creating faculty_profiles table...';
        
        CREATE TABLE faculty_profiles (
            user_id INT PRIMARY KEY,
            faculty_id VARCHAR(20) UNIQUE NOT NULL,
            department VARCHAR(100) NOT NULL,
            position VARCHAR(100) NULL,
            hire_date DATE NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (user_id) REFERENCES users (user_id)
        );
        
        -- Insert default faculty departments for existing faculty users
        INSERT INTO faculty_profiles (user_id, faculty_id, department)
        SELECT 
            u.user_id, 
            'F' + RIGHT('00000' + CAST(u.user_id AS VARCHAR), 5),
            'Computer Science'
        FROM users u 
        WHERE u.user_type = 'Faculty' AND NOT EXISTS (
            SELECT 1 FROM faculty_profiles fp WHERE fp.user_id = u.user_id
        );
        
        PRINT 'Created faculty_profiles table with default data.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking faculty_profiles table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for curriculum table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'curriculum')
    BEGIN
        PRINT 'Creating curriculum table...';
        
        CREATE TABLE curriculum (
            curriculum_id INT IDENTITY(1,1) PRIMARY KEY,
            program_id INT NOT NULL,
            subject_id INT NOT NULL,
            school_year_id INT NOT NULL,
            semester_id INT NOT NULL,
            year_level_id INT NOT NULL,
            curriculum_year VARCHAR(9) NOT NULL,
            subject_status VARCHAR(20) DEFAULT 'active',
            faculty_id INT NULL,
            is_active BIT DEFAULT 1,
            is_default BIT DEFAULT 0, -- Added for default subject enrollment
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (program_id) REFERENCES programs (program_id),
            FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
            FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id),
            FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
            FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id)
        );
        
        -- Add faculty_id foreign key if users table exists
        IF OBJECT_ID('users', 'U') IS NOT NULL
        BEGIN
            ALTER TABLE curriculum ADD CONSTRAINT FK_curriculum_faculty 
            FOREIGN KEY (faculty_id) REFERENCES users (user_id);
        END
        
        PRINT 'Created curriculum table.';
    END
    ELSE
    BEGIN
        -- Check if is_default column exists in curriculum table
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'is_default')
        BEGIN
            ALTER TABLE curriculum ADD is_default BIT DEFAULT 0;
            PRINT 'Added is_default column to curriculum table.';
        END
        
        -- Check if is_active column exists in curriculum table
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE curriculum ADD is_active BIT DEFAULT 1;
            PRINT 'Added is_active column to curriculum table.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error handling curriculum table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for subject_offerings table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_offerings')
    BEGIN
        PRINT 'Creating subject_offerings table...';
        
        CREATE TABLE subject_offerings (
            offering_id INT IDENTITY(1,1) PRIMARY KEY,
            subject_id INT NOT NULL,
            class_code VARCHAR(20) NOT NULL,
            school_year_id INT NOT NULL,
            semester_id INT NOT NULL,
            year_level_id INT NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
            FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id),
            FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
            FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id)
        );
        
        PRINT 'Created subject_offerings table.';
    END
    ELSE
    BEGIN
        -- Add is_active column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subject_offerings' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE subject_offerings ADD is_active BIT DEFAULT 1;
            PRINT 'Added is_active column to subject_offerings table.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking subject_offerings table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for enrollments table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'enrollments')
    BEGIN
        PRINT 'Creating enrollments table...';
        
        CREATE TABLE enrollments (
            enrollment_id INT IDENTITY(1,1) PRIMARY KEY,
            student_id INT NOT NULL,
            offering_id INT NOT NULL,
            enrollment_date DATE DEFAULT GETDATE(),
            grade VARCHAR(5) NULL,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (student_id) REFERENCES users (user_id),
            FOREIGN KEY (offering_id) REFERENCES subject_offerings (offering_id),
            CONSTRAINT UK_enrollments UNIQUE (student_id, offering_id)
        );
        
        PRINT 'Created enrollments table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error checking enrollments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create system_configuration table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'system_configuration')
    BEGIN
        PRINT 'Creating system_configuration table...';
        
        CREATE TABLE system_configuration (
            config_id INT IDENTITY(1,1) PRIMARY KEY,
            current_school_year_id INT NOT NULL,
            current_semester_id INT NOT NULL,
            registration_open BIT DEFAULT 1,
            enrollment_open BIT DEFAULT 1,
            auto_enroll_enabled BIT DEFAULT 1,
            auto_year_level_promotion BIT DEFAULT 0,
            last_updated DATETIME DEFAULT GETDATE(),
            updated_by INT NULL,
            CONSTRAINT FK_sysconfig_school_year FOREIGN KEY (current_school_year_id) 
                REFERENCES school_years (school_year_id),
            CONSTRAINT FK_sysconfig_semester FOREIGN KEY (current_semester_id) 
                REFERENCES semesters (semester_id)
        );
        
        -- Insert default system configuration values
        DECLARE @current_school_year_id INT;
        DECLARE @first_semester_id INT;
        
        -- Find the current school year
        SELECT @current_school_year_id = school_year_id FROM school_years WHERE is_current = 1;
        IF @current_school_year_id IS NULL
            SELECT @current_school_year_id = MAX(school_year_id) FROM school_years;
        
        -- Find the first semester
        SELECT @first_semester_id = MIN(semester_id) FROM semesters WHERE semester_code = '1ST';
        IF @first_semester_id IS NULL
            SELECT @first_semester_id = MIN(semester_id) FROM semesters;
            
        INSERT INTO system_configuration 
        (current_school_year_id, current_semester_id, registration_open, enrollment_open, auto_enroll_enabled, auto_year_level_promotion)
        VALUES (@current_school_year_id, @first_semester_id, 1, 1, 1, 0);
        
        PRINT 'Created system_configuration table with default values.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating system_configuration table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create enrollment_history table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'enrollment_history')
    BEGIN
        PRINT 'Creating enrollment_history table...';
        
        CREATE TABLE enrollment_history (
            history_id INT IDENTITY(1,1) PRIMARY KEY,
            student_id INT NOT NULL,
            school_year_id INT NOT NULL,
            semester_id INT NOT NULL,
            previous_year_level_id INT NULL,
            new_year_level_id INT NULL,
            program_id INT NOT NULL,
            subjects_enrolled INT DEFAULT 0,
            processed_date DATETIME DEFAULT GETDATE(),
            processed_by VARCHAR(100) DEFAULT 'System',
            processing_notes VARCHAR(500) NULL
        );

        -- Add foreign keys if possible
        IF OBJECT_ID('users', 'U') IS NOT NULL
            ALTER TABLE enrollment_history ADD CONSTRAINT FK_enrollment_history_student 
            FOREIGN KEY (student_id) REFERENCES users (user_id);
            
        IF OBJECT_ID('school_years', 'U') IS NOT NULL
            ALTER TABLE enrollment_history ADD CONSTRAINT FK_enrollment_history_school_year 
            FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id);
            
        IF OBJECT_ID('semesters', 'U') IS NOT NULL
            ALTER TABLE enrollment_history ADD CONSTRAINT FK_enrollment_history_semester 
            FOREIGN KEY (semester_id) REFERENCES semesters (semester_id);
            
        IF OBJECT_ID('year_levels', 'U') IS NOT NULL
        BEGIN
            ALTER TABLE enrollment_history ADD CONSTRAINT FK_enrollment_history_prev_year 
            FOREIGN KEY (previous_year_level_id) REFERENCES year_levels (year_level_id);
            
            ALTER TABLE enrollment_history ADD CONSTRAINT FK_enrollment_history_new_year 
            FOREIGN KEY (new_year_level_id) REFERENCES year_levels (year_level_id);
        END
            
        IF OBJECT_ID('programs', 'U') IS NOT NULL
            ALTER TABLE enrollment_history ADD CONSTRAINT FK_enrollment_history_program 
            FOREIGN KEY (program_id) REFERENCES programs (program_id);
            
        PRINT 'Created enrollment_history table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating enrollment_history table: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== POST SYSTEM TABLES (FROM V9.0) ===========

-- Create posts table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'posts')
    BEGIN
        PRINT 'Creating posts table...';
        
        CREATE TABLE posts (
            post_id INT IDENTITY(1,1) PRIMARY KEY,
            user_id INT NOT NULL,                           -- Author of the post
            post_title NVARCHAR(200) NOT NULL,              -- Header/title text
            post_content NVARCHAR(MAX) NULL,                -- Body text
            subject_offering_id INT NULL,                   -- NULL if general post
            section_id INT NULL,                            -- NULL if subject-specific post
            program_id INT NULL,                            -- Program association (optional)
            is_pinned BIT DEFAULT 0,                        -- For pinning important posts
            is_announcement BIT DEFAULT 0,                  -- To mark as announcement
            is_moderated BIT DEFAULT 0,                     -- To mark if post has been moderated
            view_count INT DEFAULT 0,                       -- Track post popularity
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            FOREIGN KEY (subject_offering_id) REFERENCES subject_offerings (offering_id),
            FOREIGN KEY (section_id) REFERENCES sections (section_id),
            FOREIGN KEY (program_id) REFERENCES programs (program_id),
            -- Require at least subject_offering_id OR section_id 
            CONSTRAINT CHK_posts_association CHECK (
                (subject_offering_id IS NOT NULL) OR (section_id IS NOT NULL)
            )
        );

        -- Create index for faster querying
        CREATE NONCLUSTERED INDEX IX_posts_subject_offering_id ON posts (subject_offering_id) 
        WHERE subject_offering_id IS NOT NULL;
        CREATE NONCLUSTERED INDEX IX_posts_section_id ON posts (section_id) 
        WHERE section_id IS NOT NULL;
        CREATE NONCLUSTERED INDEX IX_posts_announcements ON posts (is_announcement)
        WHERE is_announcement = 1;
        
        PRINT 'Created posts table with indexes.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating posts table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_attachments table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_attachments')
    BEGIN
        PRINT 'Creating post_attachments table...';
        
        CREATE TABLE post_attachments (
            attachment_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            file_name NVARCHAR(255) NOT NULL,                -- Original filename
            file_extension NVARCHAR(20) NOT NULL,            -- File extension (e.g., .pdf, .docx)
            file_content VARBINARY(MAX) NOT NULL,            -- Actual file data stored directly in DB
            file_size INT NOT NULL,                          -- Size in bytes
            content_type NVARCHAR(100) NULL,                 -- MIME type
            description NVARCHAR(500) NULL,                  -- Optional description
            upload_date DATETIME DEFAULT GETDATE(),
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE
        );
        
        PRINT 'Created post_attachments table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating post_attachments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_comments table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_comments')
    BEGIN
        PRINT 'Creating post_comments table...';
        
        CREATE TABLE post_comments (
            comment_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            user_id INT NOT NULL,                           -- Comment author
            parent_comment_id INT NULL,                     -- For nested comments (replies)
            comment_content NVARCHAR(MAX) NOT NULL,         -- Comment text
            is_moderated BIT DEFAULT 0,                     -- To mark if comment has been moderated
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            FOREIGN KEY (parent_comment_id) REFERENCES post_comments (comment_id)
        );

        -- Create index for faster querying
        CREATE NONCLUSTERED INDEX IX_post_comments_post_id ON post_comments (post_id);
        
        PRINT 'Created post_comments table with index.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating post_comments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_reactions table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_reactions')
    BEGIN
        PRINT 'Creating post_reactions table...';
        
        CREATE TABLE post_reactions (
            reaction_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            user_id INT NOT NULL,
            reaction_type VARCHAR(20) NOT NULL,              -- 'like', 'helpful', etc.
            created_at DATETIME DEFAULT GETDATE(),
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            -- Ensure a user can only have one reaction type per post
            CONSTRAINT UQ_user_post_reaction UNIQUE (user_id, post_id)
        );
        
        PRINT 'Created post_reactions table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating post_reactions table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_views table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_views')
    BEGIN
        PRINT 'Creating post_views table...';
        
        CREATE TABLE post_views (
            view_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            user_id INT NOT NULL,
            viewed_at DATETIME DEFAULT GETDATE(),
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            -- Ensure a user's view is counted only once
            CONSTRAINT UQ_user_post_view UNIQUE (user_id, post_id)
        );
        
        PRINT 'Created post_views table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating post_views table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create moderation_log table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'moderation_log')
    BEGIN
        PRINT 'Creating moderation_log table...';
        
        CREATE TABLE moderation_log (
            log_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NULL,
            comment_id INT NULL,
            moderator_id INT NOT NULL,                      -- User who moderated the content
            action_type VARCHAR(50) NOT NULL,               -- 'approve', 'reject', 'edit', etc.
            reason NVARCHAR(500) NULL,                      -- Reason for moderation
            moderated_at DATETIME DEFAULT GETDATE(),
            FOREIGN KEY (post_id) REFERENCES posts (post_id),
            FOREIGN KEY (comment_id) REFERENCES post_comments (comment_id),
            FOREIGN KEY (moderator_id) REFERENCES users (user_id),
            -- Ensure either post_id or comment_id is provided
            CONSTRAINT CHK_moderation_target CHECK (
                (post_id IS NOT NULL AND comment_id IS NULL) OR 
                (post_id IS NULL AND comment_id IS NOT NULL)
            )
        );
        
        PRINT 'Created moderation_log table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating moderation_log table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create UpdatePostViewCount trigger
BEGIN TRY
    IF OBJECT_ID('UpdatePostViewCount', 'TR') IS NOT NULL
        DROP TRIGGER UpdatePostViewCount;
    
    EXEC('CREATE TRIGGER UpdatePostViewCount
        ON post_views
        AFTER INSERT
        AS
        BEGIN
            SET NOCOUNT ON;
            
            UPDATE p
            SET p.view_count = p.view_count + 1
            FROM posts p
            INNER JOIN inserted i ON p.post_id = i.post_id;
        END');
    
    PRINT 'Created UpdatePostViewCount trigger.';
END TRY
BEGIN CATCH
    PRINT 'Error creating UpdatePostViewCount trigger: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== CALENDAR SYSTEM TABLES (FROM V9.1) ===========

-- Create calendar_events table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'calendar_events')
    BEGIN
        PRINT 'Creating calendar_events table...';
        
        CREATE TABLE calendar_events (
            event_id INT IDENTITY(1,1) PRIMARY KEY,
            event_title VARCHAR(100) NOT NULL,
            event_description VARCHAR(1000) NULL,  -- Allow NULL for flexibility
            start_date DATETIME NOT NULL,
            end_date DATETIME NULL,               -- Allow NULL for single-day events
            all_day BIT DEFAULT 0,                -- Flag for all-day events
            location VARCHAR(200) NULL,           -- Allow NULL for flexibility
            event_type VARCHAR(50) DEFAULT 'Announcement',  -- Type of event
            created_by INT NOT NULL,              -- User who created the event
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (created_by) REFERENCES users(user_id)
        );
        
        PRINT 'Created calendar_events table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating calendar_events table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_visibility table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_visibility')
    BEGIN
        PRINT 'Creating event_visibility table...';
        
        CREATE TABLE event_visibility (
            visibility_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            visibility_type VARCHAR(50) NOT NULL,  -- 'ALL', 'PROGRAM', 'SECTION', 'YEAR_LEVEL'
            target_id INT NULL,                   -- Program ID, Section ID, etc. (NULL for ALL)
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE
        );
        
        -- Add indexes for performance
        CREATE INDEX IX_event_visibility_event_id ON event_visibility(event_id);
        CREATE INDEX IX_event_visibility_target_id ON event_visibility(target_id);
        
        PRINT 'Created event_visibility table with indexes.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_visibility table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_attachments table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_attachments')
    BEGIN
        PRINT 'Creating event_attachments table...';
        
        CREATE TABLE event_attachments (
            attachment_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            attachment_name VARCHAR(255) NOT NULL,
            attachment_path VARCHAR(500) NOT NULL,
            file_type VARCHAR(100) NULL,          -- Allow NULL for flexibility
            upload_date DATETIME DEFAULT GETDATE(),
            uploaded_by INT NOT NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE,
            FOREIGN KEY (uploaded_by) REFERENCES users(user_id)
        );
        
        PRINT 'Created event_attachments table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_attachments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_reminders table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_reminders')
    BEGIN
        PRINT 'Creating event_reminders table...';
        
        CREATE TABLE event_reminders (
            reminder_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            reminder_time INT NOT NULL,           -- Minutes before the event
            reminder_type VARCHAR(50) DEFAULT 'NOTIFICATION', -- Type of reminder
            is_active BIT DEFAULT 1,
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE
        );
        
        PRINT 'Created event_reminders table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_reminders table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_recurrence table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_recurrence')
    BEGIN
        PRINT 'Creating event_recurrence table...';
        
        CREATE TABLE event_recurrence (
            recurrence_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            recurrence_type VARCHAR(50) NOT NULL,   -- 'DAILY', 'WEEKLY', 'MONTHLY', 'YEARLY'
            interval_value INT DEFAULT 1,          -- Every X days/weeks/etc.
            days_of_week VARCHAR(20) NULL,         -- For weekly: '1,2,3,4,5,6,7' (Sun-Sat)
            day_of_month INT NULL,                 -- For monthly by day: 1-31
            month_of_year INT NULL,                -- For yearly: 1-12
            end_date DATE NULL,                    -- NULL for no end date
            count INT NULL,                        -- Number of occurrences (NULL for no limit)
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE
        );
        
        PRINT 'Created event_recurrence table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_recurrence table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create user_event_responses table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'user_event_responses')
    BEGIN
        PRINT 'Creating user_event_responses table...';
        
        CREATE TABLE user_event_responses (
            response_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            user_id INT NOT NULL,
            response_status VARCHAR(20) NOT NULL,  -- 'ATTENDING', 'DECLINED', 'MAYBE', 'PENDING'
            response_date DATETIME DEFAULT GETDATE(),
            comments VARCHAR(500) NULL,            -- Allow NULL for flexibility
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id),
            FOREIGN KEY (user_id) REFERENCES users(user_id),
            CONSTRAINT UK_user_event_response UNIQUE (user_id, event_id)
        );
        
        PRINT 'Created user_event_responses table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating user_event_responses table: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== STORED PROCEDURES (COMBINED FROM ALL VERSIONS) ===========

PRINT '======================================================================';
PRINT 'CREATING STORED PROCEDURES';
PRINT '======================================================================';
GO

-- EnrollStudentInDefaultSubjects
BEGIN TRY
    IF OBJECT_ID('EnrollStudentInDefaultSubjects', 'P') IS NOT NULL
        DROP PROCEDURE EnrollStudentInDefaultSubjects;
        
    PRINT 'Creating EnrollStudentInDefaultSubjects procedure...';
    
    EXEC('
    CREATE PROCEDURE EnrollStudentInDefaultSubjects
        @student_user_id INT,
        @specific_school_year_id INT = NULL,
        @specific_semester_id INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @program_id INT;
        DECLARE @year_level_id INT;
        DECLARE @section_id INT;
        DECLARE @school_year_id INT;
        DECLARE @semester_id INT;
        DECLARE @subjects_enrolled INT = 0;
        DECLARE @error_message VARCHAR(500) = NULL;
        
        -- Get system configuration if specific values not provided
        IF @specific_school_year_id IS NULL OR @specific_semester_id IS NULL
        BEGIN
            SELECT TOP 1 
                @school_year_id = current_school_year_id,
                @semester_id = current_semester_id
            FROM system_configuration;
        END
        ELSE
        BEGIN
            SET @school_year_id = @specific_school_year_id;
            SET @semester_id = @specific_semester_id;
        END
        
        -- Get student profile information
        SELECT 
            @program_id = program_id,
            @year_level_id = year_level_id,
            @section_id = section_id
        FROM student_profiles
        WHERE user_id = @student_user_id;
        
        -- Check if we got needed information
        IF @program_id IS NULL OR @year_level_id IS NULL OR @school_year_id IS NULL OR @semester_id IS NULL
        BEGIN
            SET @error_message = ''Cannot find student profile information or system configuration.'';
            GOTO ErrorHandler;
        END
        
        -- Create temporary table to store eligible subject offerings
        CREATE TABLE #eligible_subjects (
            offering_id INT,
            subject_id INT,
            class_code VARCHAR(20)
        );
        
        -- Find default subjects from curriculum for this program, year and semester
        INSERT INTO #eligible_subjects (subject_id, class_code)
        SELECT 
            c.subject_id,
            CONCAT(p.program_code, ''-'', s.subject_code, ''-'', 
                   RIGHT(''00'' + CAST(ROW_NUMBER() OVER (PARTITION BY s.subject_id ORDER BY s.subject_id) AS VARCHAR), 2))
        FROM curriculum c
        JOIN programs p ON c.program_id = p.program_id
        JOIN subjects s ON c.subject_id = s.subject_id
        WHERE c.program_id = @program_id
        AND c.year_level_id = @year_level_id
        AND c.semester_id = @semester_id
        AND c.is_active = 1
        AND c.is_default = 1
        AND s.is_active = 1;
        
        -- Check if we found any default subjects
        IF NOT EXISTS (SELECT 1 FROM #eligible_subjects)
        BEGIN
            SET @error_message = ''No default subjects found for this program, year level and semester.'';
            GOTO ErrorHandler;
        END
        
        -- Create or find subject offerings for these subjects
        UPDATE es
        SET es.offering_id = so.offering_id
        FROM #eligible_subjects es
        JOIN subject_offerings so ON es.subject_id = so.subject_id
        WHERE so.school_year_id = @school_year_id
        AND so.semester_id = @semester_id
        AND so.is_active = 1;
        
        -- For subjects without offerings, create them
        INSERT INTO subject_offerings (subject_id, class_code, school_year_id, semester_id, year_level_id, is_active)
        SELECT 
            es.subject_id, 
            es.class_code, 
            @school_year_id,
            @semester_id,
            @year_level_id,
            1
        FROM #eligible_subjects es
        WHERE es.offering_id IS NULL;
        
        -- Update offerings for newly created offerings
        UPDATE es
        SET es.offering_id = so.offering_id
        FROM #eligible_subjects es
        JOIN subject_offerings so ON es.subject_id = so.subject_id AND es.class_code = so.class_code
        WHERE es.offering_id IS NULL
        AND so.school_year_id = @school_year_id
        AND so.semester_id = @semester_id;
        
        -- Enroll student in these offerings, avoiding duplicates
        INSERT INTO enrollments (student_id, offering_id, enrollment_date)
        SELECT 
            @student_user_id,
            es.offering_id,
            GETDATE()
        FROM #eligible_subjects es
        WHERE NOT EXISTS (
            SELECT 1 
            FROM enrollments e 
            WHERE e.student_id = @student_user_id 
            AND e.offering_id = es.offering_id
        );
        
        -- Count the number of subjects enrolled
        SET @subjects_enrolled = @@ROWCOUNT;
        
        -- Record the enrollment history
        INSERT INTO enrollment_history (
            student_id, school_year_id, semester_id, previous_year_level_id, 
            new_year_level_id, program_id, subjects_enrolled, processing_notes
        )
        VALUES (
            @student_user_id, @school_year_id, @semester_id, @year_level_id,
            @year_level_id, @program_id, @subjects_enrolled, ''Auto-enrolled in default subjects''
        );
        
        -- Clean up temp table
        DROP TABLE #eligible_subjects;
        
        -- Return number of subjects enrolled
        SELECT 
            @subjects_enrolled AS subjects_enrolled,
            ''Student enrolled in default subjects successfully.'' AS status;
        RETURN;
        
    ErrorHandler:
        -- Handle errors
        IF OBJECT_ID(''tempdb..#eligible_subjects'') IS NOT NULL
            DROP TABLE #eligible_subjects;
            
        SELECT 
            0 AS subjects_enrolled,
            ''Error: '' + ISNULL(@error_message, ''Unknown error during enrollment.'') AS status;
    END
    ');
    
    PRINT 'Created EnrollStudentInDefaultSubjects stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating EnrollStudentInDefaultSubjects procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- PromoteStudentsToNextYear
BEGIN TRY
    IF OBJECT_ID('PromoteStudentsToNextYear', 'P') IS NOT NULL
        DROP PROCEDURE PromoteStudentsToNextYear;
        
    PRINT 'Creating PromoteStudentsToNextYear procedure...';
    
    EXEC('
    CREATE PROCEDURE PromoteStudentsToNextYear
        @previous_school_year_id INT,
        @new_school_year_id INT,
        @auto_enroll BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Variables for tracking
        DECLARE @students_promoted INT = 0;
        DECLARE @enrollment_errors INT = 0;
        
        -- Get current semester from system config
        DECLARE @current_semester_id INT;
        SELECT TOP 1 @current_semester_id = current_semester_id FROM system_configuration;
        
        -- Create temp table to track students to promote
        CREATE TABLE #students_to_promote (
            user_id INT,
            current_year_level_id INT,
            next_year_level_id INT,
            program_id INT,
            promoted BIT DEFAULT 0
        );
        
        -- Find all students who need to be promoted
        INSERT INTO #students_to_promote (user_id, current_year_level_id, program_id)
        SELECT 
            sp.user_id,
            sp.year_level_id,
            sp.program_id
        FROM student_profiles sp
        JOIN year_levels yl ON sp.year_level_id = yl.year_level_id
        WHERE sp.academic_status = ''active''
        AND sp.school_year_id = @previous_school_year_id;
        
        -- Calculate next year level (using year_level_id + 1, assuming sequential IDs)
        UPDATE s
        SET s.next_year_level_id = next_yl.year_level_id
        FROM #students_to_promote s
        CROSS APPLY (
            SELECT TOP 1 yl.year_level_id
            FROM year_levels yl
            WHERE yl.year_level_id > s.current_year_level_id
            ORDER BY yl.year_level_id
        ) next_yl;
        
        -- Handle max year level students (they stay at their current level but update school year)
        UPDATE s
        SET s.next_year_level_id = s.current_year_level_id
        FROM #students_to_promote s
        WHERE s.next_year_level_id IS NULL;
        
        -- Update student profiles with new year level
        UPDATE sp
        SET 
            sp.year_level_id = s.next_year_level_id,
            sp.school_year_id = @new_school_year_id,
            sp.updated_at = GETDATE()
        FROM student_profiles sp
        JOIN #students_to_promote s ON sp.user_id = s.user_id;
        
        SET @students_promoted = @@ROWCOUNT;
        
        -- Mark as promoted
        UPDATE #students_to_promote SET promoted = 1;
        
        -- Record in history
        INSERT INTO enrollment_history (
            student_id, school_year_id, semester_id, previous_year_level_id, 
            new_year_level_id, program_id, subjects_enrolled, processing_notes
        )
        SELECT 
            s.user_id, 
            @new_school_year_id, 
            @current_semester_id,
            s.current_year_level_id,
            s.next_year_level_id,
            s.program_id,
            0,
            ''Year level promotion from '' + prev_yl.year_name + '' to '' + new_yl.year_name
        FROM #students_to_promote s
        JOIN year_levels prev_yl ON s.current_year_level_id = prev_yl.year_level_id
        JOIN year_levels new_yl ON s.next_year_level_id = new_yl.year_level_id
        WHERE s.promoted = 1;
        
        -- Auto-enroll if requested
        IF @auto_enroll = 1
        BEGIN
            -- Use a cursor for more controlled enrollment with error handling
            DECLARE @student_id INT;
            
            -- Cursor to iterate through promoted students
            DECLARE student_cursor CURSOR FOR 
            SELECT user_id FROM #students_to_promote WHERE promoted = 1;
            
            OPEN student_cursor;
            FETCH NEXT FROM student_cursor INTO @student_id;
            
            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY
                    -- Call the enrollment procedure for each student
                    EXEC EnrollStudentInDefaultSubjects 
                        @student_user_id = @student_id,
                        @specific_school_year_id = @new_school_year_id,
                        @specific_semester_id = @current_semester_id;
                END TRY
                BEGIN CATCH
                    SET @enrollment_errors = @enrollment_errors + 1;
                    
                    -- Log error in history
                    INSERT INTO enrollment_history (
                        student_id, school_year_id, semester_id, previous_year_level_id, 
                        new_year_level_id, program_id, subjects_enrolled, processing_notes
                    )
                    SELECT 
                        s.user_id, 
                        @new_school_year_id, 
                        @current_semester_id,
                        s.current_year_level_id,
                        s.next_year_level_id,
                        s.program_id,
                        0,
                        ''Auto-enrollment error: '' + ERROR_MESSAGE()
                    FROM #students_to_promote s
                    WHERE s.user_id = @student_id;
                END CATCH
                
                FETCH NEXT FROM student_cursor INTO @student_id;
            END
            
            CLOSE student_cursor;
            DEALLOCATE student_cursor;
        END
        
        -- Clean up
        DROP TABLE #students_to_promote;
        
        -- Return results
        SELECT 
            @students_promoted AS students_promoted,
            @enrollment_errors AS enrollment_errors,
            ''Promotion complete'' AS status;
    END
    ');
    
    PRINT 'Created PromoteStudentsToNextYear stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating PromoteStudentsToNextYear procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- CreatePost
BEGIN TRY
    IF OBJECT_ID('CreatePost', 'P') IS NOT NULL
        DROP PROCEDURE CreatePost;
        
    PRINT 'Creating CreatePost procedure...';
    
    EXEC('
    CREATE PROCEDURE CreatePost
        @user_id INT,
        @post_title NVARCHAR(200),
        @post_content NVARCHAR(MAX),
        @subject_offering_id INT = NULL,
        @section_id INT = NULL,
        @program_id INT = NULL,
        @is_announcement BIT = 0,
        @is_moderated BIT = 0,
        @is_pinned BIT = 0,
        @created_post_id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that at least one of subject_offering_id or section_id is provided
        IF @subject_offering_id IS NULL AND @section_id IS NULL
        BEGIN
            RAISERROR(''Either subject_offering_id or section_id must be provided'', 16, 1);
            RETURN;
        END
        
        -- Insert the post
        INSERT INTO posts (
            user_id, 
            post_title, 
            post_content, 
            subject_offering_id, 
            section_id, 
            program_id, 
            is_announcement,
            is_moderated,
            is_pinned,
            created_at
        )
        VALUES (
            @user_id, 
            @post_title, 
            @post_content, 
            @subject_offering_id, 
            @section_id, 
            @program_id, 
            @is_announcement,
            @is_moderated,
            @is_pinned,
            GETDATE()
        );
        
        -- Get the ID of the created post
        SET @created_post_id = SCOPE_IDENTITY();
        
        -- Return the created post ID
        SELECT @created_post_id AS post_id;
    END
    ');
    
    PRINT 'Created CreatePost stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating CreatePost procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- AddPostAttachment
BEGIN TRY
    IF OBJECT_ID('AddPostAttachment', 'P') IS NOT NULL
        DROP PROCEDURE AddPostAttachment;
        
    PRINT 'Creating AddPostAttachment procedure...';
    
    EXEC('
    CREATE PROCEDURE AddPostAttachment
        @post_id INT,
        @file_name NVARCHAR(255),
        @file_extension NVARCHAR(20),
        @file_content VARBINARY(MAX),
        @file_size INT,
        @content_type NVARCHAR(100) = NULL,
        @description NVARCHAR(500) = NULL,
        @created_attachment_id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the post exists
        IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = @post_id)
        BEGIN
            RAISERROR(''Post does not exist'', 16, 1);
            RETURN;
        END
        
        -- Insert the attachment
        INSERT INTO post_attachments (
            post_id,
            file_name,
            file_extension,
            file_content,
            file_size,
            content_type,
            description,
            upload_date
        )
        VALUES (
            @post_id,
            @file_name,
            @file_extension,
            @file_content,
            @file_size,
            @content_type,
            @description,
            GETDATE()
        );
        
        -- Get the ID of the created attachment
        SET @created_attachment_id = SCOPE_IDENTITY();
        
        -- Return the created attachment ID
        SELECT @created_attachment_id AS attachment_id;
    END
    ');
    
    PRINT 'Created AddPostAttachment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating AddPostAttachment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- AddPostComment 
BEGIN TRY
    IF OBJECT_ID('AddPostComment', 'P') IS NOT NULL
        DROP PROCEDURE AddPostComment;
        
    PRINT 'Creating AddPostComment procedure...';
    
    EXEC('
    CREATE PROCEDURE AddPostComment
        @post_id INT,
        @user_id INT,
        @comment_content NVARCHAR(MAX),
        @parent_comment_id INT = NULL,
        @is_moderated BIT = 0,
        @created_comment_id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the post exists
        IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = @post_id)
        BEGIN
            RAISERROR(''Post does not exist'', 16, 1);
            RETURN;
        END
        
        -- Validate parent comment if provided
        IF @parent_comment_id IS NOT NULL AND 
           NOT EXISTS (SELECT 1 FROM post_comments WHERE comment_id = @parent_comment_id AND post_id = @post_id)
        BEGIN
            RAISERROR(''Parent comment does not exist or does not belong to the specified post'', 16, 1);
            RETURN;
        END
        
        -- Insert the comment
        INSERT INTO post_comments (
            post_id,
            user_id,
            parent_comment_id,
            comment_content,
            is_moderated,
            created_at
        )
        VALUES (
            @post_id,
            @user_id,
            @parent_comment_id,
            @comment_content,
            @is_moderated,
            GETDATE()
        );
        
        -- Get the ID of the created comment
        SET @created_comment_id = SCOPE_IDENTITY();
        
        -- Return the created comment ID
        SELECT @created_comment_id AS comment_id;
    END
    ');
    
    PRINT 'Created AddPostComment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating AddPostComment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- ModeratePost
BEGIN TRY
    IF OBJECT_ID('ModeratePost', 'P') IS NOT NULL
        DROP PROCEDURE ModeratePost;
        
    PRINT 'Creating ModeratePost procedure...';
    
    EXEC('
    CREATE PROCEDURE ModeratePost
        @post_id INT,
        @moderator_id INT,
        @action_type VARCHAR(50),
        @reason NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the post exists
        IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = @post_id)
        BEGIN
            RAISERROR(''Post does not exist'', 16, 1);
            RETURN;
        END
        
        -- Update the post''s moderation status
        BEGIN TRANSACTION;
        
        -- Mark post as moderated
        UPDATE posts 
        SET is_moderated = 1,
            updated_at = GETDATE(),
            -- If action is to deactivate, set is_active to 0
            is_active = CASE WHEN @action_type = ''deactivate'' THEN 0 ELSE is_active END
        WHERE post_id = @post_id;
        
        -- Log the moderation action
        INSERT INTO moderation_log (
            post_id,
            moderator_id,
            action_type,
            reason,
            moderated_at
        )
        VALUES (
            @post_id,
            @moderator_id,
            @action_type,
            @reason,
            GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Return success
        SELECT 1 AS success;
    END
    ');
    
    PRINT 'Created ModeratePost stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating ModeratePost procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- ModerateComment
BEGIN TRY
    IF OBJECT_ID('ModerateComment', 'P') IS NOT NULL
        DROP PROCEDURE ModerateComment;
        
    PRINT 'Creating ModerateComment procedure...';
    
    EXEC('
    CREATE PROCEDURE ModerateComment
        @comment_id INT,
        @moderator_id INT,
        @action_type VARCHAR(50),
        @reason NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the comment exists
        IF NOT EXISTS (SELECT 1 FROM post_comments WHERE comment_id = @comment_id)
        BEGIN
            RAISERROR(''Comment does not exist'', 16, 1);
            RETURN;
        END
        
        -- Update the comment''s moderation status
        BEGIN TRANSACTION;
        
        -- Mark comment as moderated
        UPDATE post_comments 
        SET is_moderated = 1,
            updated_at = GETDATE(),
            -- If action is to deactivate, set is_active to 0
            is_active = CASE WHEN @action_type = ''deactivate'' THEN 0 ELSE is_active END
        WHERE comment_id = @comment_id;
        
        -- Log the moderation action
        INSERT INTO moderation_log (
            comment_id,
            moderator_id,
            action_type,
            reason,
            moderated_at
        )
        VALUES (
            @comment_id,
            @moderator_id,
            @action_type,
            @reason,
            GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Return success
        SELECT 1 AS success;
    END
    ');
    
    PRINT 'Created ModerateComment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating ModerateComment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- GetPostsBySubjectOffering
BEGIN TRY
    IF OBJECT_ID('GetPostsBySubjectOffering', 'P') IS NOT NULL
        DROP PROCEDURE GetPostsBySubjectOffering;
        
    PRINT 'Creating GetPostsBySubjectOffering procedure...';
    
    EXEC('
    CREATE PROCEDURE GetPostsBySubjectOffering
        @subject_offering_id INT,
        @page_number INT = 1,
        @page_size INT = 10,
        @include_inactive BIT = 0,
        @only_announcements BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Get posts for the subject offering
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_comments pc WHERE pc.post_id = p.post_id) AS comment_count,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.subject_offering_id = @subject_offering_id
            AND (p.is_active = 1 OR @include_inactive = 1)
            AND (@only_announcements = 0 OR p.is_announcement = 1)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created GetPostsBySubjectOffering stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetPostsBySubjectOffering procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- GetPostsBySection
BEGIN TRY
    IF OBJECT_ID('GetPostsBySection', 'P') IS NOT NULL
        DROP PROCEDURE GetPostsBySection;
        
    PRINT 'Creating GetPostsBySection procedure...';
    
    EXEC('
    CREATE PROCEDURE GetPostsBySection
        @section_id INT,
        @page_number INT = 1,
        @page_size INT = 10,
        @include_inactive BIT = 0,
        @only_announcements BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Get posts for the section
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_comments pc WHERE pc.post_id = p.post_id) AS comment_count,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.section_id = @section_id
            AND (p.is_active = 1 OR @include_inactive = 1)
            AND (@only_announcements = 0 OR p.is_announcement = 1)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created GetPostsBySection stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetPostsBySection procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- GetAnnouncements
BEGIN TRY
    IF OBJECT_ID('GetAnnouncements', 'P') IS NOT NULL
        DROP PROCEDURE GetAnnouncements;
        
    PRINT 'Creating GetAnnouncements procedure...';
    
    EXEC('
    CREATE PROCEDURE GetAnnouncements
        @section_id INT = NULL,
        @subject_offering_id INT = NULL,
        @program_id INT = NULL,
        @page_number INT = 1,
        @page_size INT = 10
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Get announcements based on filters
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_moderated,
            p.subject_offering_id,
            p.section_id,
            p.program_id,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.is_announcement = 1
            AND p.is_active = 1
            AND (@section_id IS NULL OR p.section_id = @section_id)
            AND (@subject_offering_id IS NULL OR p.subject_offering_id = @subject_offering_id)
            AND (@program_id IS NULL OR p.program_id = @program_id)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created GetAnnouncements stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetAnnouncements procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- GetPostDetails
BEGIN TRY
    IF OBJECT_ID('GetPostDetails', 'P') IS NOT NULL
        DROP PROCEDURE GetPostDetails;
        
    PRINT 'Creating GetPostDetails procedure...';
    
    EXEC('
    CREATE PROCEDURE GetPostDetails
        @post_id INT,
        @user_id INT = NULL  -- Optional: to record view and check if user has reacted
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Get post details
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            p.subject_offering_id,
            p.section_id,
            p.program_id,
            u.user_id AS author_id,
            u.first_name + '' '' + u.last_name AS author_name,
            CASE WHEN @user_id IS NOT NULL AND EXISTS (
                SELECT 1 FROM post_reactions pr 
                WHERE pr.post_id = p.post_id AND pr.user_id = @user_id
            ) THEN 1 ELSE 0 END AS user_has_reacted,
            (SELECT COUNT(*) FROM post_reactions pr WHERE pr.post_id = p.post_id) AS reaction_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.post_id = @post_id;
            
        -- Get post comments
        SELECT 
            pc.comment_id,
            pc.post_id,
            pc.parent_comment_id,
            pc.comment_content,
            pc.created_at,
            pc.updated_at,
            pc.is_moderated,
            u.user_id,
            u.first_name + '' '' + u.last_name AS commenter_name
        FROM 
            post_comments pc
            INNER JOIN users u ON pc.user_id = u.user_id
        WHERE 
            pc.post_id = @post_id
            AND pc.is_active = 1
        ORDER BY 
            pc.created_at ASC;
            
        -- Get post attachments
        SELECT 
            pa.attachment_id,
            pa.post_id,
            pa.file_name,
            pa.file_extension,
            pa.file_size,
            pa.content_type,
            pa.description,
            pa.upload_date
        FROM 
            post_attachments pa
        WHERE 
            pa.post_id = @post_id;
            
        -- Record user view if user_id is provided
        IF @user_id IS NOT NULL
        BEGIN
            -- Try to insert a view record if one doesn''t already exist
            IF NOT EXISTS (SELECT 1 FROM post_views WHERE post_id = @post_id AND user_id = @user_id)
            BEGIN
                BEGIN TRY
                    INSERT INTO post_views (post_id, user_id, viewed_at)
                    VALUES (@post_id, @user_id, GETDATE());
                END TRY
                BEGIN CATCH
                    -- Handle duplicate key error quietly - this means the view was already recorded
                    IF ERROR_NUMBER() <> 2601 AND ERROR_NUMBER() <> 2627
                        THROW;
                END CATCH
            END
        END
    END
    ');
    
    PRINT 'Created GetPostDetails stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetPostDetails procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- SearchPosts
BEGIN TRY
    IF OBJECT_ID('SearchPosts', 'P') IS NOT NULL
        DROP PROCEDURE SearchPosts;
        
    PRINT 'Creating SearchPosts procedure...';
    
    EXEC('
    CREATE PROCEDURE SearchPosts
        @search_term NVARCHAR(100),
        @program_id INT = NULL,
        @section_id INT = NULL,
        @subject_offering_id INT = NULL,
        @only_announcements BIT = 0,
        @page_number INT = 1,
        @page_size INT = 10
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Search posts
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            p.subject_offering_id,
            p.section_id,
            p.program_id,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_comments pc WHERE pc.post_id = p.post_id) AS comment_count,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            (p.post_title LIKE ''%'' + @search_term + ''%'' OR p.post_content LIKE ''%'' + @search_term + ''%'')
            AND p.is_active = 1
            AND (@program_id IS NULL OR p.program_id = @program_id)
            AND (@section_id IS NULL OR p.section_id = @section_id)
            AND (@subject_offering_id IS NULL OR p.subject_offering_id = @subject_offering_id)
            AND (@only_announcements = 0 OR p.is_announcement = 1)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created SearchPosts stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating SearchPosts procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- DownloadPostAttachment
BEGIN TRY
    IF OBJECT_ID('DownloadPostAttachment', 'P') IS NOT NULL
        DROP PROCEDURE DownloadPostAttachment;
        
    PRINT 'Creating DownloadPostAttachment procedure...';
    
    EXEC('
    CREATE PROCEDURE DownloadPostAttachment
        @attachment_id INT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Return file data and metadata
        SELECT 
            file_name,
            file_extension,
            file_content,
            file_size,
            content_type
        FROM 
            post_attachments
        WHERE 
            attachment_id = @attachment_id;
    END
    ');
    
    PRINT 'Created DownloadPostAttachment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating DownloadPostAttachment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Calendar System Procedures

-- Create upcoming_events_by_section view
BEGIN TRY
    EXEC('
    CREATE OR ALTER VIEW upcoming_events_by_section AS
    SELECT 
        ce.event_id,
        ce.event_title,
        ce.event_description,
        ce.start_date,
        ce.end_date,
        ce.location,
        ce.event_type,
        u.first_name + '' '' + u.last_name AS created_by_name,
        s.section_id,
        s.section_name,
        p.program_id,
        p.program_name,
        yl.year_level_id,
        yl.year_name
    FROM 
        calendar_events ce
    INNER JOIN 
        users u ON ce.created_by = u.user_id
    INNER JOIN 
        event_visibility ev ON ce.event_id = ev.event_id
    LEFT JOIN 
        sections s ON (ev.visibility_type = ''SECTION'' AND ev.target_id = s.section_id)
    LEFT JOIN
        programs p ON (ev.visibility_type = ''PROGRAM'' AND ev.target_id = p.program_id)
    LEFT JOIN
        year_levels yl ON (ev.visibility_type = ''YEAR_LEVEL'' AND ev.target_id = yl.year_level_id)
    WHERE 
        ce.is_active = 1
        AND ce.start_date >= GETDATE()
        OR (ce.end_date IS NOT NULL AND ce.end_date >= GETDATE())
    ');
    
    PRINT 'Created upcoming_events_by_section view.';
END TRY
BEGIN CATCH
    PRINT 'Error creating upcoming_events_by_section view: ' + ERROR_MESSAGE();
END CATCH
GO

-- sp_add_calendar_event
BEGIN TRY
    EXEC('
    CREATE OR ALTER PROCEDURE sp_add_calendar_event
        @title VARCHAR(100),
        @description VARCHAR(1000) = NULL,
        @start_date DATETIME,
        @end_date DATETIME = NULL,
        @all_day BIT = 0,
        @location VARCHAR(200) = NULL,
        @event_type VARCHAR(50) = ''Announcement'',
        @created_by INT,
        @visibility_type VARCHAR(50) = ''ALL'',
        @target_ids VARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @event_id INT;
        
        BEGIN TRANSACTION;
        
        BEGIN TRY
            -- Insert the event
            INSERT INTO calendar_events (
                event_title, 
                event_description, 
                start_date, 
                end_date, 
                all_day, 
                location, 
                event_type, 
                created_by
            )
            VALUES (
                @title, 
                @description, 
                @start_date, 
                @end_date, 
                @all_day, 
                @location, 
                @event_type, 
                @created_by
            );
            
            SET @event_id = SCOPE_IDENTITY();
            
            -- Handle visibility settings
            IF @visibility_type = ''ALL''
            BEGIN
                INSERT INTO event_visibility (event_id, visibility_type, target_id)
                VALUES (@event_id, ''ALL'', NULL);
            END
            ELSE IF @target_ids IS NOT NULL
            BEGIN
                -- Split the target IDs by comma and insert
                WITH targets AS (
                    SELECT value AS target_id
                    FROM STRING_SPLIT(@target_ids, '','')
                )
                INSERT INTO event_visibility (event_id, visibility_type, target_id)
                SELECT @event_id, @visibility_type, CAST(target_id AS INT)
                FROM targets;
            END
            
            COMMIT TRANSACTION;
            
            -- Return the new event ID
            SELECT @event_id AS event_id, ''Event created successfully'' AS status;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            SELECT 0 AS event_id, ''Error creating event: '' + ERROR_MESSAGE() AS status;
        END CATCH
    END
    ');
    
    PRINT 'Created sp_add_calendar_event stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating sp_add_calendar_event stored procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- sp_get_section_events
BEGIN TRY
    EXEC('
    CREATE OR ALTER PROCEDURE sp_get_section_events
        @section_id INT,
        @start_date DATE,
        @end_date DATE = NULL,
        @include_program_events BIT = 1,
        @include_year_level_events BIT = 1,
        @include_all_events BIT = 1
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- If no end date provided, default to 31 days after start
        IF @end_date IS NULL
            SET @end_date = DATEADD(DAY, 31, @start_date);
            
        -- Get section, program, and year level info
        DECLARE @program_id INT;
        DECLARE @year_level_id INT;
        
        SELECT 
            @program_id = s.program_id,
            @year_level_id = s.year_level_id
        FROM sections s
        WHERE s.section_id = @section_id;
        
        -- Query events visible to this section
        SELECT 
            ce.event_id,
            ce.event_title,
            ce.event_description,
            ce.start_date,
            ce.end_date,
            ce.all_day,
            ce.location,
            ce.event_type,
            u.first_name + '' '' + u.last_name AS created_by_name,
            CASE
                WHEN ev.visibility_type = ''SECTION'' THEN ''Section-specific''
                WHEN ev.visibility_type = ''PROGRAM'' THEN ''Program-wide''
                WHEN ev.visibility_type = ''YEAR_LEVEL'' THEN ''Year level''
                WHEN ev.visibility_type = ''ALL'' THEN ''Everyone''
                ELSE ''Unknown''
            END AS visibility
        FROM calendar_events ce
        INNER JOIN users u ON ce.created_by = u.user_id
        INNER JOIN event_visibility ev ON ce.event_id = ev.event_id
        WHERE ce.is_active = 1
        AND (
            (ce.start_date BETWEEN @start_date AND @end_date)
            OR (ce.end_date IS NOT NULL AND ce.end_date BETWEEN @start_date AND @end_date)
            OR (ce.start_date <= @start_date AND (ce.end_date IS NULL OR ce.end_date >= @end_date))
        )
        AND (
            (ev.visibility_type = ''SECTION'' AND ev.target_id = @section_id)
            OR (@include_program_events = 1 AND ev.visibility_type = ''PROGRAM'' AND ev.target_id = @program_id)
            OR (@include_year_level_events = 1 AND ev.visibility_type = ''YEAR_LEVEL'' AND ev.target_id = @year_level_id)
            OR (@include_all_events = 1 AND ev.visibility_type = ''ALL'')
        )
        ORDER BY ce.start_date, ce.event_title;
    END
    ');
    
    PRINT 'Created sp_get_section_events stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating sp_get_section_events stored procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- sp_get_events_by_date
BEGIN TRY
    EXEC('
    CREATE OR ALTER PROCEDURE sp_get_events_by_date
        @start_date DATE,
        @end_date DATE = NULL,
        @user_id INT = NULL,
        @visibility_filter VARCHAR(50) = NULL,
        @target_id INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- If no end date provided, default to 31 days after start
        IF @end_date IS NULL
            SET @end_date = DATEADD(DAY, 31, @start_date);
        
        -- Get user profile information if filtering is needed
        DECLARE @user_program_id INT;
        DECLARE @user_year_level_id INT;
        DECLARE @user_section_id INT;
        
        IF @user_id IS NOT NULL
        BEGIN
            SELECT 
                @user_program_id = program_id,
                @user_year_level_id = year_level_id,
                @user_section_id = section_id
            FROM student_profiles
            WHERE user_id = @user_id;
        END
        
        -- Query events
        SELECT 
            ce.event_id,
            ce.event_title,
            ce.event_description,
            ce.start_date,
            ce.end_date,
            ce.all_day,
            ce.location,
            ce.event_type,
            u.first_name + '' '' + u.last_name AS created_by_name,
            CASE
                WHEN ev.visibility_type = ''SECTION'' THEN ''Section-specific''
                WHEN ev.visibility_type = ''PROGRAM'' THEN ''Program-wide''
                WHEN ev.visibility_type = ''YEAR_LEVEL'' THEN ''Year level''
                WHEN ev.visibility_type = ''ALL'' THEN ''Everyone''
                ELSE ''Unknown''
            END AS visibility
        FROM calendar_events ce
        INNER JOIN users u ON ce.created_by = u.user_id
        INNER JOIN event_visibility ev ON ce.event_id = ev.event_id
        WHERE ce.is_active = 1
        AND (
            (ce.start_date BETWEEN @start_date AND @end_date)
            OR (ce.end_date IS NOT NULL AND ce.end_date BETWEEN @start_date AND @end_date)
            OR (ce.start_date <= @start_date AND (ce.end_date IS NULL OR ce.end_date >= @end_date))
        )
        -- Apply user-based visibility filtering
        AND (
            @user_id IS NULL
            OR @user_id = ce.created_by
            OR (ev.visibility_type = ''ALL'')
            OR (ev.visibility_type = ''SECTION'' AND ev.target_id = @user_section_id)
            OR (ev.visibility_type = ''PROGRAM'' AND ev.target_id = @user_program_id)
            OR (ev.visibility_type = ''YEAR_LEVEL'' AND ev.target_id = @user_year_level_id)
        )
        -- Apply explicit visibility filtering if provided
        AND (
            @visibility_filter IS NULL
            OR (ev.visibility_type = @visibility_filter AND (@target_id IS NULL OR ev.target_id = @target_id))
        )
        ORDER BY ce.start_date, ce.event_title;
    END
    ');
    
    PRINT 'Created sp_get_events_by_date stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating sp_get_events_by_date stored procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== FINAL SYSTEM SUMMARY ===========

PRINT '======================================================================';
PRINT 'V10 CONSOLIDATED SCHEMA INSTALLATION COMPLETED';
PRINT '======================================================================';
PRINT 'The following components have been installed:';
PRINT '';
PRINT '1. Base Tables (from V8.2):';
PRINT '   - Core system tables (users, school_years, semesters, year_levels)';
PRINT '   - Academic structure tables (programs, sections, subjects)';
PRINT '   - Student/faculty profile tables';
PRINT '   - Curriculum and enrollment tables';
PRINT '   - System configuration and enrollment history tables';
PRINT '';
PRINT '2. Post System (from V9.0):';
PRINT '   - Posts with attachments and comments functionality';
PRINT '   - Post reactions and view tracking';
PRINT '   - Announcement capabilities';
PRINT '   - Moderation system';
PRINT '';
PRINT '3. Calendar System (from V9.1):';
PRINT '   - Event scheduling with flexible visibility';
PRINT '   - Event attachments, reminders, and recurrence';
PRINT '   - RSVP functionality';
PRINT '';
PRINT '4. Stored Procedures:';
PRINT '   - Student enrollment procedures';
PRINT '   - Post management procedures';
PRINT '   - Calendar and event management procedures';
PRINT '';
PRINT '5. Views:';
PRINT '   - Upcoming events by section';
PRINT '';
PRINT '6. Version History:';
PRINT '   - Complete tracking of schema versions and their details';
PRINT '======================================================================';
PRINT 'Schema version has been updated to V10.0';
PRINT 'Generated on: 2025-04-10 13:51:22';
PRINT 'User: Bobsi01';
PRINT '======================================================================';
GO