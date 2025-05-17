/*
Classroom Management System - Complete Database Schema
Version: 6.0.0
Date: 2025-04-05
Description: Enhanced schema with program/subject status, multiple teacher assignments,
             and comprehensive curriculum management
*/

-- Create table for school years with YY-YY format
CREATE TABLE school_years (
    school_year_id INT IDENTITY(1,1) PRIMARY KEY,
    year_name VARCHAR(5) NOT NULL UNIQUE, -- e.g. "24-25" format
    is_current BIT DEFAULT 0 NOT NULL,
    start_date DATE NULL,
    end_date DATE NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create reference tables for programs with status
CREATE TABLE programs (
    program_id INT IDENTITY(1,1) PRIMARY KEY,
    program_name VARCHAR(100) NOT NULL,
    program_code VARCHAR(20) NOT NULL UNIQUE,
    description VARCHAR(500) NULL,
    is_active BIT DEFAULT 1 NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create reference tables for sections
CREATE TABLE sections (
    section_id INT IDENTITY(1,1) PRIMARY KEY,
    section_name VARCHAR(10) NOT NULL,
    description VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create reference tables for year levels
CREATE TABLE year_levels (
    year_level_id INT IDENTITY(1,1) PRIMARY KEY,
    year_name VARCHAR(20) NOT NULL,
    description VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create semesters table
CREATE TABLE semesters (
    semester_id INT IDENTITY(1,1) PRIMARY KEY,
    semester_name VARCHAR(20) NOT NULL,
    semester_code VARCHAR(10) NOT NULL, -- e.g. "1ST", "2ND"
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create user table with archiving capability
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NULL,
    middle_name VARCHAR(50) NULL,
    last_name VARCHAR(50) NULL,
    user_type VARCHAR(20) NULL CHECK (user_type IN ('Student', 'Faculty', 'Admin-MIS', 'Registrar')),
    birthday DATE NULL,
    sex VARCHAR(10) NULL CHECK (sex IN ('Male', 'Female', 'Other', 'Prefer not to say')),
    address VARCHAR(500) NULL,
    contact_number VARCHAR(20) NULL,
    registration_step TINYINT DEFAULT 1,
    is_archived BIT DEFAULT 0 NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create faculty profiles table
CREATE TABLE faculty_profiles (
    faculty_profile_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    faculty_id VARCHAR(20) NULL UNIQUE, -- Employee ID
    department VARCHAR(100) NULL,
    position VARCHAR(100) NULL,
    is_fulltime BIT DEFAULT 1,
    specialization VARCHAR(255) NULL,
    rank VARCHAR(50) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
GO

-- Create student profiles table with school_year_id
CREATE TABLE student_profiles (
    profile_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    student_id VARCHAR(20) NULL UNIQUE,
    program_id INT NULL,
    year_level_id INT NULL,
    section_id INT NULL,
    school_year_id INT NULL, -- Field for tracking academic year
    student_status VARCHAR(20) DEFAULT 'regular' CHECK (student_status IN ('regular', 'irregular')),
    enrollment_date DATE DEFAULT GETDATE(),
    academic_status VARCHAR(20) DEFAULT 'active', -- active, leave, graduated, etc.
    FOREIGN KEY (user_id) REFERENCES users (user_id),
    FOREIGN KEY (program_id) REFERENCES programs (program_id),
    FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
    FOREIGN KEY (section_id) REFERENCES sections (section_id),
    FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id)
);
GO

-- Create subjects table with status
CREATE TABLE subjects (
    subject_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_code VARCHAR(20) NOT NULL UNIQUE,
    subject_name VARCHAR(100) NOT NULL,
    description VARCHAR(500) NULL,
    units INT DEFAULT 3 NOT NULL,
    lecture_hours DECIMAL(4,1) NULL DEFAULT 2,
    lab_hours DECIMAL(4,1) NULL DEFAULT 3,
    is_active BIT DEFAULT 1 NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create a curriculum table to track different versions of program curricula
CREATE TABLE curriculum (
    curriculum_id INT IDENTITY(1,1) PRIMARY KEY,
    program_id INT NOT NULL,
    school_year_id INT NOT NULL, -- Effective school year when this curriculum was introduced
    curriculum_name VARCHAR(100) NOT NULL, -- e.g., "BSCS 2023 Curriculum"
    is_active BIT DEFAULT 1 NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (program_id) REFERENCES programs (program_id),
    FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id)
);
GO

-- Create program_subjects table to associate subjects with programs including year and semester
CREATE TABLE program_subjects (
    program_subject_id INT IDENTITY(1,1) PRIMARY KEY,
    curriculum_id INT NOT NULL,
    subject_id INT NOT NULL,
    year_level_id INT NOT NULL,
    semester_id INT NOT NULL,
    units INT DEFAULT 3 NOT NULL, -- Number of units/credits for this subject
    lecture_hours DECIMAL(4,1) NULL DEFAULT 2,
    lab_hours DECIMAL(4,1) NULL DEFAULT 3,
    is_required BIT DEFAULT 1 NOT NULL,
    prerequisite_subject_id INT NULL, -- Self-reference to another subject that must be taken first
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (curriculum_id) REFERENCES curriculum (curriculum_id),
    FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
    FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
    FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
    FOREIGN KEY (prerequisite_subject_id) REFERENCES subjects (subject_id)
);
GO

-- Create subject offerings table with teacher assignment capability
CREATE TABLE subject_offerings (
    offering_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_id INT NOT NULL,
    class_code VARCHAR(10) NULL UNIQUE,
    school_year_id INT NOT NULL,
    semester_id INT NOT NULL,
    year_level_id INT NULL,
    is_active BIT DEFAULT 1 NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
    FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id),
    FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
    FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id)
);
GO

-- Create table for multiple teacher assignments to subject offerings
CREATE TABLE subject_teachers (
    subject_teacher_id INT IDENTITY(1,1) PRIMARY KEY,
    offering_id INT NOT NULL,
    faculty_id INT NOT NULL, -- References user_id
    is_primary BIT DEFAULT 0 NOT NULL, -- Indicates if this is the primary teacher
    assignment_date DATETIME DEFAULT GETDATE(),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (offering_id) REFERENCES subject_offerings (offering_id),
    FOREIGN KEY (faculty_id) REFERENCES users (user_id)
);
GO

-- Create enrollments table
CREATE TABLE enrollments (
    enrollment_id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    offering_id INT NOT NULL,
    enrollment_date DATETIME DEFAULT GETDATE(),
    grade VARCHAR(5) NULL,
    remarks VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (student_id) REFERENCES users (user_id),
    FOREIGN KEY (offering_id) REFERENCES subject_offerings (offering_id)
);
GO

-- Create posts table (for notes, assignments, announcements)
CREATE TABLE posts (
    post_id INT IDENTITY(1,1) PRIMARY KEY,
    offering_id INT NULL,
    user_id INT NOT NULL,
    title VARCHAR(100) NOT NULL,
    content NVARCHAR(MAX) NOT NULL,
    content_type VARCHAR(20) DEFAULT 'post' CHECK (content_type IN ('post', 'assignment', 'announcement')),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (offering_id) REFERENCES subject_offerings (offering_id),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);
GO

-- Create comments table
CREATE TABLE comments (
    comment_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    user_id INT NOT NULL,
    content NVARCHAR(500) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    FOREIGN KEY (post_id) REFERENCES posts (post_id),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);
GO

-- Create notifications table
CREATE TABLE notifications (
    notification_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    title VARCHAR(100) NOT NULL,
    message NVARCHAR(500) NOT NULL,
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);
GO

-- Create post attachments table for file uploads
CREATE TABLE post_attachments (
    attachment_id INT IDENTITY(1,1) PRIMARY KEY,
    post_id INT NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    file_path VARCHAR(255) NOT NULL,
    file_type VARCHAR(100) NOT NULL,
    file_size INT NOT NULL, -- Size in KB
    upload_status VARCHAR(20) DEFAULT 'complete',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (post_id) REFERENCES posts (post_id)
);
GO

-- Populate reference tables with initial data
INSERT INTO programs (program_name, program_code) VALUES 
('BS Computer Science', 'BSCS'),
('BS Information Technology', 'BSIT'),
('BS Information System', 'BSIS'),
('BS Entertainment and Multimedia Computing', 'BSEMC');
GO

INSERT INTO sections (section_name) VALUES ('A'), ('B'), ('C');
GO

INSERT INTO year_levels (year_name) VALUES 
('Year 1'), ('Year 2'), ('Year 3'), ('Year 4'), ('Year 5');
GO

-- Insert default semesters
INSERT INTO semesters (semester_name, semester_code) VALUES
('First Semester', '1ST'),
('Second Semester', '2ND'),
('Summer', 'SUM');
GO

-- Insert initial school years with YY-YY format
INSERT INTO school_years (year_name, is_current, start_date, end_date) VALUES 
('23-24', 0, '2023-06-01', '2024-03-31'),
('24-25', 1, '2024-06-01', '2025-03-31'),
('25-26', 0, '2025-06-01', '2026-03-31');
GO

-- Create stored procedures for common operations

-- Procedure to enroll a student in a subject
CREATE PROCEDURE EnrollStudent
    @student_id INT,
    @offering_id INT,
    @remarks VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO enrollments (student_id, offering_id, enrollment_date, remarks)
    VALUES (@student_id, @offering_id, GETDATE(), @remarks);
    
    SELECT SCOPE_IDENTITY() AS enrollment_id;
END;
GO

-- Procedure to add a new post
CREATE PROCEDURE AddPost
    @offering_id INT,
    @user_id INT,
    @title VARCHAR(100),
    @content NVARCHAR(MAX),
    @content_type VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO posts (offering_id, user_id, title, content, content_type, created_at)
    VALUES (@offering_id, @user_id, @title, @content, @content_type, GETDATE());
    
    SELECT SCOPE_IDENTITY() AS post_id;
END;
GO

-- Procedure to archive a user
CREATE PROCEDURE ArchiveUser
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE users
    SET is_archived = 1, updated_at = GETDATE()
    WHERE user_id = @user_id;
    
    IF @@ROWCOUNT > 0
        SELECT 1 AS Success, 'User archived successfully' AS Message;
    ELSE
        SELECT 0 AS Success, 'User not found' AS Message;
END;
GO

-- Procedure to restore an archived user
CREATE PROCEDURE RestoreUser
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE users
    SET is_archived = 0, updated_at = GETDATE()
    WHERE user_id = @user_id;
    
    IF @@ROWCOUNT > 0
        SELECT 1 AS Success, 'User restored successfully' AS Message;
    ELSE
        SELECT 0 AS Success, 'User not found' AS Message;
END;
GO

-- Procedure to get active users (non-archived)
CREATE PROCEDURE GetActiveUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.user_id, u.email, u.first_name, u.middle_name, u.last_name, u.user_type,
           u.birthday, u.sex, u.address, u.contact_number,
           sp.student_id, sp.program_id, p.program_name, p.program_code,
           sp.year_level_id, yl.year_name,
           sp.section_id, s.section_name,
           sp.student_status, sp.academic_status, sp.enrollment_date,
           sp.school_year_id, sy.year_name AS school_year
    FROM users u
    LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
    LEFT JOIN programs p ON sp.program_id = p.program_id
    LEFT JOIN year_levels yl ON sp.year_level_id = yl.year_level_id
    LEFT JOIN sections s ON sp.section_id = s.section_id
    LEFT JOIN school_years sy ON sp.school_year_id = sy.school_year_id
    WHERE (u.is_archived = 0 OR u.is_archived IS NULL)
    ORDER BY u.created_at DESC;
END;
GO

-- Procedure to get active faculty members
CREATE PROCEDURE GetActiveFaculty
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.user_id, u.email, u.first_name, u.middle_name, u.last_name, 
           u.birthday, u.sex, u.address, u.contact_number,
           fp.faculty_id, fp.department, fp.position, fp.is_fulltime,
           fp.specialization, fp.rank
    FROM users u
    INNER JOIN faculty_profiles fp ON u.user_id = fp.user_id
    WHERE u.user_type = 'Faculty' AND (u.is_archived = 0 OR u.is_archived IS NULL)
    ORDER BY u.last_name, u.first_name;
END;
GO

-- Procedure to create a student profile with school year
CREATE PROCEDURE CreateStudentProfile
    @user_id INT,
    @student_id VARCHAR(20),
    @program_id INT,
    @year_level_id INT,
    @section_id INT,
    @school_year_id INT = NULL,
    @student_status VARCHAR(20) = 'regular'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- If no school year is specified, use the current school year
    IF @school_year_id IS NULL
    BEGIN
        SELECT @school_year_id = school_year_id FROM school_years WHERE is_current = 1;
    END;
    
    -- Check if the student profile already exists
    IF EXISTS (SELECT 1 FROM student_profiles WHERE user_id = @user_id)
    BEGIN
        -- Update existing profile
        UPDATE student_profiles
        SET student_id = @student_id,
            program_id = @program_id,
            year_level_id = @year_level_id,
            section_id = @section_id,
            school_year_id = @school_year_id,
            student_status = @student_status,
            updated_at = GETDATE()
        WHERE user_id = @user_id;
        
        SELECT profile_id FROM student_profiles WHERE user_id = @user_id;
    END
    ELSE
    BEGIN
        -- Create new profile
        INSERT INTO student_profiles (
            user_id, student_id, program_id, year_level_id, 
            section_id, school_year_id, student_status, academic_status
        )
        VALUES (
            @user_id, @student_id, @program_id, @year_level_id, 
            @section_id, @school_year_id, @student_status, 'active'
        );
        
        SELECT SCOPE_IDENTITY() AS profile_id;
    END
END;
GO

-- Procedure to create a faculty profile
CREATE PROCEDURE CreateFacultyProfile
    @user_id INT,
    @faculty_id VARCHAR(20) = NULL,
    @department VARCHAR(100) = NULL,
    @position VARCHAR(100) = NULL,
    @is_fulltime BIT = 1,
    @specialization VARCHAR(255) = NULL,
    @rank VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if the faculty profile already exists
    IF EXISTS (SELECT 1 FROM faculty_profiles WHERE user_id = @user_id)
    BEGIN
        -- Update existing profile
        UPDATE faculty_profiles
        SET faculty_id = @faculty_id,
            department = @department,
            position = @position,
            is_fulltime = @is_fulltime,
            specialization = @specialization,
            rank = @rank,
            updated_at = GETDATE()
        WHERE user_id = @user_id;
        
        SELECT faculty_profile_id FROM faculty_profiles WHERE user_id = @user_id;
    END
    ELSE
    BEGIN
        -- Create new profile
        INSERT INTO faculty_profiles (
            user_id, faculty_id, department, position, 
            is_fulltime, specialization, rank
        )
        VALUES (
            @user_id, @faculty_id, @department, @position, 
            @is_fulltime, @specialization, @rank
        );
        
        SELECT SCOPE_IDENTITY() AS faculty_profile_id;
    END
END;
GO

-- Procedure to get the current school year
CREATE PROCEDURE GetCurrentSchoolYear
AS
BEGIN
    SET NOCOUNT ON;
    SELECT school_year_id, year_name, start_date, end_date 
    FROM school_years 
    WHERE is_current = 1;
END;
GO

-- Procedure to create new school year
CREATE PROCEDURE CreateSchoolYear
    @start_year INT,
    @is_current BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calculate the school year in YY-YY format
    DECLARE @year_name VARCHAR(5);
    SET @year_name = RIGHT(CAST(@start_year AS VARCHAR(4)), 2) + '-' + RIGHT(CAST(@start_year + 1 AS VARCHAR(4)), 2);
    
    -- Calculate start and end dates (June 1 to March 31)
    DECLARE @start_date DATE = CAST(@start_year AS VARCHAR(4)) + '-06-01';
    DECLARE @end_date DATE = CAST(@start_year + 1 AS VARCHAR(4)) + '-03-31';
    
    -- If this will be the current year, unmark any existing current year
    IF @is_current = 1
    BEGIN
        UPDATE school_years SET is_current = 0;
    END
    
    -- Insert the new school year
    INSERT INTO school_years (year_name, is_current, start_date, end_date)
    VALUES (@year_name, @is_current, @start_date, @end_date);
    
    -- Return the new school year ID
    SELECT SCOPE_IDENTITY() AS school_year_id;
END;
GO

-- Helper procedure to set current school year
CREATE PROCEDURE SetCurrentSchoolYear
    @school_year_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- First, set all school years to non-current
    UPDATE school_years SET is_current = 0;
    
    -- Then set the specified one as current
    UPDATE school_years SET is_current = 1 WHERE school_year_id = @school_year_id;
    
    -- Return success status
    IF @@ROWCOUNT > 0
        SELECT 1 AS Success, 'School year updated successfully' AS Message;
    ELSE
        SELECT 0 AS Success, 'School year not found' AS Message;
END;
GO

-- Procedure to create a new curriculum for a program
CREATE PROCEDURE CreateCurriculum
    @program_id INT,
    @school_year_id INT,
    @curriculum_name VARCHAR(100),
    @is_active BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- If setting this curriculum to active, deactivate other curricula for this program
    IF @is_active = 1
    BEGIN
        UPDATE curriculum 
        SET is_active = 0
        WHERE program_id = @program_id;
    END
    
    -- Insert the new curriculum
    INSERT INTO curriculum (program_id, school_year_id, curriculum_name, is_active)
    VALUES (@program_id, @school_year_id, @curriculum_name, @is_active);
    
    -- Return the new curriculum ID
    SELECT SCOPE_IDENTITY() AS curriculum_id;
END;
GO

-- Procedure to add a subject to a program curriculum
CREATE PROCEDURE AddSubjectToCurriculum
    @curriculum_id INT,
    @subject_id INT,
    @year_level_id INT,
    @semester_id INT,
    @units INT = 3,
    @lecture_hours DECIMAL(4,1) = 2.0,
    @lab_hours DECIMAL(4,1) = 3.0,
    @is_required BIT = 1,
    @prerequisite_subject_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if subject already exists in curriculum for the same year and semester
    IF EXISTS (
        SELECT 1 FROM program_subjects 
        WHERE curriculum_id = @curriculum_id 
        AND subject_id = @subject_id
        AND year_level_id = @year_level_id
        AND semester_id = @semester_id
    )
    BEGIN
        -- Update existing subject in curriculum
        UPDATE program_subjects
        SET units = @units,
            lecture_hours = @lecture_hours,
            lab_hours = @lab_hours,
            is_required = @is_required,
            prerequisite_subject_id = @prerequisite_subject_id,
            updated_at = GETDATE()
        WHERE curriculum_id = @curriculum_id 
        AND subject_id = @subject_id
        AND year_level_id = @year_level_id
        AND semester_id = @semester_id;
        
        SELECT program_subject_id FROM program_subjects
        WHERE curriculum_id = @curriculum_id 
        AND subject_id = @subject_id
        AND year_level_id = @year_level_id
        AND semester_id = @semester_id;
    END
    ELSE
    BEGIN
        -- Insert the subject into the curriculum
        INSERT INTO program_subjects (
            curriculum_id, subject_id, year_level_id, semester_id, 
            units, lecture_hours, lab_hours, is_required, prerequisite_subject_id
        )
        VALUES (
            @curriculum_id, @subject_id, @year_level_id, @semester_id, 
            @units, @lecture_hours, @lab_hours, @is_required, @prerequisite_subject_id
        );
        
        -- Return the new program_subject ID
        SELECT SCOPE_IDENTITY() AS program_subject_id;
    END
END;
GO

-- Procedure to get a program's curriculum with subjects by year and semester
CREATE PROCEDURE GetProgramCurriculum
    @program_id INT,
    @school_year_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- If school year is not specified, use the active curriculum
    DECLARE @curriculum_id INT;
    
    IF @school_year_id IS NULL
    BEGIN
        SELECT TOP 1 @curriculum_id = curriculum_id 
        FROM curriculum 
        WHERE program_id = @program_id AND is_active = 1;
    END
    ELSE
    BEGIN
        -- Get the curriculum effective for the specified school year
        SELECT TOP 1 @curriculum_id = c.curriculum_id 
        FROM curriculum c
        WHERE c.program_id = @program_id 
        AND c.school_year_id <= @school_year_id
        ORDER BY c.school_year_id DESC;
    END
    
    -- If no curriculum found, return an empty result
    IF @curriculum_id IS NULL
    BEGIN
        SELECT 0 AS result, 'No curriculum found for the specified program' AS message;
        RETURN;
    END
    
    -- Get the curriculum details with all subjects by year and semester
    SELECT 
        c.curriculum_id, c.curriculum_name, p.program_id, p.program_name,
        yl.year_level_id, yl.year_name, 
        s.semester_id, s.semester_name,
        subj.subject_id, subj.subject_code, subj.subject_name, subj.description,
        subj.is_active AS subject_is_active,
        ps.units, ps.lecture_hours, ps.lab_hours, ps.is_required,
        prereq.subject_code AS prerequisite_code, prereq.subject_name AS prerequisite_name
    FROM curriculum c
    JOIN programs p ON c.program_id = p.program_id
    JOIN program_subjects ps ON c.curriculum_id = ps.curriculum_id
    JOIN year_levels yl ON ps.year_level_id = yl.year_level_id
    JOIN semesters s ON ps.semester_id = s.semester_id
    JOIN subjects subj ON ps.subject_id = subj.subject_id
    LEFT JOIN subjects prereq ON ps.prerequisite_subject_id = prereq.subject_id
    WHERE c.curriculum_id = @curriculum_id
    ORDER BY yl.year_level_id, s.semester_id, subj.subject_code;
END;
GO

-- Procedure to create subject offerings for a curriculum
CREATE PROCEDURE CreateSubjectOfferings
    @curriculum_id INT,
    @school_year_id INT,
    @semester_id INT,
    @year_level_id INT = NULL -- If NULL, create offerings for all year levels
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create offerings for all relevant subjects in the curriculum
    INSERT INTO subject_offerings (subject_id, class_code, school_year_id, semester_id, year_level_id, is_active)
    SELECT 
        ps.subject_id,
        CONCAT(subj.subject_code, '-', sy.year_name, '-', sem.semester_code),
        @school_year_id,
        @semester_id,
        ps.year_level_id,
        1 -- is_active
    FROM program_subjects ps
    JOIN subjects subj ON ps.subject_id = subj.subject_id
    JOIN school_years sy ON sy.school_year_id = @school_year_id
    JOIN semesters sem ON sem.semester_id = @semester_id
    WHERE ps.curriculum_id = @curriculum_id
    AND (ps.year_level_id = @year_level_id OR @year_level_id IS NULL)
    AND ps.semester_id = @semester_id
    AND subj.is_active = 1;
    
    -- Return number of offerings created
    SELECT @@ROWCOUNT AS offerings_created;
END;
GO

-- Procedure to assign a teacher to a subject offering
CREATE PROCEDURE AssignTeacherToSubject
    @offering_id INT,
    @faculty_id INT,
    @is_primary BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if this faculty is already assigned to this offering
    IF EXISTS (SELECT 1 FROM subject_teachers 
              WHERE offering_id = @offering_id AND faculty_id = @faculty_id)
    BEGIN
        -- Update the existing assignment
        UPDATE subject_teachers
        SET is_primary = @is_primary,
            updated_at = GETDATE()
        WHERE offering_id = @offering_id AND faculty_id = @faculty_id;
        
        SELECT subject_teacher_id FROM subject_teachers 
        WHERE offering_id = @offering_id AND faculty_id = @faculty_id;
    END
    ELSE
    BEGIN
        -- If this is the primary teacher, unset any existing primary teacher
        IF @is_primary = 1
        BEGIN
            UPDATE subject_teachers
            SET is_primary = 0
            WHERE offering_id = @offering_id AND is_primary = 1;
        END
        
        -- Insert new teacher assignment
        INSERT INTO subject_teachers (offering_id, faculty_id, is_primary)
        VALUES (@offering_id, @faculty_id, @is_primary);
        
        SELECT SCOPE_IDENTITY() AS subject_teacher_id;
    END
END;
GO

-- Procedure to get teachers assigned to a subject offering
CREATE PROCEDURE GetSubjectTeachers
    @offering_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        st.subject_teacher_id, st.is_primary,
        u.user_id, u.first_name, u.middle_name, u.last_name,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                u.first_name + ' ' + u.last_name
            ELSE 
                u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
        END AS full_name,
        fp.faculty_id, fp.department, fp.position, fp.specialization
    FROM subject_teachers st
    JOIN users u ON st.faculty_id = u.user_id
    JOIN faculty_profiles fp ON u.user_id = fp.user_id
    WHERE st.offering_id = @offering_id
    ORDER BY st.is_primary DESC, u.last_name, u.first_name;
END;
GO

-- Procedure to get student's enrollment history by program and term
CREATE PROCEDURE GetStudentEnrollmentHistory
    @student_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.user_id, u.first_name, u.last_name,
        sp.student_id AS student_number, 
        p.program_name,
        sy.year_name AS school_year,
        sem.semester_name,
        yl.year_name AS year_level,
        s.subject_code, s.subject_name,
        e.enrollment_date, e.grade, e.remarks
    FROM enrollments e
    JOIN users u ON e.student_id = u.user_id
    JOIN student_profiles sp ON u.user_id = sp.user_id
    JOIN subject_offerings so ON e.offering_id = so.offering_id
    JOIN subjects s ON so.subject_id = s.subject_id
    JOIN school_years sy ON so.school_year_id = sy.school_year_id
    JOIN semesters sem ON so.semester_id = sem.semester_id
    JOIN year_levels yl ON so.year_level_id = yl.year_level_id
    JOIN programs p ON sp.program_id = p.program_id
    WHERE e.student_id = @student_id
    ORDER BY sy.year_name, sem.semester_id, s.subject_code;
END;
GO

-- Procedure to update subject status
CREATE PROCEDURE UpdateSubjectStatus
    @subject_id INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE subjects
    SET is_active = @is_active,
        updated_at = GETDATE()
    WHERE subject_id = @subject_id;
    
    IF @@ROWCOUNT > 0
        SELECT 1 AS Success, 'Subject status updated successfully' AS Message;
    ELSE
        SELECT 0 AS Success, 'Subject not found' AS Message;
END;
GO

-- Procedure to update program status
CREATE PROCEDURE UpdateProgramStatus
    @program_id INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE programs
    SET is_active = @is_active,
        updated_at = GETDATE()
    WHERE program_id = @program_id;
    
    IF @@ROWCOUNT > 0
        SELECT 1 AS Success, 'Program status updated successfully' AS Message;
    ELSE
        SELECT 0 AS Success, 'Program not found' AS Message;
END;
GO
