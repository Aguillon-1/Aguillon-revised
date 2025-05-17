/*
Classroom Management System - Schema Enhancement
Version: 6.1.0
Date: 2025-04-05
Description: Adding grading system with midterm and final grades
*/

-- Create grade types table (midterm, final, etc.)
CREATE TABLE grade_types (
    grade_type_id INT IDENTITY(1,1) PRIMARY KEY,
    type_name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255) NULL,
    weight DECIMAL(5,2) DEFAULT 50.00, -- Default weight as percentage (50%)
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL
);
GO

-- Create grading period table
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
GO

-- Create student grades table
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
GO

-- Create grade components table for detailed breakdowns (optional)
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
GO

-- Insert default grade types
INSERT INTO grade_types (type_name, description, weight) VALUES 
('Midterm', 'Midterm examination grade', 40.00),
('Final', 'Final examination grade', 60.00);
GO

-- Create stored procedures for managing grades

-- Procedure to add or update a student grade
CREATE PROCEDURE AddOrUpdateStudentGrade
    @enrollment_id INT,
    @grade_type_id INT,
    @score DECIMAL(5,2) = NULL,
    @letter_grade VARCHAR(5) = NULL,
    @remarks VARCHAR(500) = NULL,
    @is_published BIT = 0,
    @graded_by INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if the grade already exists
    IF EXISTS (SELECT 1 FROM student_grades 
               WHERE enrollment_id = @enrollment_id AND grade_type_id = @grade_type_id)
    BEGIN
        -- Update existing grade
        UPDATE student_grades
        SET score = @score,
            letter_grade = @letter_grade,
            remarks = @remarks,
            is_published = @is_published,
            graded_by = @graded_by,
            graded_at = GETDATE(),
            updated_at = GETDATE()
        WHERE enrollment_id = @enrollment_id AND grade_type_id = @grade_type_id;
        
        SELECT grade_id FROM student_grades 
        WHERE enrollment_id = @enrollment_id AND grade_type_id = @grade_type_id;
    END
    ELSE
    BEGIN
        -- Insert new grade
        INSERT INTO student_grades (
            enrollment_id, grade_type_id, score, letter_grade, 
            remarks, is_published, graded_by
        )
        VALUES (
            @enrollment_id, @grade_type_id, @score, @letter_grade,
            @remarks, @is_published, @graded_by
        );
        
        -- Return the new grade ID
        SELECT SCOPE_IDENTITY() AS grade_id;
    END
END;
GO

-- Procedure to get student grades by offering ID
CREATE PROCEDURE GetStudentGradesByOffering
    @offering_id INT,
    @faculty_id INT = NULL -- Optional: filter by faculty if multiple teachers
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.user_id, u.first_name, u.middle_name, u.last_name,
        CASE 
            WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                u.first_name + ' ' + u.last_name
            ELSE 
                u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
        END AS student_name,
        sp.student_id AS student_number,
        e.enrollment_id,
        gt.type_name AS grade_type,
        sg.grade_id, sg.score, sg.letter_grade,
        sg.is_published, sg.remarks,
        fu.first_name + ' ' + fu.last_name AS graded_by_name,
        sg.graded_at
    FROM enrollments e
    JOIN users u ON e.student_id = u.user_id
    JOIN student_profiles sp ON u.user_id = sp.user_id
    JOIN subject_offerings so ON e.offering_id = so.offering_id
    LEFT JOIN subject_teachers st ON so.offering_id = st.offering_id
    LEFT JOIN student_grades sg ON e.enrollment_id = sg.enrollment_id
    LEFT JOIN grade_types gt ON sg.grade_type_id = gt.grade_type_id
    LEFT JOIN users fu ON sg.graded_by = fu.user_id
    WHERE e.offering_id = @offering_id
    AND (@faculty_id IS NULL OR st.faculty_id = @faculty_id)
    ORDER BY u.last_name, u.first_name, gt.type_name;
END;
GO

-- Procedure to get a student's grades across all subjects
CREATE PROCEDURE GetStudentGrades
    @student_id INT,
    @school_year_id INT = NULL,
    @semester_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.user_id, u.first_name, u.last_name,
        sp.student_id AS student_number,
        sy.year_name AS school_year,
        sem.semester_name,
        subj.subject_code, subj.subject_name,
        gt.type_name AS grade_type,
        sg.score, sg.letter_grade,
        sg.is_published,
        fu.first_name + ' ' + fu.last_name AS professor_name,
        sg.graded_at
    FROM enrollments e
    JOIN users u ON e.student_id = u.user_id
    JOIN student_profiles sp ON u.user_id = sp.user_id
    JOIN subject_offerings so ON e.offering_id = so.offering_id
    JOIN subjects subj ON so.subject_id = subj.subject_id
    JOIN school_years sy ON so.school_year_id = sy.school_year_id
    JOIN semesters sem ON so.semester_id = sem.semester_id
    LEFT JOIN student_grades sg ON e.enrollment_id = sg.enrollment_id
    LEFT JOIN grade_types gt ON sg.grade_type_id = gt.grade_type_id
    LEFT JOIN users fu ON sg.graded_by = fu.user_id
    WHERE e.student_id = @student_id
    AND (so.school_year_id = @school_year_id OR @school_year_id IS NULL)
    AND (so.semester_id = @semester_id OR @semester_id IS NULL)
    ORDER BY sy.year_name, sem.semester_name, subj.subject_code, gt.type_name;
END;
GO

-- Procedure to calculate final grades based on midterm and final exam scores
CREATE PROCEDURE CalculateFinalGrades
    @enrollment_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @midterm_score DECIMAL(5,2),
            @final_score DECIMAL(5,2),
            @midterm_weight DECIMAL(5,2),
            @final_weight DECIMAL(5,2),
            @final_grade DECIMAL(5,2),
            @letter_grade VARCHAR(5);
    
    -- Get midterm score and weight
    SELECT @midterm_score = sg.score, @midterm_weight = gt.weight
    FROM student_grades sg
    JOIN grade_types gt ON sg.grade_type_id = gt.grade_type_id
    WHERE sg.enrollment_id = @enrollment_id AND gt.type_name = 'Midterm';
    
    -- Get final exam score and weight
    SELECT @final_score = sg.score, @final_weight = gt.weight
    FROM student_grades sg
    JOIN grade_types gt ON sg.grade_type_id = gt.grade_type_id
    WHERE sg.enrollment_id = @enrollment_id AND gt.type_name = 'Final';
    
    -- If both grades exist, calculate final grade
    IF @midterm_score IS NOT NULL AND @final_score IS NOT NULL
    BEGIN
        -- Calculate weighted average
        SET @final_grade = ((@midterm_score * @midterm_weight) + (@final_score * @final_weight)) / (@midterm_weight + @final_weight);
        
        -- Determine letter grade based on score
        SET @letter_grade = 
            CASE 
                WHEN @final_grade >= 90 THEN 'A'
                WHEN @final_grade >= 85 THEN 'B+'
                WHEN @final_grade >= 80 THEN 'B'
                WHEN @final_grade >= 75 THEN 'C+'
                WHEN @final_grade >= 70 THEN 'C'
                WHEN @final_grade >= 65 THEN 'D+'
                WHEN @final_grade >= 60 THEN 'D'
                ELSE 'F'
            END;
        
        -- Update the enrollment with the final grade
        UPDATE enrollments
        SET grade = @letter_grade,
            updated_at = GETDATE()
        WHERE enrollment_id = @enrollment_id;
        
        SELECT @enrollment_id AS enrollment_id, @midterm_score AS midterm_score, 
               @final_score AS final_score, @final_grade AS final_grade, 
               @letter_grade AS letter_grade;
    END
    ELSE
    BEGIN
        SELECT @enrollment_id AS enrollment_id, 
               @midterm_score AS midterm_score, 
               @final_score AS final_score,
               NULL AS final_grade,
               NULL AS letter_grade,
               'Missing grade components' AS message;
    END
END;
GO

-- Procedure to publish grades for an entire subject offering
CREATE PROCEDURE PublishGrades
    @offering_id INT,
    @grade_type_id INT,
    @faculty_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE sg
    SET sg.is_published = 1,
        sg.updated_at = GETDATE()
    FROM student_grades sg
    JOIN enrollments e ON sg.enrollment_id = e.enrollment_id
    JOIN subject_offerings so ON e.offering_id = so.offering_id
    JOIN subject_teachers st ON so.offering_id = st.offering_id
    WHERE e.offering_id = @offering_id
    AND sg.grade_type_id = @grade_type_id
    AND st.faculty_id = @faculty_id;
    
    SELECT @@ROWCOUNT AS grades_published;
END;
GO

-- Procedure to calculate grade statistics for a subject offering
CREATE PROCEDURE GetGradeStatistics
    @offering_id INT,
    @grade_type_id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        subj.subject_code, 
        subj.subject_name,
        gt.type_name AS grade_type,
        COUNT(sg.grade_id) AS student_count,
        AVG(sg.score) AS average_score,
        MAX(sg.score) AS highest_score,
        MIN(sg.score) AS lowest_score,
        (SELECT COUNT(*) FROM student_grades sg2 
         JOIN enrollments e2 ON sg2.enrollment_id = e2.enrollment_id
         WHERE e2.offering_id = @offering_id 
         AND (sg2.grade_type_id = @grade_type_id OR @grade_type_id IS NULL)
         AND sg2.score >= 90) AS count_a,
        (SELECT COUNT(*) FROM student_grades sg2 
         JOIN enrollments e2 ON sg2.enrollment_id = e2.enrollment_id
         WHERE e2.offering_id = @offering_id 
         AND (sg2.grade_type_id = @grade_type_id OR @grade_type_id IS NULL)
         AND sg2.score >= 80 AND sg2.score < 90) AS count_b,
        (SELECT COUNT(*) FROM student_grades sg2 
         JOIN enrollments e2 ON sg2.enrollment_id = e2.enrollment_id
         WHERE e2.offering_id = @offering_id 
         AND (sg2.grade_type_id = @grade_type_id OR @grade_type_id IS NULL)
         AND sg2.score >= 70 AND sg2.score < 80) AS count_c,
        (SELECT COUNT(*) FROM student_grades sg2 
         JOIN enrollments e2 ON sg2.enrollment_id = e2.enrollment_id
         WHERE e2.offering_id = @offering_id 
         AND (sg2.grade_type_id = @grade_type_id OR @grade_type_id IS NULL)
         AND sg2.score >= 60 AND sg2.score < 70) AS count_d,
        (SELECT COUNT(*) FROM student_grades sg2 
         JOIN enrollments e2 ON sg2.enrollment_id = e2.enrollment_id
         WHERE e2.offering_id = @offering_id 
         AND (sg2.grade_type_id = @grade_type_id OR @grade_type_id IS NULL)
         AND sg2.score < 60) AS count_f
    FROM subject_offerings so
    JOIN subjects subj ON so.subject_id = subj.subject_id
    JOIN enrollments e ON so.offering_id = e.offering_id
    JOIN student_grades sg ON e.enrollment_id = sg.enrollment_id
    JOIN grade_types gt ON sg.grade_type_id = gt.grade_type_id
    WHERE so.offering_id = @offering_id
    AND (sg.grade_type_id = @grade_type_id OR @grade_type_id IS NULL)
    GROUP BY subj.subject_code, subj.subject_name, gt.type_name;
END;
GO
