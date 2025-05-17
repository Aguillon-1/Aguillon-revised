/*
Classroom Management System - Schema Enhancement
Version: 7.1.0
Date: 2025-04-08
Description: Adds missing columns to both users and student_profiles tables
*/

-- Check if we need to apply this version
IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '7.1.0')
BEGIN
    -- Insert version record
    INSERT INTO schema_version (version_number, description) 
    VALUES ('7.1.0', 'Added missing fields to users and student_profiles tables');
    
    -- Check if users table exists
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
    BEGIN
        -- Add missing columns to users table
        
        -- Add school_year_id column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'school_year_id')
        BEGIN
            ALTER TABLE users
            ADD school_year_id INT NULL;
            
            -- Add foreign key constraint if school_years table exists
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'school_years')
            BEGIN
                ALTER TABLE users
                ADD CONSTRAINT FK_users_school_year FOREIGN KEY (school_year_id) 
                REFERENCES school_years (school_year_id);
            END
            
            PRINT 'Added school_year_id column to users table.';
        END
        
        -- Add birthday column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'birthday')
        BEGIN
            ALTER TABLE users
            ADD birthday DATE NULL;
            PRINT 'Added birthday column to users table.';
        END
        
        -- Add sex column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'sex')
        BEGIN
            ALTER TABLE users
            ADD sex VARCHAR(10) NULL;
            PRINT 'Added sex column to users table.';
        END
        
        -- Add address column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'address')
        BEGIN
            ALTER TABLE users
            ADD address VARCHAR(500) NULL;
            PRINT 'Added address column to users table.';
        END
        
        -- Add contact_number column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'contact_number')
        BEGIN
            ALTER TABLE users
            ADD contact_number VARCHAR(20) NULL;
            PRINT 'Added contact_number column to users table.';
        END
        
        -- Add enrollment_date column if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                      WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'enrollment_date')
        BEGIN
            ALTER TABLE users
            ADD enrollment_date DATE NULL;
            PRINT 'Added enrollment_date column to users table.';
        END
        
        PRINT 'Successfully added all missing user information columns.';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: users table does not exist. Cannot add columns.';
    END
    
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
    PRINT 'Schema V7.1.0 installation complete';
    PRINT 'Key changes in this version:';
    PRINT '- Added missing user information fields:';
    PRINT '  * school_year_id';
    PRINT '  * birthday';
    PRINT '  * sex';
    PRINT '  * address';
    PRINT '  * contact_number';
    PRINT '  * enrollment_date';
    PRINT '- Added missing student profile fields:';
    PRINT '  * school_year_id (with FK to school_years)';
    PRINT '  * enrollment_date (with DEFAULT GETDATE())';
    PRINT '=================================================';
END
ELSE
BEGIN
    PRINT 'Schema V7.1.0 is already installed.';
END
