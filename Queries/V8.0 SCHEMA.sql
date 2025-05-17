/*
Classroom Management System - Combined Complete Schema
Version: 7.0 - 7.4 (Combined)
Date: 2025-04-08
Description: Comprehensive schema file combining all fixes and enhancements from v7.0-7.4
*/

-- Create a versioning table if it doesn't exist
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

-- Insert combined version record
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '7.4.0')
BEGIN
    INSERT INTO schema_version (version_number, description) 
    VALUES ('7.4.0', 'Combined schema including all fixes from v7.0-7.4');
    PRINT 'Added schema version 7.4.0 record.';
END

-- =========== USER MANAGEMENT TABLES ===========

-- Create users table if not exists
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
        user_type VARCHAR(20) NOT NULL, -- 'Admin', 'Faculty', 'Student'
        birthday DATE NULL,             -- Added in 7.1.0
        sex VARCHAR(10) NULL,           -- Added in 7.1.0
        address VARCHAR(500) NULL,      -- Added in 7.1.0
        contact_number VARCHAR(20) NULL, -- Added in 7.1.0
        enrollment_date DATE NULL,      -- Added in 7.1.0
        school_year_id INT NULL,        -- Added in 7.1.0
        is_archived BIT DEFAULT 0,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL
    );
    PRINT 'Created users table with all columns.';
END
ELSE
BEGIN
    -- Add missing columns from 7.1.0 to users table
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'birthday')
    BEGIN
        ALTER TABLE users ADD birthday DATE NULL;
        PRINT 'Added birthday column to users table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'sex')
    BEGIN
        ALTER TABLE users ADD sex VARCHAR(10) NULL;
        PRINT 'Added sex column to users table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'address')
    BEGIN
        ALTER TABLE users ADD address VARCHAR(500) NULL;
        PRINT 'Added address column to users table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'contact_number')
    BEGIN
        ALTER TABLE users ADD contact_number VARCHAR(20) NULL;
        PRINT 'Added contact_number column to users table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'enrollment_date')
    BEGIN
        ALTER TABLE users ADD enrollment_date DATE NULL;
        PRINT 'Added enrollment_date column to users table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'school_year_id')
    BEGIN
        ALTER TABLE users ADD school_year_id INT NULL;
        PRINT 'Added school_year_id column to users table.';
    END
END

-- =========== ACADEMIC STRUCTURE TABLES ===========

-- Create programs table if not exists
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
    -- Add is_active column if it doesn't exist (from 7.3.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'is_active')
    BEGIN
        ALTER TABLE programs ADD is_active BIT NOT NULL DEFAULT 1;
        PRINT 'Added is_active column to programs table.';
    END
    
    -- Add program_code column if it doesn't exist (from 7.3.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'programs' AND COLUMN_NAME = 'program_code')
    BEGIN
        ALTER TABLE programs ADD program_code VARCHAR(20) NULL;
        
        -- Update existing programs with default codes
        UPDATE programs SET program_code = 'BSCS' WHERE program_name LIKE '%Computer Science%';
        UPDATE programs SET program_code = 'BSIT' WHERE program_name LIKE '%Information Technology%';
        UPDATE programs SET program_code = 'BSIS' WHERE program_name LIKE '%Information System%';
        UPDATE programs SET program_code = 'BSEMC' WHERE program_name LIKE '%Entertainment and Multimedia Computing%';
        
        PRINT 'Added program_code column and updated existing program codes.';
    END
END

-- Create year_levels table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'year_levels')
BEGIN
    CREATE TABLE year_levels (
        year_level_id INT IDENTITY(1,1) PRIMARY KEY,
        year_name VARCHAR(20) NOT NULL, -- 'Year 1', 'Year 2', etc.
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
    -- Add is_active column if it doesn't exist (from 7.3.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'year_levels' AND COLUMN_NAME = 'is_active')
    BEGIN
        ALTER TABLE year_levels ADD is_active BIT NOT NULL DEFAULT 1;
        PRINT 'Added is_active column to year_levels table.';
    END
    
    -- Add description column if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'year_levels' AND COLUMN_NAME = 'description')
    BEGIN
        ALTER TABLE year_levels ADD description VARCHAR(500) NULL;
        PRINT 'Added description column to year_levels table.';
    END
END

-- Create sections table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'sections')
BEGIN
    CREATE TABLE sections (
        section_id INT IDENTITY(1,1) PRIMARY KEY,
        section_name VARCHAR(20) NOT NULL, -- 'A', 'B', 'C', etc.
        is_active BIT DEFAULT 1,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL
    );
    PRINT 'Created sections table.';
END

-- Create school_years table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
BEGIN
    CREATE TABLE school_years (
        school_year_id INT IDENTITY(1,1) PRIMARY KEY,
        year_name VARCHAR(20) NOT NULL, -- '2024-2025'
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

-- Create semesters table if not exists
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

-- =========== PROFILE TABLES ===========

-- Create student_profiles table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'student_profiles')
BEGIN
    CREATE TABLE student_profiles (
        user_id INT PRIMARY KEY,
        student_id VARCHAR(20) UNIQUE NOT NULL,
        program_id INT NOT NULL,
        year_level_id INT NOT NULL,
        section_id INT NULL,
        school_year_id INT NULL,         -- Added in 7.1.0/7.2.0
        enrollment_date DATE DEFAULT GETDATE(), -- Added in 7.1.0/7.2.0
        student_status VARCHAR(20) DEFAULT 'regular', -- 'regular', 'irregular'
        academic_status VARCHAR(20) DEFAULT 'active', -- 'active', 'on_leave', 'graduated', 'dismissed'
        admission_date DATE DEFAULT GETDATE(),
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        FOREIGN KEY (user_id) REFERENCES users (user_id),
        FOREIGN KEY (program_id) REFERENCES programs (program_id),
        FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
        FOREIGN KEY (section_id) REFERENCES sections (section_id)
    );
    PRINT 'Created student_profiles table with all columns.';
    
    -- Add foreign key for school_year_id if school_years table exists
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
    BEGIN
        ALTER TABLE student_profiles
        ADD CONSTRAINT FK_student_profiles_school_year FOREIGN KEY (school_year_id) 
        REFERENCES school_years (school_year_id);
        PRINT 'Added school_year_id foreign key to student_profiles table.';
    END
END
ELSE
BEGIN
    -- Add school_year_id column if it doesn't exist (from 7.1.0/7.2.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'student_profiles' AND COLUMN_NAME = 'school_year_id')
    BEGIN
        ALTER TABLE student_profiles ADD school_year_id INT NULL;
        
        -- Add foreign key if school_years table exists
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
        BEGIN
            ALTER TABLE student_profiles
            ADD CONSTRAINT FK_student_profiles_school_year FOREIGN KEY (school_year_id) 
            REFERENCES school_years (school_year_id);
        END
        
        PRINT 'Added school_year_id column and foreign key to student_profiles table.';
    END
    
    -- Add enrollment_date column if it doesn't exist (from 7.1.0/7.2.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'student_profiles' AND COLUMN_NAME = 'enrollment_date')
    BEGIN
        ALTER TABLE student_profiles ADD enrollment_date DATE DEFAULT GETDATE();
        PRINT 'Added enrollment_date column to student_profiles table.';
    END
END

-- Create faculty_profiles table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'faculty_profiles')
BEGIN
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

-- =========== CURRICULUM AND SUBJECT TABLES ===========

-- Create subjects table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
BEGIN
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
    ('CS102', 'Programming Fundamentals', 3, 2, 1, 1),
    ('CS201', 'Data Structures and Algorithms', 3, 2, 1, 1),
    ('IT101', 'Information Technology Concepts', 3, 3, 0, 1);
    
    PRINT 'Created subjects table with sample data.';
END
ELSE
BEGIN
    -- Add lecture_units column if it doesn't exist (from 7.3.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lecture_units')
    BEGIN
        ALTER TABLE subjects ADD lecture_units INT NOT NULL DEFAULT 0;
        PRINT 'Added lecture_units column to subjects table.';
    END
    
    -- Add lab_units column if it doesn't exist (from 7.3.0)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lab_units')
    BEGIN
        ALTER TABLE subjects ADD lab_units INT NOT NULL DEFAULT 0;
        PRINT 'Added lab_units column to subjects table.';
    END
    
    -- Add is_active column if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'is_active')
    BEGIN
        ALTER TABLE subjects ADD is_active BIT NOT NULL DEFAULT 1;
        PRINT 'Added is_active column to subjects table.';
    END
    
    -- If lecture_units and lab_units are zero but units exists, update them
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'units')
    BEGIN
        UPDATE subjects 
        SET lecture_units = units, lab_units = 0 
        WHERE lecture_units = 0 AND lab_units = 0;
        PRINT 'Updated existing subjects with default lecture_units and lab_units values.';
    END
END

-- Create subject_prerequisites table if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_prerequisites')
BEGIN
    CREATE TABLE subject_prerequisites (
        prerequisite_id INT IDENTITY(1,1) PRIMARY KEY,
        subject_id INT NOT NULL,
        prerequisite_subject_id INT NOT NULL,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        CONSTRAINT FK_prerequisite_subject FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
        CONSTRAINT FK_prerequisite_required FOREIGN KEY (prerequisite_subject_id) REFERENCES subjects (subject_id),
        CONSTRAINT UK_subject_prerequisite UNIQUE (subject_id, prerequisite_subject_id)
    );
    PRINT 'Created subject_prerequisites table.';
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
        CONSTRAINT FK_curriculum_semester FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
        CONSTRAINT FK_curriculum_year_level FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
        CONSTRAINT FK_curriculum_faculty FOREIGN KEY (faculty_id) REFERENCES users (user_id),
        -- Make sure a subject appears only once in a curriculum for a specific program/year/semester
        CONSTRAINT UK_curriculum UNIQUE (program_id, subject_id, school_year_id, semester_id, curriculum_year)
    );
    PRINT 'Created curriculum table with all necessary constraints.';
END
ELSE
BEGIN
    -- Add missing columns to curriculum table if needed
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'is_active')
    BEGIN
        ALTER TABLE curriculum ADD is_active BIT DEFAULT 1;
        PRINT 'Added is_active column to curriculum table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'subject_status')
    BEGIN
        ALTER TABLE curriculum ADD subject_status VARCHAR(20) DEFAULT 'active';
        PRINT 'Added subject_status column to curriculum table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'curriculum_year')
    BEGIN
        ALTER TABLE curriculum ADD curriculum_year VARCHAR(9) DEFAULT '2024-2025';
        PRINT 'Added curriculum_year column to curriculum table.';
    END
    
    -- Make sure foreign keys exist
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_curriculum_semester')
    BEGIN
        ALTER TABLE curriculum
        ADD CONSTRAINT FK_curriculum_semester FOREIGN KEY (semester_id) REFERENCES semesters (semester_id);
        PRINT 'Added semester_id foreign key to curriculum table.';
    END
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_curriculum_faculty')
    BEGIN
        ALTER TABLE curriculum
        ADD CONSTRAINT FK_curriculum_faculty FOREIGN KEY (faculty_id) REFERENCES users (user_id);
        PRINT 'Added faculty_id foreign key to curriculum table.';
    END
END

-- =========== CLASS OFFERING TABLES ===========

-- Create subject_offerings table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_offerings')
BEGIN
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
ELSE IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'subject_offerings' 
    AND COLUMN_NAME = 'is_active'
)
BEGIN
    -- Add the is_active column if the table exists but column doesn't
    ALTER TABLE subject_offerings
    ADD is_active BIT NOT NULL DEFAULT 1;
    PRINT 'Added is_active column to existing subject_offerings table.';
END

-- Create subject_teachers table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_teachers')
BEGIN
    CREATE TABLE subject_teachers (
        subject_teacher_id INT IDENTITY(1,1) PRIMARY KEY,
        offering_id INT NOT NULL,
        faculty_id INT NOT NULL,
        is_primary BIT DEFAULT 1,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        FOREIGN KEY (offering_id) REFERENCES subject_offerings (offering_id),
        FOREIGN KEY (faculty_id) REFERENCES users (user_id),
        CONSTRAINT UK_subject_teachers UNIQUE (offering_id, faculty_id)
    );
    PRINT 'Created subject_teachers table.';
END

-- Create enrollments table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'enrollments')
BEGIN
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

-- =========== GRADING SYSTEM TABLES ===========

-- Create grade_types table (midterm, final, etc.)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'grade_types')
BEGIN
    CREATE TABLE grade_types (
        grade_type_id INT IDENTITY(1,1) PRIMARY KEY,
        type_name VARCHAR(50) NOT NULL UNIQUE,
        description VARCHAR(255) NULL,
        weight DECIMAL(5,2) DEFAULT 50.00, -- Default weight as percentage (50%)
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL
    );
    
    -- Insert default grade types
    INSERT INTO grade_types (type_name, description, weight) VALUES 
    ('Midterm', 'Midterm examination grade', 40.00),
    ('Final', 'Final examination grade', 60.00);
    
    PRINT 'Created grade_types table with default values.';
END

-- Create grading_periods table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'grading_periods')
BEGIN
    CREATE TABLE grading_periods (
        grading_period_id INT IDENTITY(1,1) PRIMARY KEY,
        period_name VARCHAR(50) NOT NULL,
        school_year_id INT NOT NULL,
        semester_id INT NOT NULL,
        start_date DATE NOT NULL,
        end_date DATE NOT NULL,
        is_active BIT DEFAULT 0,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id),
        FOREIGN KEY (semester_id) REFERENCES semesters (semester_id)
    );
    PRINT 'Created grading_periods table.';
END

-- Create student_grades table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'student_grades')
BEGIN
    CREATE TABLE student_grades (
        grade_id INT IDENTITY(1,1) PRIMARY KEY,
        enrollment_id INT NOT NULL,
        grade_type_id INT NOT NULL,
        score DECIMAL(5,2) NULL, -- Numeric score (e.g., 85.50)
        letter_grade VARCHAR(5) NULL, -- Letter grade (e.g., A, B+, etc.)
        remarks VARCHAR(500) NULL,
        is_published BIT DEFAULT 0 NOT NULL, -- Whether the grade is visible to students
        graded_by INT NOT NULL, -- Faculty user ID
        graded_at DATETIME DEFAULT GETDATE(),
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        FOREIGN KEY (enrollment_id) REFERENCES enrollments (enrollment_id),
        FOREIGN KEY (grade_type_id) REFERENCES grade_types (grade_type_id),
        FOREIGN KEY (graded_by) REFERENCES users (user_id)
    );
    PRINT 'Created student_grades table.';
END

-- Create grade_components table if not exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'grade_components')
BEGIN
    CREATE TABLE grade_components (
        component_id INT IDENTITY(1,1) PRIMARY KEY,
        grade_id INT NOT NULL,
        component_name VARCHAR(100) NOT NULL,
        component_score DECIMAL(5,2) NOT NULL,
        max_score DECIMAL(5,2) NOT NULL,
        weight DECIMAL(5,2) DEFAULT 100.00, -- Weight as percentage
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        FOREIGN KEY (grade_id) REFERENCES student_grades (grade_id)
    );
    PRINT 'Created grade_components table.';
END

-- =========== ADD FOREIGN KEY CONSTRAINTS ===========

-- Add foreign key from users to school_years
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'school_year_id')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_users_school_year')
    BEGIN
        ALTER TABLE users
        ADD CONSTRAINT FK_users_school_year FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id);
        PRINT 'Added foreign key constraint from users.school_year_id to school_years.';
    END
END

GO -- Batch separator for views and stored procedures

-- =========== VIEWS AND PROCEDURES ===========

-- Create or alter the curriculum manager view
IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_curriculum_manager')
BEGIN
    DROP VIEW vw_curriculum_manager;
    PRINT 'Dropped existing vw_curriculum_manager view.';
END
GO

CREATE VIEW vw_curriculum_manager AS
SELECT 
    c.curriculum_id,
    sy.year_name AS school_year,
    p.program_name,
    p.program_code,
    sem.semester_name,
    sem.semester_code,
    s.subject_code,
    s.subject_name,
    s.lecture_units,
    s.lab_units,
    (s.lecture_units + s.lab_units) AS total_units,
    yl.year_name AS year_level,
    c.subject_status,
    c.curriculum_year,
    u.user_id AS professor_id,
    CASE 
        WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
            u.first_name + ' ' + u.last_name
        ELSE 
            u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
    END AS professor_name,
    fp.department AS professor_department,
    c.is_active
FROM 
    curriculum c
JOIN subjects s ON c.subject_id = s.subject_id
JOIN programs p ON c.program_id = p.program_id
JOIN school_years sy ON c.school_year_id = sy.school_year_id
JOIN semesters sem ON c.semester_id = sem.semester_id
JOIN year_levels yl ON c.year_level_id = yl.year_level_id
LEFT JOIN users u ON c.faculty_id = u.user_id
LEFT JOIN faculty_profiles fp ON u.user_id = fp.user_id;
GO
PRINT 'Created vw_curriculum_manager view.';

-- Create or update curriculum management stored procedures

-- Procedure to manage curriculum entries
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'ManageCurriculumEntry')
BEGIN
    DROP PROCEDURE ManageCurriculumEntry;
    PRINT 'Dropped existing ManageCurriculumEntry procedure.';
END
GO

CREATE PROCEDURE ManageCurriculumEntry
    @curriculum_id INT = NULL,
    @program_id INT,
    @subject_id INT,
    @school_year_id INT,
    @semester_id INT,
    @year_level_id INT,
    @curriculum_year VARCHAR(9),
    @subject_status VARCHAR(20) = 'active',
    @faculty_id INT = NULL,
    @is_active BIT = 1,
    @is_delete BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Delete operation
    IF @is_delete = 1 AND @curriculum_id IS NOT NULL
    BEGIN
        DELETE FROM curriculum WHERE curriculum_id = @curriculum_id;
        SELECT 'Curriculum entry deleted successfully.' AS Status;
        RETURN;
    END
    
    -- Update operation
    IF @curriculum_id IS NOT NULL
    BEGIN
        UPDATE curriculum
        SET program_id = @program_id,
            subject_id = @subject_id,
            school_year_id = @school_year_id,
            semester_id = @semester_id,
            year_level_id = @year_level_id,
            curriculum_year = @curriculum_year,
            subject_status = @subject_status,
            faculty_id = @faculty_id,
            is_active = @is_active,
            updated_at = GETDATE()
        WHERE curriculum_id = @curriculum_id;
        
        SELECT 'Curriculum entry updated successfully.' AS Status;
    END
    -- Insert operation
    ELSE
    BEGIN
        -- Check if entry already exists
        IF EXISTS (
            SELECT 1 FROM curriculum 
            WHERE program_id = @program_id 
            AND subject_id = @subject_id 
            AND school_year_id = @school_year_id 
            AND semester_id = @semester_id
            AND curriculum_year = @curriculum_year
        )
        BEGIN
            SELECT 'Entry already exists in curriculum.' AS Status;
            RETURN;
        END
        
        INSERT INTO curriculum (
            program_id, subject_id, school_year_id, semester_id, 
            year_level_id, curriculum_year, subject_status, faculty_id, is_active
        )
        VALUES (
            @program_id, @subject_id, @school_year_id, @semester_id,
            @year_level_id, @curriculum_year, @subject_status, @faculty_id, @is_active
        );
        
        SELECT SCOPE_IDENTITY() AS curriculum_id, 'Curriculum entry created successfully.' AS Status;
    END
END
GO
PRINT 'Created ManageCurriculumEntry stored procedure.';

-- Procedure to get available subjects for curriculum assignment
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetAvailableSubjectsForCurriculum')
BEGIN
    DROP PROCEDURE GetAvailableSubjectsForCurriculum;
    PRINT 'Dropped existing GetAvailableSubjectsForCurriculum procedure.';
END
GO

CREATE PROCEDURE GetAvailableSubjectsForCurriculum
    @program_id INT,
    @school_year_id INT,
    @semester_id INT,
    @curriculum_year VARCHAR(9)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.subject_id,
        s.subject_code,
        s.subject_name,
        s.description,
        s.lecture_units,
        s.lab_units,
        (s.lecture_units + s.lab_units) AS total_units,
        s.is_active
    FROM subjects s
    WHERE s.is_active = 1
    AND s.subject_id NOT IN (
        SELECT c.subject_id
        FROM curriculum c
        WHERE c.program_id = @program_id
        AND c.school_year_id = @school_year_id
        AND c.semester_id = @semester_id
        AND c.curriculum_year = @curriculum_year
    )
    ORDER BY s.subject_code;
END
GO
PRINT 'Created GetAvailableSubjectsForCurriculum procedure.';

-- Procedure to get available faculty
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetAvailableFaculty')
BEGIN
    DROP PROCEDURE GetAvailableFaculty;
    PRINT 'Dropped existing GetAvailableFaculty procedure.';
END
GO

CREATE PROCEDURE GetAvailableFaculty
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.user_id AS faculty_id,
        u.first_name,
        u.middle_name,
        u.last_name,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                u.first_name + ' ' + u.last_name
            ELSE 
                u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
        END AS faculty_name,
        fp.faculty_id AS faculty_code,
        fp.department,
        fp.position
    FROM users u
    JOIN faculty_profiles fp ON u.user_id = fp.user_id
    WHERE u.user_type = 'Faculty'
    AND fp.is_active = 1
    AND (u.is_archived = 0 OR u.is_archived IS NULL)
    ORDER BY u.last_name, u.first_name;
END
GO
PRINT 'Created GetAvailableFaculty procedure.';

-- Procedure to get curriculum by program
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetCurriculumByProgram')
BEGIN
    DROP PROCEDURE GetCurriculumByProgram;
    PRINT 'Dropped existing GetCurriculumByProgram procedure.';
END
GO

CREATE PROCEDURE GetCurriculumByProgram
    @program_id INT,
    @curriculum_year VARCHAR(9) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.curriculum_id,
        sy.year_name AS school_year,
        p.program_name,
        p.program_code,
        sem.semester_name,
        sem.semester_code,
        s.subject_code,
        s.subject_name,
        s.lecture_units,
        s.lab_units,
        (s.lecture_units + s.lab_units) AS total_units,
        yl.year_name AS year_level,
        c.subject_status,
        c.curriculum_year,
        u.user_id AS professor_id,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                u.first_name + ' ' + u.last_name
            ELSE 
                u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
        END AS professor_name,
        fp.department AS professor_department,
        c.is_active
    FROM 
        curriculum c
    JOIN subjects s ON c.subject_id = s.subject_id
    JOIN programs p ON c.program_id = p.program_id
    JOIN school_years sy ON c.school_year_id = sy.school_year_id
    JOIN semesters sem ON c.semester_id = sem.semester_id
    JOIN year_levels yl ON c.year_level_id = yl.year_level_id
    LEFT JOIN users u ON c.faculty_id = u.user_id
    LEFT JOIN faculty_profiles fp ON u.user_id = fp.user_id
    WHERE c.program_id = @program_id
    AND (@curriculum_year IS NULL OR c.curriculum_year = @curriculum_year)
    ORDER BY c.curriculum_year, sy.year_name, sem.semester_id, yl.year_level_id, s.subject_code;
END
GO
PRINT 'Created GetCurriculumByProgram procedure.';

-- Procedure to manage subject prerequisites
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'ManageSubjectPrerequisites')
BEGIN
    DROP PROCEDURE ManageSubjectPrerequisites;
    PRINT 'Dropped existing ManageSubjectPrerequisites procedure.';
END
GO

CREATE PROCEDURE ManageSubjectPrerequisites
    @subject_id INT,
    @prerequisite_subject_ids VARCHAR(MAX), -- Comma-separated list of subject IDs
    @operation VARCHAR(10) = 'SET' -- 'SET' or 'ADD' or 'REMOVE'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Convert the comma-separated string to a table
    DECLARE @PrerequisiteTable TABLE (prerequisite_subject_id INT);
    
    INSERT INTO @PrerequisiteTable
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@prerequisite_subject_ids, ',');
    
    -- For SET operation, remove all existing prerequisites and add the new ones
    IF @operation = 'SET'
    BEGIN
        DELETE FROM subject_prerequisites WHERE subject_id = @subject_id;
        
        INSERT INTO subject_prerequisites (subject_id, prerequisite_subject_id)
        SELECT @subject_id, prerequisite_subject_id 
        FROM @PrerequisiteTable;
        
        SELECT 'Prerequisites set successfully.' AS Status;
    END
    
    -- For ADD operation, add new prerequisites that don't exist yet
    ELSE IF @operation = 'ADD'
    BEGIN
        INSERT INTO subject_prerequisites (subject_id, prerequisite_subject_id)
        SELECT @subject_id, prerequisite_subject_id 
        FROM @PrerequisiteTable
        WHERE prerequisite_subject_id NOT IN (
            SELECT prerequisite_subject_id 
            FROM subject_prerequisites 
            WHERE subject_id = @subject_id
        );
        
        SELECT 'Prerequisites added successfully.' AS Status;
    END
    
    -- For REMOVE operation, remove specified prerequisites
    ELSE IF @operation = 'REMOVE'
    BEGIN
        DELETE FROM subject_prerequisites 
        WHERE subject_id = @subject_id
        AND prerequisite_subject_id IN (
            SELECT prerequisite_subject_id 
            FROM @PrerequisiteTable
        );
        
        SELECT 'Prerequisites removed successfully.' AS Status;
    END
END
GO
PRINT 'Created ManageSubjectPrerequisites procedure.';

-- Procedure to get subject prerequisites
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetSubjectPrerequisites')
BEGIN
    DROP PROCEDURE GetSubjectPrerequisites;
    PRINT 'Dropped existing GetSubjectPrerequisites procedure.';
END
GO

CREATE PROCEDURE GetSubjectPrerequisites
    @subject_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        sp.prerequisite_id,
        sp.subject_id,
        s.subject_code,
        s.subject_name,
        sp.prerequisite_subject_id,
        ps.subject_code AS prerequisite_code,
        ps.subject_name AS prerequisite_name,
        ps.lecture_units AS prerequisite_lecture_units,
        ps.lab_units AS prerequisite_lab_units,
        (ps.lecture_units + ps.lab_units) AS prerequisite_total_units
    FROM subject_prerequisites sp
    JOIN subjects s ON sp.subject_id = s.subject_id
    JOIN subjects ps ON sp.prerequisite_subject_id = ps.subject_id
    WHERE sp.subject_id = @subject_id
    ORDER BY ps.subject_code;
END
GO
PRINT 'Created GetSubjectPrerequisites procedure.';

-- Report generation procedure for curriculum
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GenerateCurriculumReport')
BEGIN
    DROP PROCEDURE GenerateCurriculumReport;
    PRINT 'Dropped existing GenerateCurriculumReport procedure.';
END
GO

CREATE PROCEDURE GenerateCurriculumReport
    @program_id INT,
    @curriculum_year VARCHAR(9) = NULL,
    @include_prerequisites BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get program information
    SELECT 
        p.program_id,
        p.program_code,
        p.program_name,
        COALESCE(@curriculum_year, MAX(c.curriculum_year)) AS curriculum_year,
        COUNT(DISTINCT c.subject_id) AS total_subjects,
        SUM(s.lecture_units) AS total_lecture_units,
        SUM(s.lab_units) AS total_lab_units,
        SUM(s.lecture_units + s.lab_units) AS total_units
    FROM programs p
    LEFT JOIN curriculum c ON p.program_id = c.program_id
    LEFT JOIN subjects s ON c.subject_id = s.subject_id
    WHERE p.program_id = @program_id
    AND (@curriculum_year IS NULL OR c.curriculum_year = @curriculum_year)
    GROUP BY p.program_id, p.program_code, p.program_name;
    
    -- Get curriculum details by year level and semester
    SELECT 
        yl.year_level_id,
        yl.year_name,
        sem.semester_id,
        sem.semester_name,
        sem.semester_code,
        c.curriculum_id,
        s.subject_code,
        s.subject_name,
        s.lecture_units,
        s.lab_units,
        (s.lecture_units + s.lab_units) AS total_units,
        c.subject_status,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                u.first_name + ' ' + u.last_name
            ELSE 
                u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
        END AS professor_name,
        fp.department AS professor_department
    FROM curriculum c
    JOIN subjects s ON c.subject_id = s.subject_id
    JOIN year_levels yl ON c.year_level_id = yl.year_level_id
    JOIN semesters sem ON c.semester_id = sem.semester_id
    LEFT JOIN users u ON c.faculty_id = u.user_id
    LEFT JOIN faculty_profiles fp ON u.user_id = fp.user_id
    WHERE c.program_id = @program_id
    AND (@curriculum_year IS NULL OR c.curriculum_year = @curriculum_year)
    ORDER BY yl.year_level_id, sem.semester_id, s.subject_code;
    
    -- Get subject prerequisites if requested
    IF @include_prerequisites = 1
    BEGIN
        SELECT 
            c.curriculum_id,
            s.subject_code,
            s.subject_name,
            ps.subject_code AS prerequisite_code,
            ps.subject_name AS prerequisite_name,
            (ps.lecture_units + ps.lab_units) AS prerequisite_units
        FROM curriculum c
        JOIN subjects s ON c.subject_id = s.subject_id
        JOIN subject_prerequisites sp ON s.subject_id = sp.subject_id
        JOIN subjects ps ON sp.prerequisite_subject_id = ps.subject_id
        WHERE c.program_id = @program_id
        AND (@curriculum_year IS NULL OR c.curriculum_year = @curriculum_year)
        ORDER BY s.subject_code, ps.subject_code;
    END
END
GO
PRINT 'Created GenerateCurriculumReport procedure.';

-- Summary of schema updates
PRINT '=================================================';
PRINT 'Combined Schema V8.0 (V7.0 - V7.4) installation complete';
PRINT 'Key changes in this version:';
PRINT '- Created base tables for classroom management system';
PRINT '- Added extended user profile fields (birthday, sex, address, etc.)';
PRINT '- Added school_year_id and enrollment_date to student_profiles';
PRINT '- Added lecture_units and lab_units columns to subjects table';
PRINT '- Created curriculum table with all necessary relationships';
PRINT '- Added is_active flags to various tables for better record management';
PRINT '- Added support for subject prerequisites';
PRINT '- Created views and stored procedures for curriculum management';
PRINT '=================================================';
