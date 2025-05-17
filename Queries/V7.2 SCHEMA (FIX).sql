/*
Classroom Management System - Schema Enhancement
Version: 7.2.0 FIX FOR V7
Date: 2025-04-08
Description: Adds missing columns to student_profiles table
*/

-- Check if we need to apply this version
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '7.2.0')
BEGIN
    -- Insert version record
    INSERT INTO schema_version (version_number, description) 
    VALUES ('7.2.0', 'Added missing fields to student_profiles table');
    
    -- Check if student_profiles table exists
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'student_profiles')
    BEGIN
        -- Add school_year_id column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'student_profiles' AND COLUMN_NAME = 'school_year_id')
        BEGIN
            ALTER TABLE student_profiles
            ADD school_year_id INT NULL;
            
            -- Add foreign key constraint if school_years table exists
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
            BEGIN
                ALTER TABLE student_profiles
                ADD CONSTRAINT FK_student_profiles_school_year FOREIGN KEY (school_year_id) 
                REFERENCES school_years (school_year_id);
            END
            
            PRINT 'Added school_year_id column to student_profiles table.';
        END
        
        -- Add enrollment_date column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'student_profiles' AND COLUMN_NAME = 'enrollment_date')
        BEGIN
            ALTER TABLE student_profiles
            ADD enrollment_date DATE DEFAULT GETDATE();
            PRINT 'Added enrollment_date column to student_profiles table.';
        END
        
        PRINT 'Successfully added all missing student profile columns.';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: student_profiles table does not exist. Cannot add columns.';
    END
    
    -- Summary of schema updates
    PRINT '=================================================';
    PRINT 'Schema V7.2.0 installation complete';
    PRINT 'Key changes in this version:';
    PRINT '- Added missing student profile fields:';
    PRINT '  * school_year_id (with FK to school_years)';
    PRINT '  * enrollment_date (with DEFAULT GETDATE())';
    PRINT '=================================================';
END
ELSE
BEGIN
    PRINT 'Schema V7.2.0 is already installed.';
END

