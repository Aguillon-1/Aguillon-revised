/*
Classroom Management System - Simplified Database Schema
Version: 4.0
Date: 2025-04-02
*/

-- Create user table
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NULL,
    last_name VARCHAR(50) NULL,
    user_type VARCHAR(20) NULL CHECK (user_type IN ('Student', 'Faculty', 'Admin-MIS','Registrar')),
    registration_step TINYINT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create student profiles table to capture student-specific details
CREATE TABLE student_profiles (
    profile_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    student_id VARCHAR(20) NULL UNIQUE,
    program_id INT NULL,
    year_level_id INT NULL,
    section_id INT NULL,
    enrollment_date DATE DEFAULT GETDATE(),
    academic_status VARCHAR(20) DEFAULT 'active', -- active, leave, graduated, etc.
    FOREIGN KEY (user_id) REFERENCES users (user_id)
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