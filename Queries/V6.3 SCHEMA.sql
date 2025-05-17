/*
Classroom Management System - Schema Enhancement
Version: 6.3.0
Date: 2025-04-08
Description: Enhances V6.2 with lecture/lab units for subjects and curriculum management functionality
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

-- Step 1: Check if we need to apply this version
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '6.3.0')
BEGIN
    -- Insert version record
    INSERT INTO schema_version (version_number, description) 
    VALUES ('6.3.0', 'Added lecture/lab units to subjects and curriculum management functionality');
    
    -- Step 2: Modify subjects table to add lecture_units and lab_units columns
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subjects')
    BEGIN
        -- Check if lecture_units column already exists
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lecture_units')
        BEGIN
            -- Add lecture_units column
            ALTER TABLE subjects
            ADD lecture_units INT NOT NULL DEFAULT 0;
            PRINT 'Added lecture_units column to subjects table.';
        END

        -- Check if lab_units column already exists
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'subjects' AND COLUMN_NAME = 'lab_units')
        BEGIN
            -- Add lab_units column
            ALTER TABLE subjects
            ADD lab_units INT NOT NULL DEFAULT 0;
            PRINT 'Added lab_units column to subjects table.';
        END

        -- Update existing records to set lecture_units = units and lab_units = 0
        UPDATE subjects 
        SET lecture_units = units, lab_units = 0 
        WHERE lecture_units = 0 AND lab_units = 0;
        PRINT 'Updated existing subjects with default lecture_units and lab_units values.';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: subjects table does not exist. Cannot add lecture_units and lab_units columns.';
    END

    -- Step 3: Create curriculum table if it doesn't exist
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
        PRINT 'Created curriculum table.';
    END
    ELSE
    BEGIN
        -- If curriculum table exists but might be missing some columns, add them
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'curriculum_year')
        BEGIN
            ALTER TABLE curriculum
            ADD curriculum_year VARCHAR(9) NOT NULL DEFAULT '2024-2025';
            PRINT 'Added curriculum_year column to curriculum table.';
        END
        
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'subject_status')
        BEGIN
            ALTER TABLE curriculum
            ADD subject_status VARCHAR(20) DEFAULT 'active';
            PRINT 'Added subject_status column to curriculum table.';
        END
        
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'curriculum' AND COLUMN_NAME = 'faculty_id')
        BEGIN
            ALTER TABLE curriculum
            ADD faculty_id INT NULL;
            PRINT 'Added faculty_id column to curriculum table.';
            
            -- Add foreign key constraint
            ALTER TABLE curriculum
            ADD CONSTRAINT FK_curriculum_faculty FOREIGN KEY (faculty_id) REFERENCES users (user_id);
            PRINT 'Added foreign key constraint for faculty_id.';
        END
    END
    
    -- Step 4: Create or update curriculum_view to join all needed tables
    IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_curriculum_manager')
    BEGIN
        DROP VIEW vw_curriculum_manager;
        PRINT 'Dropped existing vw_curriculum_manager view.';
    END
    
    EXEC('
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
            WHEN u.middle_name IS NULL OR u.middle_name = '''' THEN 
                u.first_name + '' '' + u.last_name
            ELSE 
                u.first_name + '' '' + LEFT(u.middle_name, 1) + ''. '' + u.last_name
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
    ');
    PRINT 'Created vw_curriculum_manager view.';
    
    -- Step 5: Create or update ManageCurriculumEntry stored procedure
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'ManageCurriculumEntry')
    BEGIN
        DROP PROCEDURE ManageCurriculumEntry;
        PRINT 'Dropped existing ManageCurriculumEntry procedure.';
    END
    
    EXEC('
    CREATE PROCEDURE ManageCurriculumEntry
        @curriculum_id INT = NULL,
        @program_id INT,
        @subject_id INT,
        @school_year_id INT,
        @semester_id INT,
        @year_level_id INT,
        @curriculum_year VARCHAR(9),
        @subject_status VARCHAR(20) = ''active'',
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
            SELECT ''Curriculum entry deleted successfully.'' AS Status;
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
            
            SELECT ''Curriculum entry updated successfully.'' AS Status;
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
                SELECT ''Entry already exists in curriculum.'' AS Status;
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
            
            SELECT SCOPE_IDENTITY() AS curriculum_id, ''Curriculum entry created successfully.'' AS Status;
        END
    END
    ');
    PRINT 'Created ManageCurriculumEntry stored procedure.';
    
    -- Step 6: Create helper stored procedures for the Curriculum Manager
    
    -- Procedure to get available subjects for curriculum assignment
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetAvailableSubjectsForCurriculum')
    BEGIN
        DROP PROCEDURE GetAvailableSubjectsForCurriculum;
        PRINT 'Dropped existing GetAvailableSubjectsForCurriculum procedure.';
    END
    
    EXEC('
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
    ');
    PRINT 'Created GetAvailableSubjectsForCurriculum procedure.';
    
    -- Procedure to get faculty members available for assignment
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetAvailableFaculty')
    BEGIN
        DROP PROCEDURE GetAvailableFaculty;
        PRINT 'Dropped existing GetAvailableFaculty procedure.';
    END
    
    EXEC('
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
                WHEN u.middle_name IS NULL OR u.middle_name = '''' THEN 
                    u.first_name + '' '' + u.last_name
                ELSE 
                    u.first_name + '' '' + LEFT(u.middle_name, 1) + ''. '' + u.last_name
            END AS faculty_name,
            fp.faculty_id AS faculty_code,
            fp.department,
            fp.position
        FROM users u
        JOIN faculty_profiles fp ON u.user_id = fp.user_id
        WHERE u.user_type = ''Faculty''
        AND fp.is_active = 1
        AND (u.is_archived = 0 OR u.is_archived IS NULL)
        ORDER BY u.last_name, u.first_name;
    END
    ');
    PRINT 'Created GetAvailableFaculty procedure.';
    
    -- Procedure to get curriculum entries by program and curriculum year
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetCurriculumByProgram')
    BEGIN
        DROP PROCEDURE GetCurriculumByProgram;
        PRINT 'Dropped existing GetCurriculumByProgram procedure.';
    END
    
    EXEC('
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
                WHEN u.middle_name IS NULL OR u.middle_name = '''' THEN 
                    u.first_name + '' '' + u.last_name
                ELSE 
                    u.first_name + '' '' + LEFT(u.middle_name, 1) + ''. '' + u.last_name
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
    ');
    PRINT 'Created GetCurriculumByProgram procedure.';
    
    -- Create a procedure to check prerequisites for subjects
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
    
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'ManageSubjectPrerequisites')
    BEGIN
        DROP PROCEDURE ManageSubjectPrerequisites;
        PRINT 'Dropped existing ManageSubjectPrerequisites procedure.';
    END
    
    EXEC('
    CREATE PROCEDURE ManageSubjectPrerequisites
        @subject_id INT,
        @prerequisite_subject_ids VARCHAR(MAX), -- Comma-separated list of subject IDs
        @operation VARCHAR(10) = ''SET'' -- ''SET'' or ''ADD'' or ''REMOVE''
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Convert the comma-separated string to a table
        DECLARE @PrerequisiteTable TABLE (prerequisite_subject_id INT);
        
        INSERT INTO @PrerequisiteTable
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@prerequisite_subject_ids, '','');
        
        -- For SET operation, remove all existing prerequisites and add the new ones
        IF @operation = ''SET''
        BEGIN
            DELETE FROM subject_prerequisites WHERE subject_id = @subject_id;
            
            INSERT INTO subject_prerequisites (subject_id, prerequisite_subject_id)
            SELECT @subject_id, prerequisite_subject_id 
            FROM @PrerequisiteTable;
            
            SELECT ''Prerequisites set successfully.'' AS Status;
        END
        
        -- For ADD operation, add new prerequisites that don''t exist yet
        ELSE IF @operation = ''ADD''
        BEGIN
            INSERT INTO subject_prerequisites (subject_id, prerequisite_subject_id)
            SELECT @subject_id, prerequisite_subject_id 
            FROM @PrerequisiteTable
            WHERE prerequisite_subject_id NOT IN (
                SELECT prerequisite_subject_id 
                FROM subject_prerequisites 
                WHERE subject_id = @subject_id
            );
            
            SELECT ''Prerequisites added successfully.'' AS Status;
        END
        
        -- For REMOVE operation, remove specified prerequisites
        ELSE IF @operation = ''REMOVE''
        BEGIN
            DELETE FROM subject_prerequisites 
            WHERE subject_id = @subject_id
            AND prerequisite_subject_id IN (
                SELECT prerequisite_subject_id 
                FROM @PrerequisiteTable
            );
            
            SELECT ''Prerequisites removed successfully.'' AS Status;
        END
    END
    ');
    PRINT 'Created ManageSubjectPrerequisites procedure.';
    
    -- Procedure to get prerequisites for a subject
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GetSubjectPrerequisites')
    BEGIN
        DROP PROCEDURE GetSubjectPrerequisites;
        PRINT 'Dropped existing GetSubjectPrerequisites procedure.';
    END
    
    EXEC('
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
    ');
    PRINT 'Created GetSubjectPrerequisites procedure.';
    
    -- Report generation procedure for curriculum
    IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'GenerateCurriculumReport')
    BEGIN
        DROP PROCEDURE GenerateCurriculumReport;
        PRINT 'Dropped existing GenerateCurriculumReport procedure.';
    END
    
    EXEC('
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
                WHEN u.middle_name IS NULL OR u.middle_name = '''' THEN 
                    u.first_name + '' '' + u.last_name
                ELSE 
                    u.first_name + '' '' + LEFT(u.middle_name, 1) + ''. '' + u.last_name
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
    ');
    PRINT 'Created GenerateCurriculumReport procedure.';
    
    -- Summary of schema updates
    PRINT '=================================================';
    PRINT 'Schema V6.3.0 installation complete';
    PRINT 'Key changes in this version:';
    PRINT '- Added lecture_units and lab_units columns to subjects table';
    PRINT '- Created curriculum table for managing course curriculums';
    PRINT '- Added support for subject prerequisites';
    PRINT '- Added views and stored procedures for curriculum management';
    PRINT '=================================================';
END
ELSE
BEGIN
    PRINT 'Schema V6.3.0 is already installed.';
END
