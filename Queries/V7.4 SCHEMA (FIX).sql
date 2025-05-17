/*
Classroom Management System - Schema Enhancement
Version: 7.3.0
Date: 2025-04-08
Description: Adds missing columns for Curriculum Manager functionality (Database3.mdf)
*/

-- Create schema_version table if it doesn't exist
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

-- Check if we need to apply this version
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '7.3.0')
BEGIN
    -- Insert version record
    INSERT INTO schema_version (version_number, description) 
    VALUES ('7.3.0', 'Added missing columns needed for Curriculum Manager functionality');
    
    -- ============ PROGRAMS TABLE ============
    -- Create programs table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'programs')
    BEGIN
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
        ('BS Information Technology', 'BSIT', 'Bachelor of Science in Information Technology', 1),
        ('BS Information System', 'BSIS', 'Bachelor of Science in Information System', 1),
        ('BS Entertainment and Multimedia Computing', 'BSEMC', 'Bachelor of Science in Entertainment and Multimedia Computing', 1);
        
        PRINT 'Created programs table with default values.';
    END
    ELSE
    BEGIN
        -- Add is_active column
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE programs
            ADD is_active BIT NOT NULL DEFAULT 1;
            PRINT 'Added is_active column to programs table.';
        END
        
        -- Add program_code column
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'program_code')
        BEGIN
            ALTER TABLE programs
            ADD program_code VARCHAR(20) NULL;
            PRINT 'Added program_code column to programs table.';
            
            -- Update existing programs with default codes
            UPDATE programs SET program_code = 'BSCS' WHERE program_name LIKE '%Computer Science%';
            UPDATE programs SET program_code = 'BSIT' WHERE program_name LIKE '%Information Technology%';
            UPDATE programs SET program_code = 'BSIS' WHERE program_name LIKE '%Information System%';
            UPDATE programs SET program_code = 'BSEMC' WHERE program_name LIKE '%Entertainment and Multimedia Computing%';
            PRINT 'Updated program_code values for existing programs.';
        END
    END
    
    -- ============ YEAR LEVELS TABLE ============
    -- Create year_levels table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'year_levels')
    BEGIN
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
        ('Year 1', 1), 
        ('Year 2', 1), 
        ('Year 3', 1), 
        ('Year 4', 1), 
        ('Year 5', 1);
        
        PRINT 'Created year_levels table with default values.';
    END
    ELSE
    BEGIN
        -- Add is_active column
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'year_levels' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE year_levels
            ADD is_active BIT NOT NULL DEFAULT 1;
            PRINT 'Added is_active column to year_levels table.';
        END
    END
    
    -- ============ SUBJECTS TABLE ============
    -- Create subjects table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
    BEGIN
        CREATE TABLE subjects (
            subject_id INT IDENTITY(1,1) PRIMARY KEY,
            subject_code VARCHAR(20) NOT NULL UNIQUE,
            subject_name VARCHAR(100) NOT NULL,
            description VARCHAR(500) NULL,
            units INT NOT NULL DEFAULT 3,
            lecture_units INT NOT NULL DEFAULT 3,
            lab_units INT NOT NULL DEFAULT 0,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert some sample subjects
        INSERT INTO subjects (subject_code, subject_name, units, lecture_units, lab_units, is_active) VALUES 
        ('CS101', 'Introduction to Computer Science', 3, 3, 0, 1),
        ('CS102', 'Programming Fundamentals', 3, 2, 1, 1),
        ('CS201', 'Data Structures and Algorithms', 3, 2, 1, 1),
        ('IT101', 'Information Technology Concepts', 3, 3, 0, 1);
        
        PRINT 'Created subjects table with sample data.';
    END
    ELSE
    BEGIN
        -- Add lecture_units column
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lecture_units')
        BEGIN
            ALTER TABLE subjects
            ADD lecture_units INT NOT NULL DEFAULT 0;
            PRINT 'Added lecture_units column to subjects table.';
        END
        
        -- Add lab_units column
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lab_units')
        BEGIN
            ALTER TABLE subjects
            ADD lab_units INT NOT NULL DEFAULT 0;
            PRINT 'Added lab_units column to subjects table.';
        END
        
        -- If there's a units column, update lecture_units based on it
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'units')
        BEGIN
            UPDATE subjects 
            SET lecture_units = units, lab_units = 0 
            WHERE lecture_units = 0 AND lab_units = 0;
            PRINT 'Updated existing subjects with default lecture_units and lab_units values.';
        END
        
        -- Add is_active column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE subjects
            ADD is_active BIT NOT NULL DEFAULT 1;
            PRINT 'Added is_active column to subjects table.';
        END
    END
    
    -- ============ SCHOOL YEARS TABLE ============
    -- Create school_years table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
    BEGIN
        CREATE TABLE school_years (
            school_year_id INT IDENTITY(1,1) PRIMARY KEY,
            year_name VARCHAR(20) NOT NULL,
            is_current BIT DEFAULT 0,
            start_date DATE NULL,
            end_date DATE NULL,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert default school years
        INSERT INTO school_years (year_name, is_current, start_date, end_date) VALUES 
        ('2023-2024', 0, '2023-06-01', '2024-03-31'),
        ('2024-2025', 1, '2024-06-01', '2025-03-31'),
        ('2025-2026', 0, '2025-06-01', '2026-03-31');
        
        PRINT 'Created school_years table with default values.';
    END
    
    -- ============ SEMESTERS TABLE ============
    -- Create semesters table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'semesters')
    BEGIN
        CREATE TABLE semesters (
            semester_id INT IDENTITY(1,1) PRIMARY KEY,
            semester_name VARCHAR(50) NOT NULL, -- 'First Semester', 'Second Semester', 'Summer'
            semester_code VARCHAR(5) NOT NULL, -- '1ST', '2ND', 'SUM'
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL
        );
        
        -- Insert default semesters
        INSERT INTO semesters (semester_name, semester_code) VALUES 
        ('First Semester', '1ST'), 
        ('Second Semester', '2ND'), 
        ('Summer', 'SUM');
               
        PRINT 'Created semesters table with default values.';
    END
    
    -- ============ FACULTY PROFILES TABLE ============
    -- Create faculty_profiles table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'faculty_profiles')
    BEGIN
        -- First ensure users table exists
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
        BEGIN
            CREATE TABLE users (
                user_id INT IDENTITY(1,1) PRIMARY KEY,
                username VARCHAR(50) UNIQUE NOT NULL,
                password_hash VARCHAR(255) NOT NULL,
                email VARCHAR(100) UNIQUE NOT NULL,
                first_name VARCHAR(50) NOT NULL,
                middle_name VARCHAR(50) NULL,
                last_name VARCHAR(50) NOT NULL,
                user_type VARCHAR(20) NOT NULL,
                is_archived BIT DEFAULT 0,
                created_at DATETIME DEFAULT GETDATE(),
                updated_at DATETIME NULL
            );
            PRINT 'Created users table.';
        END
        
        CREATE TABLE faculty_profiles (
            user_id INT PRIMARY KEY,
            faculty_id VARCHAR(20) UNIQUE NOT NULL,
            department VARCHAR(100) NOT NULL,
            position VARCHAR(100) NULL,
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (user_id) REFERENCES users (user_id)
        );
        
        PRINT 'Created faculty_profiles table.';
    END
    
    -- ============ CURRICULUM TABLE ============
    -- Create curriculum table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'curriculum')
    BEGIN
        CREATE TABLE curriculum (
            curriculum_id INT IDENTITY(1,1) PRIMARY KEY,
            program_id INT NOT NULL,
            subject_id INT NOT NULL,
            school_year_id INT NOT NULL,
            semester_id INT NOT NULL,
            year_level_id INT NOT NULL,
            curriculum_year VARCHAR(9) NOT NULL, -- Example: '2024-2025'
            subject_status VARCHAR(20) DEFAULT 'active', -- 'active', 'inactive', 'deprecated'
            faculty_id INT NULL, -- Assigned professor (optional)
            is_active BIT DEFAULT 1,
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            CONSTRAINT FK_curriculum_program FOREIGN KEY (program_id) REFERENCES programs (program_id),
            CONSTRAINT FK_curriculum_subject FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
            CONSTRAINT FK_curriculum_school_year FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id),
            CONSTRAINT FK_curriculum_semester FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
            CONSTRAINT FK_curriculum_year_level FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id)
        );
        
        -- Add faculty_id foreign key if users table exists
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
        BEGIN
            ALTER TABLE curriculum
            ADD CONSTRAINT FK_curriculum_faculty FOREIGN KEY (faculty_id) REFERENCES users (user_id);
        END
        
        PRINT 'Created curriculum table with all necessary constraints.';
    END
    
    -- Summary of schema updates
    PRINT '=================================================';
    PRINT 'Schema V7.4.0 installation complete';
    PRINT 'Key changes in this version:';
    PRINT '- Created or updated programs table with program_code and is_active';
    PRINT '- Created or updated year_levels table with is_active';
    PRINT '- Created or updated subjects table with lecture_units and lab_units';
    PRINT '- Created school_years table if needed';
    PRINT '- Created semesters table if needed';
    PRINT '- Created faculty_profiles table if needed';
    PRINT '- Created curriculum table with all required relationships';
    PRINT '=================================================';
END
ELSE
BEGIN
    PRINT 'Schema V7.4.0 is already installed.';
END
