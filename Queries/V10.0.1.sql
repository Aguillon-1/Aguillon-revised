-- =================================================================
-- V10.0.1: Ensure All Default Programs, Year Levels, and Sections Exist
-- Date: 2025-06-12
-- Author: Bobsi01
-- =================================================================

-- =========================
-- Add All Default Programs
-- =========================
IF NOT EXISTS (SELECT 1 FROM programs WHERE program_code = 'BSCS')
BEGIN
    INSERT INTO programs (program_name, program_code, description, is_active)
    VALUES ('BS Computer Science', 'BSCS', 'Bachelor of Science in Computer Science', 1);
    PRINT 'V10.0.1: Inserted BSCS program as default.';
END
ELSE
    PRINT 'V10.0.1: BSCS program already exists.';

IF NOT EXISTS (SELECT 1 FROM programs WHERE program_code = 'BSIT')
BEGIN
    INSERT INTO programs (program_name, program_code, description, is_active)
    VALUES ('BS Information Technology', 'BSIT', 'Bachelor of Science in Information Technology', 1);
    PRINT 'V10.0.1: Inserted BSIT program as default.';
END
ELSE
    PRINT 'V10.0.1: BSIT program already exists.';

IF NOT EXISTS (SELECT 1 FROM programs WHERE program_code = 'BSIS')
BEGIN
    INSERT INTO programs (program_name, program_code, description, is_active)
    VALUES ('BS Information Systems', 'BSIS', 'Bachelor of Science in Information Systems', 1);
    PRINT 'V10.0.1: Inserted BSIS program as default.';
END
ELSE
    PRINT 'V10.0.1: BSIS program already exists.';

IF NOT EXISTS (SELECT 1 FROM programs WHERE program_code = 'BSEMC')
BEGIN
    INSERT INTO programs (program_name, program_code, description, is_active)
    VALUES ('BS Entertainment and Multimedia Computing', 'BSEMC', 'Bachelor of Science in Entertainment and Multimedia Computing', 1);
    PRINT 'V10.0.1: Inserted BSEMC program as default.';
END
ELSE
    PRINT 'V10.0.1: BSEMC program already exists.';

-- =========================
-- Add All Default Year Levels
-- =========================
IF NOT EXISTS (SELECT 1 FROM year_levels WHERE year_name = 'Year 1')
BEGIN
    INSERT INTO year_levels (year_name, is_active) VALUES ('Year 1', 1);
    PRINT 'V10.0.1: Inserted Year 1 as default year level.';
END
ELSE
    PRINT 'V10.0.1: Year 1 already exists.';

IF NOT EXISTS (SELECT 1 FROM year_levels WHERE year_name = 'Year 2')
BEGIN
    INSERT INTO year_levels (year_name, is_active) VALUES ('Year 2', 1);
    PRINT 'V10.0.1: Inserted Year 2 as default year level.';
END
ELSE
    PRINT 'V10.0.1: Year 2 already exists.';

IF NOT EXISTS (SELECT 1 FROM year_levels WHERE year_name = 'Year 3')
BEGIN
    INSERT INTO year_levels (year_name, is_active) VALUES ('Year 3', 1);
    PRINT 'V10.0.1: Inserted Year 3 as default year level.';
END
ELSE
    PRINT 'V10.0.1: Year 3 already exists.';

IF NOT EXISTS (SELECT 1 FROM year_levels WHERE year_name = 'Year 4')
BEGIN
    INSERT INTO year_levels (year_name, is_active) VALUES ('Year 4', 1);
    PRINT 'V10.0.1: Inserted Year 4 as default year level.';
END
ELSE
    PRINT 'V10.0.1: Year 4 already exists.';

IF NOT EXISTS (SELECT 1 FROM year_levels WHERE year_name = 'Year 5')
BEGIN
    INSERT INTO year_levels (year_name, is_active) VALUES ('Year 5', 1);
    PRINT 'V10.0.1: Inserted Year 5 as default year level.';
END
ELSE
    PRINT 'V10.0.1: Year 5 already exists.';

-- =========================
-- Add Default Sections (A, B, C) for Each Program and Year Level
-- =========================
DECLARE @program_id INT, @year_level_id INT, @section_name VARCHAR(20);

DECLARE program_cursor CURSOR FOR
    SELECT program_id FROM programs WHERE program_code IN ('BSCS', 'BSIT', 'BSIS', 'BSEMC');
DECLARE year_level_cursor CURSOR FOR
    SELECT year_level_id FROM year_levels WHERE year_name IN ('Year 1', 'Year 2', 'Year 3', 'Year 4', 'Year 5');

-- Iterate for each program
OPEN program_cursor;
FETCH NEXT FROM program_cursor INTO @program_id;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Iterate for each year level
    OPEN year_level_cursor;
    FETCH NEXT FROM year_level_cursor INTO @year_level_id;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Add section A if missing
        SET @section_name = 'A';
        IF NOT EXISTS (
            SELECT 1 FROM sections 
            WHERE section_name = @section_name AND program_id = @program_id AND year_level_id = @year_level_id
        )
        BEGIN
            INSERT INTO sections (section_name, year_level_id, program_id)
            VALUES (@section_name, @year_level_id, @program_id);
            PRINT 'V10.0.1: Inserted Section A for program_id=' + CAST(@program_id AS VARCHAR) + ', year_level_id=' + CAST(@year_level_id AS VARCHAR) + '.';
        END

        -- Add section B if missing
        SET @section_name = 'B';
        IF NOT EXISTS (
            SELECT 1 FROM sections 
            WHERE section_name = @section_name AND program_id = @program_id AND year_level_id = @year_level_id
        )
        BEGIN
            INSERT INTO sections (section_name, year_level_id, program_id)
            VALUES (@section_name, @year_level_id, @program_id);
            PRINT 'V10.0.1: Inserted Section B for program_id=' + CAST(@program_id AS VARCHAR) + ', year_level_id=' + CAST(@year_level_id AS VARCHAR) + '.';
        END

        -- Add section C if missing
        SET @section_name = 'C';
        IF NOT EXISTS (
            SELECT 1 FROM sections 
            WHERE section_name = @section_name AND program_id = @program_id AND year_level_id = @year_level_id
        )
        BEGIN
            INSERT INTO sections (section_name, year_level_id, program_id)
            VALUES (@section_name, @year_level_id, @program_id);
            PRINT 'V10.0.1: Inserted Section C for program_id=' + CAST(@program_id AS VARCHAR) + ', year_level_id=' + CAST(@year_level_id AS VARCHAR) + '.';
        END

        FETCH NEXT FROM year_level_cursor INTO @year_level_id;
    END
    CLOSE year_level_cursor;

    FETCH NEXT FROM program_cursor INTO @program_id;
END
CLOSE program_cursor;
DEALLOCATE program_cursor;
DEALLOCATE year_level_cursor;

-- =========================
-- FINAL SUMMARY
-- =========================
PRINT '======================================================================';
PRINT 'V10.0.1 PATCH: ENSURED ALL DEFAULT PROGRAMS, YEAR LEVELS, AND SECTIONS EXIST';
PRINT '- Programs: BSCS, BSIT, BSIS, BSEMC';
PRINT '- Year Levels: Year 1, Year 2, Year 3, Year 4, Year 5';
PRINT '- Sections: A, B, C for each program and year level combination';
PRINT 'This script will only add what is missing; existing records are left untouched.';
PRINT '======================================================================';
GO