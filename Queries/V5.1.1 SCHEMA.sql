/*
Classroom Management System - Updated Database Schema
Version: 5.1.1
Date: 2025-04-03
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

-- Create reference tables for programs, sections, and year levels
CREATE TABLE programs (
    program_id INT IDENTITY(1,1) PRIMARY KEY,
    program_name VARCHAR(100) NOT NULL,
    description VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

CREATE TABLE sections (
    section_id INT IDENTITY(1,1) PRIMARY KEY,
    section_name VARCHAR(10) NOT NULL,
    description VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

CREATE TABLE year_levels (
    year_level_id INT IDENTITY(1,1) PRIMARY KEY,
    year_name VARCHAR(20) NOT NULL,
    description VARCHAR(500) NULL,
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
    middle_name VARCHAR (50)  NULL,
    last_name VARCHAR(50) NULL,
    user_type VARCHAR(20) NULL CHECK (user_type IN ('Student', 'Faculty', 'Admin-MIS','Registrar')),
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

-- Create student profiles table with school_year_id
CREATE TABLE student_profiles (
    profile_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    student_id VARCHAR(20) NULL UNIQUE,
    program_id INT NULL,
    year_level_id INT NULL,
    section_id INT NULL,
    school_year_id INT NULL, -- New field for tracking academic year
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

-- Create subjects table
CREATE TABLE subjects (
    subject_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_code VARCHAR(20) NOT NULL UNIQUE,
    subject_name VARCHAR(100) NOT NULL,
    description VARCHAR(500) NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create subject offerings table
CREATE TABLE subject_offerings (
    offering_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_id INT NOT NULL,
    class_code VARCHAR(10) NULL UNIQUE,
    school_year_id INT NULL, -- Adding school year to subject offerings
    FOREIGN KEY (subject_id) REFERENCES subjects (subject_id),
    FOREIGN KEY (school_year_id) REFERENCES school_years (school_year_id)
);
GO

-- Create enrollments table
CREATE TABLE enrollments (
    enrollment_id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    offering_id INT NOT NULL,
    enrollment_date DATETIME DEFAULT GETDATE(),
    grade VARCHAR(5) NULL,
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
INSERT INTO programs (program_name) VALUES 
('BS Computer Science'),
('BS Information Technology'),
('BS Information System'),
('BS Entertainment and Multimedia Computing');
GO

INSERT INTO sections (section_name) VALUES ('A'), ('B'), ('C');
GO

INSERT INTO year_levels (year_name) VALUES 
('Year 1'), ('Year 2'), ('Year 3'), ('Year 4'), ('Year 5');
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
    @offering_id INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO enrollments (student_id, offering_id, enrollment_date)
    VALUES (@student_id, @offering_id, GETDATE());
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
END;
GO

-- Procedure to get active users (non-archived)
CREATE PROCEDURE GetActiveUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.user_id, u.email, u.first_name, u.last_name, u.user_type,
           u.birthday, u.sex, u.address, u.contact_number,
           sp.student_id, sp.program_id, p.program_name,
           sp.year_level_id, sp.section_id, s.section_name,
           sp.student_status, sp.academic_status, sp.enrollment_date,
           sp.school_year_id, sy.year_name AS school_year
    FROM users u
    LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
    LEFT JOIN programs p ON sp.program_id = p.program_id
    LEFT JOIN sections s ON sp.section_id = s.section_id
    LEFT JOIN school_years sy ON sp.school_year_id = sy.school_year_id
    WHERE u.is_archived = 0 OR u.is_archived IS NULL
    ORDER BY u.created_at DESC;
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
    
    INSERT INTO student_profiles (user_id, student_id, program_id, year_level_id, 
                                section_id, school_year_id, student_status, academic_status)
    VALUES (@user_id, @student_id, @program_id, @year_level_id, 
           @section_id, @school_year_id, @student_status, 'active');
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
