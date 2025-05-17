/*
Classroom Management System - Enhanced Schema with Default Subject Enrollment
Version: 8.1 (FIXED)
Date: 2025-04-09
Description: Enhanced schema with automatic enrollment in default subjects, system configuration,
            and support for semester-specific default subjects.
            This version includes fixes for missing columns and tables.
*/

-- Set error handling to continue execution on errors
SET XACT_ABORT OFF;
SET NOCOUNT ON;
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

    -- Insert version record for V8.2 if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '8.1')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('8.1', 'Added default subject enrollment and system configuration capability');
        PRINT 'Added schema version 8.1 record.';
    END
    ELSE
        PRINT 'Schema version 8.1 record already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error handling schema_version table: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== VALIDATE AND FIX BASE TABLES ===========
-- First check and fix programs table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'programs')
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

-- Check and fix subjects table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
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

-- Check and fix curriculum table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'curriculum')
    BEGIN
        -- Add is_active column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE curriculum ADD is_active BIT DEFAULT 1;
            PRINT 'Added is_active column to curriculum table.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error updating curriculum table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check and fix subject_offerings table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_offerings')
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
    PRINT 'Error updating subject_offerings table: ' + ERROR_MESSAGE();
END CATCH
GO

GO

-- Add missing foreign key references if needed
BEGIN TRY
    -- Add section_id foreign key to student_profiles if it doesn't exist
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'student_profiles' AND COLUMN_NAME = 'section_id')
    AND NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
                   WHERE CONSTRAINT_NAME = 'FK_student_profiles_section')
    BEGIN
        ALTER TABLE student_profiles ADD CONSTRAINT FK_student_profiles_section
        FOREIGN KEY (section_id) REFERENCES sections(section_id);
        PRINT 'Added section foreign key to student_profiles table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error adding foreign keys: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== VALIDATE BASE TABLES EXIST ===========
-- This section checks if essential tables from previous schemas exist

-- Check for users table (required for foreign keys)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
    BEGIN
        PRINT 'WARNING: users table does not exist. This is required from previous schema versions.';
        PRINT 'Creating minimal users table structure...';
        
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
        
        PRINT 'Created minimal users table.';
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
        PRINT 'WARNING: school_years table does not exist. This is required from previous schema versions.';
        PRINT 'Creating school_years table structure...';
        
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
        VALUES ('2024-2025', 1, '2024-06-01', '2025-05-31');
        
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
        PRINT 'WARNING: semesters table does not exist. This is required from previous schema versions.';
        PRINT 'Creating semesters table structure...';
        
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
        PRINT 'WARNING: year_levels table does not exist. This is required from previous schema versions.';
        PRINT 'Creating year_levels table structure...';
        
        CREATE TABLE year_levels (
            year_level_id INT IDENTITY(1,1) PRIMARY KEY,
            year_name VARCHAR(20) NOT NULL,
            description VARCHAR(500) NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        CREATE TABLE sections (
    section_id INT IDENTITY(1,1) PRIMARY KEY,
    section_name VARCHAR(20) NOT NULL,
    year_level_id INT NOT NULL,
    program_id INT NOT NULL,
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (year_level_id) REFERENCES year_levels(year_level_id)
);

-- Add FK reference to programs after programs table is created
IF OBJECT_ID('programs', 'U') IS NOT NULL
    ALTER TABLE sections ADD CONSTRAINT FK_sections_program
    FOREIGN KEY (program_id) REFERENCES programs(program_id);

-- Insert default sections (after both year_levels and programs are created)
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
        PRINT 'WARNING: programs table does not exist. This is required from previous schema versions.';
        PRINT 'Creating programs table structure...';
        
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
END TRY
BEGIN CATCH
    PRINT 'Error checking programs table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for student_profiles table (required for enrollment)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'student_profiles')
    BEGIN
        PRINT 'WARNING: student_profiles table does not exist. This is required from previous schema versions.';
        PRINT 'Creating student_profiles table structure...';
        
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

-- Check for faculty_profiles table (required for curriculum manager)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'faculty_profiles')
    BEGIN
        PRINT 'WARNING: faculty_profiles table does not exist. Creating faculty_profiles table structure...';
        
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

-- Check for subjects table (required for enrollment)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
    BEGIN
        PRINT 'WARNING: subjects table does not exist. This is required from previous schema versions.';
        PRINT 'Creating subjects table structure...';
        
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
END TRY
BEGIN CATCH
    PRINT 'Error checking subjects table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for curriculum table (required for default subject enrollment)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'curriculum')
    BEGIN
        PRINT 'WARNING: curriculum table does not exist. This is required from previous schema versions.';
        PRINT 'Creating curriculum table structure...';
        
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
        
        PRINT 'Created curriculum table.';
        
        -- Add faculty_id foreign key if users table exists
        IF OBJECT_ID('users', 'U') IS NOT NULL
        BEGIN
            ALTER TABLE curriculum ADD CONSTRAINT FK_curriculum_faculty 
            FOREIGN KEY (faculty_id) REFERENCES users (user_id);
            PRINT 'Added faculty_id foreign key to curriculum table.';
        END
    END
    ELSE
    BEGIN
        -- Check if is_default column exists in curriculum table
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'is_default')
        BEGIN
            ALTER TABLE curriculum ADD is_default BIT DEFAULT 0;
            PRINT 'Added is_default column to curriculum table.';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error handling curriculum table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for subject_offerings table (required for enrollment)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_offerings')
    BEGIN
        PRINT 'WARNING: subject_offerings table does not exist. This is required from previous schema versions.';
        PRINT 'Creating subject_offerings table structure...';
        
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
END TRY
BEGIN CATCH
    PRINT 'Error checking subject_offerings table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check for enrollments table (required for enrollment)
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'enrollments')
    BEGIN
        PRINT 'WARNING: enrollments table does not exist. This is required from previous schema versions.';
        PRINT 'Creating enrollments table structure...';
        
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

-- =========== NEW TABLES FOR V8.2 ===========

-- Create system_configuration table safely
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'system_configuration')
    BEGIN
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
    ELSE
        PRINT 'system_configuration table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating system_configuration table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create enrollment_history table safely
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'enrollment_history')
    BEGIN
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
    ELSE
        PRINT 'enrollment_history table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating enrollment_history table: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== STORED PROCEDURES FOR V8.2 ===========

-- Create EnrollStudentInDefaultSubjects stored procedure
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

-- Create PromoteStudentsToNextYear stored procedure
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
            ''Students promoted to the next year level'' AS status;
    END
    ');
    
    PRINT 'Created PromoteStudentsToNextYear stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating PromoteStudentsToNextYear procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create SetDefaultSubjectsInCurriculum stored procedure
BEGIN TRY
    IF OBJECT_ID('SetDefaultSubjectsInCurriculum', 'P') IS NOT NULL
        DROP PROCEDURE SetDefaultSubjectsInCurriculum;
        
    PRINT 'Creating SetDefaultSubjectsInCurriculum procedure...';
    
    EXEC('
    CREATE PROCEDURE SetDefaultSubjectsInCurriculum
        @program_id INT,
        @year_level_id INT,
        @semester_id INT,
        @subject_ids VARCHAR(MAX), -- Comma-separated list of subject IDs
        @curriculum_year VARCHAR(9) = NULL,
        @set_all_default BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- If no curriculum year provided, use the current one
        IF @curriculum_year IS NULL
        BEGIN
            SELECT @curriculum_year = year_name 
            FROM school_years 
            WHERE is_current = 1;
            
            -- If still NULL, use the latest year
            IF @curriculum_year IS NULL
            BEGIN
                SELECT @curriculum_year = MAX(curriculum_year) 
                FROM curriculum
                WHERE program_id = @program_id;
            END
        END
        
        -- If set_all_default, mark all matching subjects as default
        IF @set_all_default = 1
        BEGIN
            UPDATE curriculum
            SET is_default = 1
            WHERE program_id = @program_id
            AND year_level_id = @year_level_id
            AND semester_id = @semester_id
            AND curriculum_year = @curriculum_year
            AND is_active = 1;
            
            SELECT 
                COUNT(*) AS subjects_set_default,
                ''All matching subjects set as default'' AS status
            FROM curriculum
            WHERE program_id = @program_id
            AND year_level_id = @year_level_id
            AND semester_id = @semester_id
            AND curriculum_year = @curriculum_year
            AND is_default = 1;
            
            RETURN;
        END
        
        -- If specific subjects provided, convert the list to a table
        IF @subject_ids IS NOT NULL
        BEGIN
            -- Reset all default flags for this criteria
            UPDATE curriculum
            SET is_default = 0
            WHERE program_id = @program_id
            AND year_level_id = @year_level_id
            AND semester_id = @semester_id
            AND curriculum_year = @curriculum_year;
            
            -- Convert comma-separated string to a table
            DECLARE @SubjectTable TABLE (subject_id INT);
            
            INSERT INTO @SubjectTable
            SELECT CAST(value AS INT)
            FROM STRING_SPLIT(@subject_ids, '','');
            
            -- Set the specified subjects as default
            UPDATE c
            SET is_default = 1
            FROM curriculum c
            JOIN @SubjectTable st ON c.subject_id = st.subject_id
            WHERE c.program_id = @program_id
            AND c.year_level_id = @year_level_id
            AND c.semester_id = @semester_id
            AND c.curriculum_year = @curriculum_year;
            
            SELECT 
                @@ROWCOUNT AS subjects_set_default,
                ''Specified subjects set as default'' AS status;
        END
    END
    ');
    
    PRINT 'Created SetDefaultSubjectsInCurriculum stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating SetDefaultSubjectsInCurriculum procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create UpdateSystemConfiguration stored procedure
BEGIN TRY
    IF OBJECT_ID('UpdateSystemConfiguration', 'P') IS NOT NULL
        DROP PROCEDURE UpdateSystemConfiguration;
        
    PRINT 'Creating UpdateSystemConfiguration procedure...';
    
    EXEC('
    CREATE PROCEDURE UpdateSystemConfiguration
        @school_year_id INT = NULL,
        @semester_id INT = NULL,
        @registration_open BIT = NULL,
        @enrollment_open BIT = NULL,
        @auto_enroll_enabled BIT = NULL,
        @auto_year_level_promotion BIT = NULL,
        @updated_by INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Check if we have any system configuration
        IF NOT EXISTS (SELECT 1 FROM system_configuration)
        BEGIN
            -- Insert default system configuration
            DECLARE @default_school_year_id INT, @default_semester_id INT;
            
            -- Get current school year
            SELECT TOP 1 @default_school_year_id = school_year_id 
            FROM school_years 
            WHERE is_current = 1;
            
            IF @default_school_year_id IS NULL
                SELECT TOP 1 @default_school_year_id = school_year_id FROM school_years ORDER BY school_year_id DESC;
                
            -- Get first semester
            SELECT TOP 1 @default_semester_id = semester_id 
            FROM semesters 
            WHERE semester_code = ''1ST'';
            
            IF @default_semester_id IS NULL
                SELECT TOP 1 @default_semester_id = semester_id FROM semesters ORDER BY semester_id;
                
            INSERT INTO system_configuration (
                current_school_year_id, current_semester_id, registration_open, 
                enrollment_open, auto_enroll_enabled, auto_year_level_promotion
            )
            VALUES (
                @default_school_year_id, @default_semester_id, 1, 1, 1, 0
            );
        END
        
        -- Update system configuration with provided values
        UPDATE system_configuration
        SET 
            current_school_year_id = ISNULL(@school_year_id, current_school_year_id),
            current_semester_id = ISNULL(@semester_id, current_semester_id),
            registration_open = ISNULL(@registration_open, registration_open),
            enrollment_open = ISNULL(@enrollment_open, enrollment_open),
            auto_enroll_enabled = ISNULL(@auto_enroll_enabled, auto_enroll_enabled),
            auto_year_level_promotion = ISNULL(@auto_year_level_promotion, auto_year_level_promotion),
            last_updated = GETDATE(),
            updated_by = @updated_by
        WHERE config_id = (SELECT TOP 1 config_id FROM system_configuration);
        
        -- If updating school year, also update the is_current flag in school_years table
        IF @school_year_id IS NOT NULL
        BEGIN
            -- First reset all school years
            UPDATE school_years SET is_current = 0;
            
            -- Then set the current one
            UPDATE school_years SET is_current = 1 WHERE school_year_id = @school_year_id;
        END
        
        -- Return updated configuration
        SELECT 
            sc.*,
            sy.year_name AS current_school_year,
            sem.semester_name AS current_semester
        FROM system_configuration sc
        JOIN school_years sy ON sc.current_school_year_id = sy.school_year_id
        JOIN semesters sem ON sc.current_semester_id = sem.semester_id;
    END
    ');
    
    PRINT 'Created UpdateSystemConfiguration stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating UpdateSystemConfiguration procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create EnrollNewlyRegisteredStudent stored procedure
BEGIN TRY
    IF OBJECT_ID('EnrollNewlyRegisteredStudent', 'P') IS NOT NULL
        DROP PROCEDURE EnrollNewlyRegisteredStudent;
        
    PRINT 'Creating EnrollNewlyRegisteredStudent procedure...';
    
    EXEC('
    CREATE PROCEDURE EnrollNewlyRegisteredStudent
        @user_id INT,
        @student_id VARCHAR(20),
        @program_id INT,
        @year_level_id INT,
        @section_id INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @student_profile_exists BIT = 0;
        DECLARE @error_message NVARCHAR(500) = NULL;
        
        -- Check if auto-enrollment is enabled
        DECLARE @auto_enroll_enabled BIT = 0;
        DECLARE @current_school_year_id INT;
        DECLARE @current_semester_id INT;
        
        SELECT TOP 1
            @auto_enroll_enabled = auto_enroll_enabled,
            @current_school_year_id = current_school_year_id,
            @current_semester_id = current_semester_id
        FROM system_configuration;
        
        -- If no system_configuration found, use reasonable defaults
        IF @current_school_year_id IS NULL
        BEGIN
            SELECT TOP 1 @current_school_year_id = school_year_id 
            FROM school_years 
            WHERE is_current = 1 
            ORDER BY school_year_id DESC;
            
            IF @current_school_year_id IS NULL
                SELECT TOP 1 @current_school_year_id = school_year_id 
                FROM school_years 
                ORDER BY school_year_id DESC;
        END
        
        IF @current_semester_id IS NULL
        BEGIN
            SELECT TOP 1 @current_semester_id = semester_id 
            FROM semesters 
            WHERE semester_code = ''1ST'';
            
            IF @current_semester_id IS NULL
                SELECT TOP 1 @current_semester_id = semester_id 
                FROM semesters;
        END
        
        -- Default auto-enroll to enabled if not found
        IF @auto_enroll_enabled IS NULL SET @auto_enroll_enabled = 1;
        
        -- Check if student profile already exists
        IF EXISTS (SELECT 1 FROM student_profiles WHERE user_id = @user_id)
        BEGIN
            SET @student_profile_exists = 1;
        END
        ELSE
        BEGIN
            -- Create student profile if it doesn''t exist
            BEGIN TRY
                INSERT INTO student_profiles (
                    user_id, student_id, program_id, year_level_id, section_id,
                    school_year_id, enrollment_date, student_status, academic_status
                )
                VALUES (
                    @user_id, @student_id, @program_id, @year_level_id, @section_id,
                    @current_school_year_id, GETDATE(), ''regular'', ''active''
                );
                
                -- Update the user record too
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = ''users'' AND COLUMN_NAME = ''school_year_id'')
                AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                         WHERE TABLE_NAME = ''users'' AND COLUMN_NAME = ''enrollment_date'')
                BEGIN
                    UPDATE users
                    SET 
                        school_year_id = @current_school_year_id,
                        enrollment_date = GETDATE()
                    WHERE user_id = @user_id;
                END
                
            END TRY
            BEGIN CATCH
                SET @error_message = ''Failed to create student profile: '' + ERROR_MESSAGE();
                
                SELECT
                    0 AS success,
                    0 AS student_profile_created,
                    0 AS subjects_enrolled,
                    @error_message AS status;
                
                RETURN;
            END CATCH
        END
        
        -- If auto-enrollment is enabled, enroll in default subjects
        IF @auto_enroll_enabled = 1
        BEGIN
            DECLARE @subjects_enrolled INT = 0;
            DECLARE @enrollment_status NVARCHAR(500);
            
            -- Create a table variable to capture result
            DECLARE @EnrollmentResult TABLE (
                subjects_enrolled INT,
                status NVARCHAR(500)
            );
            
            -- Call enrollment procedure and capture results
            BEGIN TRY
                INSERT INTO @EnrollmentResult
                EXEC EnrollStudentInDefaultSubjects 
                    @student_user_id = @user_id,
                    @specific_school_year_id = @current_school_year_id,
                    @specific_semester_id = @current_semester_id;
                    
                -- Get results
                SELECT 
                    @subjects_enrolled = subjects_enrolled,
                    @enrollment_status = status
                FROM @EnrollmentResult;
                
                -- Return results
                SELECT
                    1 AS success,
                    CASE WHEN @student_profile_exists = 1 THEN 0 ELSE 1 END AS student_profile_created,
                    @subjects_enrolled AS subjects_enrolled,
                    CASE 
                        WHEN @student_profile_exists = 1 THEN ''Student profile already exists. '' 
                        ELSE ''Student profile created successfully. '' 
                    END + @enrollment_status AS status;
            END TRY
            BEGIN CATCH
                SELECT
                    0 AS success,
                    CASE WHEN @student_profile_exists = 1 THEN 0 ELSE 1 END AS student_profile_created,
                    0 AS subjects_enrolled,
                    CASE 
                        WHEN @student_profile_exists = 1 THEN ''Student profile already exists. '' 
                        ELSE ''Student profile created successfully. '' 
                    END + ''Error during auto-enrollment: '' + ERROR_MESSAGE() AS status;
            END CATCH
        END
        ELSE
        BEGIN
            -- Return results without enrollment
            SELECT
                1 AS success,
                CASE WHEN @student_profile_exists = 1 THEN 0 ELSE 1 END AS student_profile_created,
                0 AS subjects_enrolled,
                CASE 
                    WHEN @student_profile_exists = 1 THEN ''Student profile already exists. '' 
                    ELSE ''Student profile created successfully. '' 
                END + ''Auto-enrollment is disabled.'' AS status;
        END
    END
    ');
    
    PRINT 'Created EnrollNewlyRegisteredStudent stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating EnrollNewlyRegisteredStudent procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== VIEWS ===========

-- Create vw_default_subjects view
BEGIN TRY
    IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_default_subjects')
    BEGIN
        DROP VIEW vw_default_subjects;
        PRINT 'Dropped existing vw_default_subjects view.';
    END
    
    PRINT 'Creating vw_default_subjects view...';
    
    EXEC('
    CREATE VIEW vw_default_subjects AS
    SELECT 
        c.curriculum_id,
        p.program_id,
        p.program_name,
        p.program_code,
        yl.year_level_id,
        yl.year_name,
        sem.semester_id,
        sem.semester_name,
        sem.semester_code,
        s.subject_id,
        s.subject_code,
        s.subject_name,
        s.lecture_units,
        s.lab_units,
        (s.lecture_units + s.lab_units) AS total_units,
        c.faculty_id,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '''' THEN 
                u.first_name + '' '' + u.last_name
            ELSE 
                u.first_name + '' '' + LEFT(u.middle_name, 1) + ''. '' + u.last_name
        END AS faculty_name,
        c.curriculum_year,
        c.is_default,
        c.is_active
    FROM 
        curriculum c
    JOIN programs p ON c.program_id = p.program_id
    JOIN year_levels yl ON c.year_level_id = yl.year_level_id
    JOIN semesters sem ON c.semester_id = sem.semester_id
    JOIN subjects s ON c.subject_id = s.subject_id
    LEFT JOIN users u ON c.faculty_id = u.user_id
    WHERE 
        c.is_default = 1
        AND c.is_active = 1
        AND s.is_active = 1
        AND p.is_active = 1
        AND yl.is_active = 1;
    ');
    
    PRINT 'Created vw_default_subjects view.';
END TRY
BEGIN CATCH
    PRINT 'Error creating vw_default_subjects view: ' + ERROR_MESSAGE();
END CATCH
GO


-- Create vw_current_enrollments view
BEGIN TRY
    IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_current_enrollments')
    BEGIN
        DROP VIEW vw_current_enrollments;
        PRINT 'Dropped existing vw_current_enrollments view.';
    END
    
    PRINT 'Creating vw_current_enrollments view...';
    
    EXEC('
    CREATE VIEW vw_current_enrollments AS
    SELECT 
        e.enrollment_id,
        u.user_id AS student_id,
        u.first_name,
        u.middle_name,
        u.last_name,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '''' THEN 
                u.first_name + '' '' + u.last_name
            ELSE 
                u.first_name + '' '' + LEFT(u.middle_name, 1) + ''. '' + u.last_name
        END AS student_name,
        sp.student_id AS student_code,
        p.program_id,
        p.program_name,
        p.program_code,
        yl.year_level_id,
        yl.year_name,
        sec.section_id,
        sec.section_name,
        sy.school_year_id,
        sy.year_name AS school_year,
        sem.semester_id,
        sem.semester_name,
        sem.semester_code,
        s.subject_id,
        s.subject_code,
        s.subject_name,
        s.lecture_units,
        s.lab_units,
        (s.lecture_units + s.lab_units) AS total_units,
        so.offering_id,
        so.class_code,
        e.grade,
        e.enrollment_date
    FROM 
        enrollments e
    JOIN users u ON e.student_id = u.user_id
    JOIN student_profiles sp ON u.user_id = sp.user_id
    JOIN programs p ON sp.program_id = p.program_id
    JOIN year_levels yl ON sp.year_level_id = yl.year_level_id
    LEFT JOIN sections sec ON sp.section_id = sec.section_id
    JOIN subject_offerings so ON e.offering_id = so.offering_id
    JOIN subjects s ON so.subject_id = s.subject_id
    JOIN school_years sy ON so.school_year_id = sy.school_year_id
    JOIN semesters sem ON so.semester_id = sem.semester_id;
    ');
    
    PRINT 'Created vw_current_enrollments view.';
END TRY
BEGIN CATCH
    PRINT 'Error creating vw_current_enrollments view: ' + ERROR_MESSAGE();
END CATCH
GO

-- Summary of schema updates
PRINT '=================================================';
PRINT 'FIXED Schema V8.2 installation complete';
PRINT 'Key changes in this version:';
PRINT '- Added missing columns to programs table';
PRINT '- Added missing columns to subjects table';
PRINT '- Created missing sections table';
PRINT '- Added system_configuration table for central config';
PRINT '- Added is_default flag to curriculum for default subjects';
PRINT '- Added enrollment_history to track auto-enrollment actions';
PRINT '- Created procedures for automatic enrollment in default subjects';
PRINT '- Created procedures for year level promotion';
PRINT '- Added views for default subjects and current enrollments';
PRINT '- faculty_profiles table added';
PRINT '=================================================';
