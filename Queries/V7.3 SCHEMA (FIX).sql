/*
Classroom Management System - Schema Enhancement
Version: 7.3.0
Date: 2025-04-08
Description: Adds missing columns for Curriculum Manager functionality
*/

-- Check if we need to apply this version
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '7.3.0')
BEGIN
    -- Insert version record
    INSERT INTO schema_version (version_number, description) 
    VALUES ('7.3.0', 'Added missing columns needed for Curriculum Manager functionality');
    
    -- Add missing columns to Programs table
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'programs')
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
    ELSE
    BEGIN
        PRINT 'ERROR: programs table does not exist.';
    END
    
    -- Add is_active to Year Levels table
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'year_levels')
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'year_levels' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE year_levels
            ADD is_active BIT NOT NULL DEFAULT 1;
            PRINT 'Added is_active column to year_levels table.';
        END
    END
    ELSE
    BEGIN
        PRINT 'ERROR: year_levels table does not exist.';
    END
    
    -- Add lecture_units and lab_units columns to subjects table
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
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
    END
    ELSE
    BEGIN
        PRINT 'ERROR: subjects table does not exist.';
    END
    
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
            CONSTRAINT FK_curriculum_year_level FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id)
        );
        PRINT 'Created curriculum table.';
        
        -- Add the semester_id foreign key if semesters table exists
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'semesters')
        BEGIN
            ALTER TABLE curriculum
            ADD CONSTRAINT FK_curriculum_semester FOREIGN KEY (semester_id) REFERENCES semesters (semester_id);
            PRINT 'Added semester_id foreign key to curriculum table.';
        END
        
        -- Add the faculty_id foreign key if users table exists
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
        BEGIN
            ALTER TABLE curriculum
            ADD CONSTRAINT FK_curriculum_faculty FOREIGN KEY (faculty_id) REFERENCES users (user_id);
            PRINT 'Added faculty_id foreign key to curriculum table.';
        END
    END
    ELSE
    BEGIN
        -- Check if curriculum table is missing any columns
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'is_active')
        BEGIN
            ALTER TABLE curriculum
            ADD is_active BIT DEFAULT 1;
            PRINT 'Added is_active column to curriculum table.';
        END
        
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'subject_status')
        BEGIN
            ALTER TABLE curriculum
            ADD subject_status VARCHAR(20) DEFAULT 'active';
            PRINT 'Added subject_status column to curriculum table.';
        END
        
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'curriculum_year')
        BEGIN
            ALTER TABLE curriculum
            ADD curriculum_year VARCHAR(9) DEFAULT '2024-2025';
            PRINT 'Added curriculum_year column to curriculum table.';
        END
    END
    
    -- Create the semesters table if it doesn't exist
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
        INSERT INTO semesters (semester_name, semester_code) 
        VALUES ('First Semester', '1ST'), 
               ('Second Semester', '2ND'), 
               ('Summer', 'SUM');
               
        PRINT 'Created semesters table and added default values.';
        
        -- If curriculum table already exists but without FK to semesters, add it
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'curriculum')
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
                          WHERE CONSTRAINT_NAME = 'FK_curriculum_semester')
            BEGIN
                ALTER TABLE curriculum
                ADD CONSTRAINT FK_curriculum_semester FOREIGN KEY (semester_id) REFERENCES semesters (semester_id);
                PRINT 'Added semester_id foreign key to existing curriculum table.';
            END
        END
    END
    
    -- Summary of schema updates
    PRINT '=================================================';
    PRINT 'Schema V7.3.0 installation complete';
    PRINT 'Key changes in this version:';
    PRINT '- Added to programs table:';
    PRINT '  * is_active (BIT, default 1)';
    PRINT '  * program_code (VARCHAR(20))';
    PRINT '- Added to year_levels table:';
    PRINT '  * is_active (BIT, default 1)';
    PRINT '- Added to subjects table:';
    PRINT '  * lecture_units (INT, default 0)';
    PRINT '  * lab_units (INT, default 0)';
    PRINT '- Created curriculum table (if not existing)';
    PRINT '- Created semesters table (if not existing)';
    PRINT '=================================================';
END
ELSE
BEGIN
    PRINT 'Schema V7.3.0 is already installed.';
END
