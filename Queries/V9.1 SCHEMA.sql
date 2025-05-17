/*
Classroom Management System - Calendar Enhancement
Version: 9.1
Date: 2025-04-10
Description: Enhancement to add calendar functionality for announcements and events
             Compatible with V8.2 and V9 schemas
             Allows for flexible event scheduling and viewing by class sections
Author: Bobsi01
*/

-- Set error handling to continue execution on errors
SET XACT_ABORT OFF;
SET NOCOUNT ON;
GO

-- Check for required schema versions
DECLARE @v82_exists BIT = 0;
DECLARE @v9_exists BIT = 0;

-- Check if schema_version table exists
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'schema_version')
BEGIN
    -- Check for V8.2
    IF EXISTS (SELECT 1 FROM schema_version WHERE version_number = '8.2')
        SET @v82_exists = 1;
        
    -- Check for V9
    IF EXISTS (SELECT 1 FROM schema_version WHERE version_number = '9')
        SET @v9_exists = 1;
END

-- Display warning if prerequisites may be missing
IF @v82_exists = 0 OR @v9_exists = 0
BEGIN
    PRINT '--------------------------------------------------------------';
    PRINT 'WARNING: This script is designed to work with V8.2 and V9 schemas.';
    PRINT 'It is recommended to run V8.2 SCHEMA.sql and V9 SCHEMA.sql before this script.';
    PRINT '--------------------------------------------------------------';
END
GO

-- Update schema version
BEGIN TRY
    -- Insert version record for V9.1 if it doesn't exist
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'schema_version')
    AND NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '9.1')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('9.1', 'Added calendar and event announcement system');
        PRINT 'Added schema version 9.1 record.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error updating schema version: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create calendar_events table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'calendar_events')
    BEGIN
        CREATE TABLE calendar_events (
            event_id INT IDENTITY(1,1) PRIMARY KEY,
            event_title VARCHAR(100) NOT NULL,
            event_description VARCHAR(1000) NULL,  -- Allow NULL for flexibility
            start_date DATETIME NOT NULL,
            end_date DATETIME NULL,               -- Allow NULL for single-day events
            all_day BIT DEFAULT 0,                -- Flag for all-day events
            location VARCHAR(200) NULL,           -- Allow NULL for flexibility
            event_type VARCHAR(50) DEFAULT 'Announcement',  -- Type of event
            created_by INT NOT NULL,              -- User who created the event
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (created_by) REFERENCES users(user_id)
        );
        
        PRINT 'Created calendar_events table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating calendar_events table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_visibility table for targeting specific groups
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_visibility')
    BEGIN
        CREATE TABLE event_visibility (
            visibility_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            visibility_type VARCHAR(50) NOT NULL,  -- 'ALL', 'PROGRAM', 'SECTION', 'YEAR_LEVEL'
            target_id INT NULL,                   -- Program ID, Section ID, etc. (NULL for ALL)
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE
        );
        
        -- Add indexes for performance
        CREATE INDEX IX_event_visibility_event_id ON event_visibility(event_id);
        CREATE INDEX IX_event_visibility_target_id ON event_visibility(target_id);
        
        PRINT 'Created event_visibility table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_visibility table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_attachments table for flexible attachments
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_attachments')
    BEGIN
        CREATE TABLE event_attachments (
            attachment_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            attachment_name VARCHAR(255) NOT NULL,
            attachment_path VARCHAR(500) NOT NULL,
            file_type VARCHAR(100) NULL,          -- Allow NULL for flexibility
            upload_date DATETIME DEFAULT GETDATE(),
            uploaded_by INT NOT NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE,
            FOREIGN KEY (uploaded_by) REFERENCES users(user_id)
        );
        
        PRINT 'Created event_attachments table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_attachments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_reminders table for notification functionality
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_reminders')
    BEGIN
        CREATE TABLE event_reminders (
            reminder_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            reminder_time INT NOT NULL,           -- Minutes before the event
            reminder_type VARCHAR(50) DEFAULT 'NOTIFICATION', -- Type of reminder
            is_active BIT DEFAULT 1,
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE
        );
        
        PRINT 'Created event_reminders table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_reminders table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create event_recurrence table for recurring events
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'event_recurrence')
    BEGIN
        CREATE TABLE event_recurrence (
            recurrence_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            recurrence_type VARCHAR(50) NOT NULL,   -- 'DAILY', 'WEEKLY', 'MONTHLY', 'YEARLY'
            interval_value INT DEFAULT 1,          -- Every X days/weeks/etc.
            days_of_week VARCHAR(20) NULL,         -- For weekly: '1,2,3,4,5,6,7' (Sun-Sat)
            day_of_month INT NULL,                 -- For monthly by day: 1-31
            month_of_year INT NULL,                -- For yearly: 1-12
            end_date DATE NULL,                    -- NULL for no end date
            count INT NULL,                        -- Number of occurrences (NULL for no limit)
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id) ON DELETE CASCADE
        );
        
        PRINT 'Created event_recurrence table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating event_recurrence table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create user_event_responses table for RSVP functionality
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'user_event_responses')
    BEGIN
        CREATE TABLE user_event_responses (
            response_id INT IDENTITY(1,1) PRIMARY KEY,
            event_id INT NOT NULL,
            user_id INT NOT NULL,
            response_status VARCHAR(20) NOT NULL,  -- 'ATTENDING', 'DECLINED', 'MAYBE', 'PENDING'
            response_date DATETIME DEFAULT GETDATE(),
            comments VARCHAR(500) NULL,            -- Allow NULL for flexibility
            FOREIGN KEY (event_id) REFERENCES calendar_events(event_id),
            FOREIGN KEY (user_id) REFERENCES users(user_id),
            CONSTRAINT UK_user_event_response UNIQUE (user_id, event_id)
        );
        
        PRINT 'Created user_event_responses table.';
    END
END TRY
BEGIN CATCH
    PRINT 'Error creating user_event_responses table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create view to easily check upcoming events by section
BEGIN TRY
    EXEC('
    CREATE OR ALTER VIEW upcoming_events_by_section AS
    SELECT 
        ce.event_id,
        ce.event_title,
        ce.event_description,
        ce.start_date,
        ce.end_date,
        ce.location,
        ce.event_type,
        u.first_name + '' '' + u.last_name AS created_by_name,
        s.section_id,
        s.section_name,
        p.program_id,
        p.program_name,
        yl.year_level_id,
        yl.year_name
    FROM 
        calendar_events ce
    INNER JOIN 
        users u ON ce.created_by = u.user_id
    INNER JOIN 
        event_visibility ev ON ce.event_id = ev.event_id
    LEFT JOIN 
        sections s ON (ev.visibility_type = ''SECTION'' AND ev.target_id = s.section_id)
    LEFT JOIN
        programs p ON (ev.visibility_type = ''PROGRAM'' AND ev.target_id = p.program_id)
    LEFT JOIN
        year_levels yl ON (ev.visibility_type = ''YEAR_LEVEL'' AND ev.target_id = yl.year_level_id)
    WHERE 
        ce.is_active = 1
        AND ce.start_date >= GETDATE()
        OR (ce.end_date IS NOT NULL AND ce.end_date >= GETDATE())
    ');
    
    PRINT 'Created upcoming_events_by_section view.';
END TRY
BEGIN CATCH
    PRINT 'Error creating upcoming_events_by_section view: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create stored procedure to add an event
BEGIN TRY
    EXEC('
    CREATE OR ALTER PROCEDURE sp_add_calendar_event
        @title VARCHAR(100),
        @description VARCHAR(1000) = NULL,
        @start_date DATETIME,
        @end_date DATETIME = NULL,
        @all_day BIT = 0,
        @location VARCHAR(200) = NULL,
        @event_type VARCHAR(50) = ''Announcement'',
        @created_by INT,
        @visibility_type VARCHAR(50) = ''ALL'',
        @target_ids VARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @event_id INT;
        
        BEGIN TRANSACTION;
        
        BEGIN TRY
            -- Insert the event
            INSERT INTO calendar_events (
                event_title, 
                event_description, 
                start_date, 
                end_date, 
                all_day, 
                location, 
                event_type, 
                created_by
            )
            VALUES (
                @title, 
                @description, 
                @start_date, 
                @end_date, 
                @all_day, 
                @location, 
                @event_type, 
                @created_by
            );
            
            SET @event_id = SCOPE_IDENTITY();
            
            -- Handle visibility settings
            IF @visibility_type = ''ALL''
            BEGIN
                INSERT INTO event_visibility (event_id, visibility_type, target_id)
                VALUES (@event_id, ''ALL'', NULL);
            END
            ELSE IF @target_ids IS NOT NULL
            BEGIN
                -- Split the target IDs by comma and insert
                WITH targets AS (
                    SELECT value AS target_id
                    FROM STRING_SPLIT(@target_ids, '','')
                )
                INSERT INTO event_visibility (event_id, visibility_type, target_id)
                SELECT @event_id, @visibility_type, CAST(target_id AS INT)
                FROM targets;
            END
            
            COMMIT TRANSACTION;
            
            -- Return the new event ID
            SELECT @event_id AS event_id, ''Event created successfully'' AS status;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            SELECT 0 AS event_id, ''Error creating event: '' + ERROR_MESSAGE() AS status;
        END CATCH
    END
    ');
    
    PRINT 'Created sp_add_calendar_event stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating sp_add_calendar_event stored procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create stored procedure to get events by section
BEGIN TRY
    EXEC('
    CREATE OR ALTER PROCEDURE sp_get_section_events
        @section_id INT,
        @start_date DATE,
        @end_date DATE = NULL,
        @include_program_events BIT = 1,
        @include_year_level_events BIT = 1,
        @include_all_events BIT = 1
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- If no end date provided, default to 31 days after start
        IF @end_date IS NULL
            SET @end_date = DATEADD(DAY, 31, @start_date);
            
        -- Get section, program, and year level info
        DECLARE @program_id INT;
        DECLARE @year_level_id INT;
        
        SELECT 
            @program_id = s.program_id,
            @year_level_id = s.year_level_id
        FROM sections s
        WHERE s.section_id = @section_id;
        
        -- Query events visible to this section
        SELECT 
            ce.event_id,
            ce.event_title,
            ce.event_description,
            ce.start_date,
            ce.end_date,
            ce.all_day,
            ce.location,
            ce.event_type,
            u.first_name + '' '' + u.last_name AS created_by_name,
            CASE
                WHEN ev.visibility_type = ''SECTION'' THEN ''Section-specific''
                WHEN ev.visibility_type = ''PROGRAM'' THEN ''Program-wide''
                WHEN ev.visibility_type = ''YEAR_LEVEL'' THEN ''Year level''
                WHEN ev.visibility_type = ''ALL'' THEN ''Everyone''
                ELSE ''Unknown''
            END AS visibility
        FROM calendar_events ce
        INNER JOIN users u ON ce.created_by = u.user_id
        INNER JOIN event_visibility ev ON ce.event_id = ev.event_id
        WHERE ce.is_active = 1
        AND (
            (ce.start_date BETWEEN @start_date AND @end_date)
            OR (ce.end_date IS NOT NULL AND ce.end_date BETWEEN @start_date AND @end_date)
            OR (ce.start_date <= @start_date AND (ce.end_date IS NULL OR ce.end_date >= @end_date))
        )
        AND (
            (ev.visibility_type = ''SECTION'' AND ev.target_id = @section_id)
            OR (@include_program_events = 1 AND ev.visibility_type = ''PROGRAM'' AND ev.target_id = @program_id)
            OR (@include_year_level_events = 1 AND ev.visibility_type = ''YEAR_LEVEL'' AND ev.target_id = @year_level_id)
            OR (@include_all_events = 1 AND ev.visibility_type = ''ALL'')
        )
        ORDER BY ce.start_date, ce.event_title;
    END
    ');
    
    PRINT 'Created sp_get_section_events stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating sp_get_section_events stored procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create stored procedure to get events by date range
BEGIN TRY
    EXEC('
    CREATE OR ALTER PROCEDURE sp_get_events_by_date
        @start_date DATE,
        @end_date DATE = NULL,
        @user_id INT = NULL,
        @visibility_filter VARCHAR(50) = NULL,
        @target_id INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- If no end date provided, default to 31 days after start
        IF @end_date IS NULL
            SET @end_date = DATEADD(DAY, 31, @start_date);
        
        -- Get user profile information if filtering is needed
        DECLARE @user_program_id INT;
        DECLARE @user_year_level_id INT;
        DECLARE @user_section_id INT;
        
        IF @user_id IS NOT NULL
        BEGIN
            SELECT 
                @user_program_id = program_id,
                @user_year_level_id = year_level_id,
                @user_section_id = section_id
            FROM student_profiles
            WHERE user_id = @user_id;
        END
        
        -- Query events
        SELECT 
            ce.event_id,
            ce.event_title,
            ce.event_description,
            ce.start_date,
            ce.end_date,
            ce.all_day,
            ce.location,
            ce.event_type,
            u.first_name + '' '' + u.last_name AS created_by_name,
            CASE
                WHEN ev.visibility_type = ''SECTION'' THEN ''Section-specific''
                WHEN ev.visibility_type = ''PROGRAM'' THEN ''Program-wide''
                WHEN ev.visibility_type = ''YEAR_LEVEL'' THEN ''Year level''
                WHEN ev.visibility_type = ''ALL'' THEN ''Everyone''
                ELSE ''Unknown''
            END AS visibility
        FROM calendar_events ce
        INNER JOIN users u ON ce.created_by = u.user_id
        INNER JOIN event_visibility ev ON ce.event_id = ev.event_id
        WHERE ce.is_active = 1
        AND (
            (ce.start_date BETWEEN @start_date AND @end_date)
            OR (ce.end_date IS NOT NULL AND ce.end_date BETWEEN @start_date AND @end_date)
            OR (ce.start_date <= @start_date AND (ce.end_date IS NULL OR ce.end_date >= @end_date))
        )
        -- Apply user-based visibility filtering
        AND (
            @user_id IS NULL
            OR @user_id = ce.created_by
            OR (ev.visibility_type = ''ALL'')
            OR (ev.visibility_type = ''SECTION'' AND ev.target_id = @user_section_id)
            OR (ev.visibility_type = ''PROGRAM'' AND ev.target_id = @user_program_id)
            OR (ev.visibility_type = ''YEAR_LEVEL'' AND ev.target_id = @user_year_level_id)
        )
        -- Apply explicit visibility filtering if provided
        AND (
            @visibility_filter IS NULL
            OR (ev.visibility_type = @visibility_filter AND (@target_id IS NULL OR ev.target_id = @target_id))
        )
        ORDER BY ce.start_date, ce.event_title;
    END
    ');
    
    PRINT 'Created sp_get_events_by_date stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating sp_get_events_by_date stored procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Display summary of changes
PRINT '--------------------------------------------------------------';
PRINT 'CALENDAR ENHANCEMENT V9.1 - SUMMARY OF CHANGES';
PRINT '--------------------------------------------------------------';
PRINT '1. Created new tables: ';
PRINT '   - calendar_events: Core table for storing event details';
PRINT '   - event_visibility: Controls who can see events (by program, section, year level)';
PRINT '   - event_attachments: Allows files to be attached to events';
PRINT '   - event_reminders: Supports notification functionality';
PRINT '   - event_recurrence: Enables recurring events';
PRINT '   - user_event_responses: Tracks user responses to events';
PRINT '';
PRINT '2. Created database views: ';
PRINT '   - upcoming_events_by_section: Shows events relevant to each section';
PRINT '';
PRINT '3. Added stored procedures: ';
PRINT '   - sp_add_calendar_event: Creates new calendar events';
PRINT '   - sp_get_section_events: Retrieves events for a specific section';
PRINT '   - sp_get_events_by_date: Retrieves events within a date range';
PRINT '';
PRINT '4. Features implemented: ';
PRINT '   - Google Calendar-like event system for class sections';
PRINT '   - Support for single and multi-day events';
PRINT '   - Flexible NULL support for optional information';
PRINT '   - Event visibility controls (section, program, year level, or all)';
PRINT '   - Support for event attachments and reminders';
PRINT '   - RSVP functionality for event attendance tracking';
PRINT '--------------------------------------------------------------';
PRINT 'Calendar system successfully installed.';
PRINT 'Schema version upgraded to V9.1';
PRINT '--------------------------------------------------------------';
GO