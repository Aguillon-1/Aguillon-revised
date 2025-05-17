/*
Classroom Management System - Updated Database Schema
Version: 4.1
Date: 2025-04-03
*/

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
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NULL,
    last_name VARCHAR(50) NULL,
    user_type VARCHAR(20) NULL CHECK (user_type IN ('Student', 'Faculty', 'Admin-MIS','Registrar')),
    registration_step TINYINT DEFAULT 1,
    is_archived BIT DEFAULT 0 NOT NULL, -- New column for archiving users
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create student profiles table with references to lookup tables
CREATE TABLE student_profiles (
    profile_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    student_id VARCHAR(20) NULL UNIQUE,
    program_id INT NULL,
    year_level_id INT NULL,
    section_id INT NULL,
    enrollment_date DATE DEFAULT GETDATE(),
    academic_status VARCHAR(20) DEFAULT 'active', -- active, leave, graduated, etc.
    FOREIGN KEY (user_id) REFERENCES users (user_id),
    FOREIGN KEY (program_id) REFERENCES programs (program_id),
    FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
    FOREIGN KEY (section_id) REFERENCES sections (section_id)
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
    FOREIGN KEY (subject_id) REFERENCES subjects (subject_id)
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

-- Create simple procedures for common operations

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

-- New procedure to archive a user
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

-- New procedure to restore an archived user
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

-- New procedure to get active users (non-archived)
CREATE PROCEDURE GetActiveUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.user_id,  u.email, u.first_name, u.last_name, u.user_type,
           sp.student_id, sp.program_id, p.program_name,
           sp.year_level_id, sp.section_id, s.section_name,
           sp.academic_status, sp.enrollment_date
    FROM users u
    LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
    LEFT JOIN programs p ON sp.program_id = p.program_id
    LEFT JOIN sections s ON sp.section_id = s.section_id
    WHERE u.is_archived = 0 OR u.is_archived IS NULL
    ORDER BY u.created_at DESC;
END;
GO

-- New procedure to get archived users
CREATE PROCEDURE GetArchivedUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.user_id,  u.email, u.first_name, u.last_name, u.user_type,
           sp.student_id, sp.program_id, p.program_name,
           sp.year_level_id, sp.section_id, s.section_name,
           sp.academic_status, sp.enrollment_date
    FROM users u
    LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
    LEFT JOIN programs p ON sp.program_id = p.program_id
    LEFT JOIN sections s ON sp.section_id = s.section_id
    WHERE u.is_archived = 1
    ORDER BY u.updated_at DESC;
END;
GO
