/*
Classroom Management System - Updated Database Schema
Version: 5.2.0
Date: 2025-04-05
Description: Enhanced with program curriculum management
*/

-- Create a semesters table
CREATE TABLE semesters (
    semester_id INT IDENTITY(1,1) PRIMARY KEY,
    semester_name VARCHAR(20) NOT NULL, -- e.g., "First Semester", "Second Semester"
    semester_code VARCHAR(10) NOT NULL, -- e.g., "1ST", "2ND"
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
    lecture_hours DECIMAL(4,1) NULL,
    lab_hours DECIMAL(4,1) NULL,
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

-- Modify subject_offerings table to include semester and year level information
ALTER TABLE subject_offerings
ADD semester_id INT NULL,
    year_level_id INT NULL,
    faculty_id INT NULL, -- The faculty teaching this offering
    FOREIGN KEY (semester_id) REFERENCES semesters (semester_id),
    FOREIGN KEY (year_level_id) REFERENCES year_levels (year_level_id),
    FOREIGN KEY (faculty_id) REFERENCES users (user_id);
GO

-- Inserting initial semester data
INSERT INTO semesters (semester_name, semester_code) VALUES
('First Semester', '1ST'),
('Second Semester', '2ND'),
('Summer', 'SUM');
GO

-- Create stored procedures for curriculum management

-- Create a new procedure to create curriculum for a program
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

-- Create a procedure to add a subject to a program curriculum
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
        RAISERROR('Subject already exists in this curriculum for the specified year and semester', 16, 1);
        RETURN;
    END
    
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
END;
GO

-- Create a procedure to get a program's curriculum with subjects by year and semester
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
        SELECT @curriculum_id = curriculum_id 
        FROM curriculum 
        WHERE program_id = @program_id AND is_active = 1;
    END
    ELSE
    BEGIN
        -- Get the curriculum effective for the specified school year
        SELECT TOP 1 @curriculum_id = curriculum_id 
        FROM curriculum 
        WHERE program_id = @program_id 
        AND school_year_id <= @school_year_id
        ORDER BY school_year_id DESC;
    END
    
    -- If no curriculum found, return an empty result
    IF @curriculum_id IS NULL
    BEGIN
        SELECT 0 AS result, 'No curriculum found for the specified program' AS message;
        RETURN;
    END
    
    -- Get the curriculum details with all subjects by year and semester
    SELECT 
        c.curriculum_id, c.curriculum_name, p.program_name,
        yl.year_level_id, yl.year_name, 
        s.semester_id, s.semester_name,
        subj.subject_id, subj.subject_code, subj.subject_name, subj.description,
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

-- Create a procedure to create subject offerings for a curriculum
CREATE PROCEDURE CreateSubjectOfferings
    @curriculum_id INT,
    @school_year_id INT,
    @year_level_id INT = NULL, -- If NULL, create offerings for all year levels
    @semester_id INT = NULL    -- If NULL, create offerings for all semesters
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create offerings for all relevant subjects in the curriculum
    INSERT INTO subject_offerings (subject_id, class_code, school_year_id, semester_id, year_level_id)
    SELECT 
        ps.subject_id,
        CONCAT(subj.subject_code, '-', sy.year_name, '-', s.semester_code),
        @school_year_id,
        ps.semester_id,
        ps.year_level_id
    FROM program_subjects ps
    JOIN subjects subj ON ps.subject_id = subj.subject_id
    JOIN semesters s ON ps.semester_id = s.semester_id
    JOIN school_years sy ON sy.school_year_id = @school_year_id
    WHERE ps.curriculum_id = @curriculum_id
    AND (ps.year_level_id = @year_level_id OR @year_level_id IS NULL)
    AND (ps.semester_id = @semester_id OR @semester_id IS NULL);
    
    -- Return number of offerings created
    SELECT @@ROWCOUNT AS offerings_created;
END;
GO

-- Create a procedure to get student's enrollment history by program and term
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
        yl.year_name AS year_level,
        sem.semester_name,
        s.subject_code, s.subject_name,
        e.enrollment_date, e.grade
    FROM enrollments e
    JOIN users u ON e.student_id = u.user_id
    JOIN student_profiles sp ON u.user_id = sp.user_id
    JOIN subject_offerings so ON e.offering_id = so.offering_id
    JOIN subjects s ON so.subject_id = s.subject_id
    JOIN school_years sy ON so.school_year_id = sy.school_year_id
    JOIN year_levels yl ON so.year_level_id = yl.year_level_id
    JOIN semesters sem ON so.semester_id = sem.semester_id
    JOIN programs p ON sp.program_id = p.program_id
    WHERE e.student_id = @student_id
    ORDER BY sy.year_name, sem.semester_id, s.subject_code;
END;
GO
