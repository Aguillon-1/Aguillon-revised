/*
Classroom Management System - Enhanced Schema with Post Support
Version: 9.0
Date: 2025-04-10
Description: Adds support for creating posts with header/body text, file attachments, 
             and commenting functionality. Posts can be associated with specific subjects
             or be general at the section level. Includes announcement and moderation flags.
*/

-- Set error handling to continue execution on errors
SET XACT_ABORT OFF;
SET NOCOUNT ON;
GO

-- Display dependency warning
PRINT '======================================================================';
PRINT 'WARNING: V8.2 SCHEMA must be deployed before running this V9 SCHEMA!';
PRINT 'This schema is an incremental update and depends on tables and';
PRINT 'structures created in V8.2 SCHEMA.';
PRINT '======================================================================';
GO

-- Check if required tables from V8.2 exist
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
   OR NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'subject_offerings')
   OR NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'sections')
   OR NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'programs')
BEGIN
    RAISERROR('Required tables from V8.2 SCHEMA are missing. Please deploy V8.2 SCHEMA first.', 16, 1);
    RETURN;
END
GO

-- Create a versioning table if it doesn't exist
BEGIN TRY
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
    ELSE
        PRINT 'schema_version table already exists.';

    -- Insert version record for V9.0 if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM schema_version WHERE version_number = '9.0')
    BEGIN
        INSERT INTO schema_version (version_number, description) 
        VALUES ('9.0', 'Added post system with file attachments, commenting functionality, announcement and moderation flags');
        PRINT 'Added schema version 9.0 record.';
    END
    ELSE
        PRINT 'Schema version 9.0 record already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error handling schema_version table: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== ADD POST SYSTEM TABLES ===========

-- Create posts table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'posts')
    BEGIN
        CREATE TABLE posts (
            post_id INT IDENTITY(1,1) PRIMARY KEY,
            user_id INT NOT NULL,                           -- Author of the post
            post_title NVARCHAR(200) NOT NULL,              -- Header/title text
            post_content NVARCHAR(MAX) NULL,                -- Body text
            subject_offering_id INT NULL,                   -- NULL if general post
            section_id INT NULL,                            -- NULL if subject-specific post
            program_id INT NULL,                            -- Program association (optional)
            is_pinned BIT DEFAULT 0,                        -- For pinning important posts
            is_announcement BIT DEFAULT 0,                  -- To mark as announcement
            is_moderated BIT DEFAULT 0,                     -- To mark if post has been moderated
            view_count INT DEFAULT 0,                       -- Track post popularity
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            FOREIGN KEY (subject_offering_id) REFERENCES subject_offerings (offering_id),
            FOREIGN KEY (section_id) REFERENCES sections (section_id),
            FOREIGN KEY (program_id) REFERENCES programs (program_id),
            -- Require at least subject_offering_id OR section_id 
            CONSTRAINT CHK_posts_association CHECK (
                (subject_offering_id IS NOT NULL) OR (section_id IS NOT NULL)
            )
        );
        PRINT 'Created posts table.';

        -- Create index for faster querying
        CREATE NONCLUSTERED INDEX IX_posts_subject_offering_id ON posts (subject_offering_id) 
        WHERE subject_offering_id IS NOT NULL;
        CREATE NONCLUSTERED INDEX IX_posts_section_id ON posts (section_id) 
        WHERE section_id IS NOT NULL;
        CREATE NONCLUSTERED INDEX IX_posts_announcements ON posts (is_announcement)
        WHERE is_announcement = 1;
        PRINT 'Created indexes on posts table.';
    END
    ELSE
        PRINT 'posts table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating posts table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_attachments table for file storage
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_attachments')
    BEGIN
        CREATE TABLE post_attachments (
            attachment_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            file_name NVARCHAR(255) NOT NULL,                -- Original filename
            file_extension NVARCHAR(20) NOT NULL,            -- File extension (e.g., .pdf, .docx)
            file_content VARBINARY(MAX) NOT NULL,            -- Actual file data stored directly in DB
            file_size INT NOT NULL,                          -- Size in bytes
            content_type NVARCHAR(100) NULL,                 -- MIME type
            description NVARCHAR(500) NULL,                  -- Optional description
            upload_date DATETIME DEFAULT GETDATE(),
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE
        );
        PRINT 'Created post_attachments table.';
    END
    ELSE
        PRINT 'post_attachments table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating post_attachments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create comments table
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_comments')
    BEGIN
        CREATE TABLE post_comments (
            comment_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            user_id INT NOT NULL,                           -- Comment author
            parent_comment_id INT NULL,                     -- For nested comments (replies)
            comment_content NVARCHAR(MAX) NOT NULL,         -- Comment text
            is_moderated BIT DEFAULT 0,                     -- To mark if comment has been moderated
            created_at DATETIME DEFAULT GETDATE(),
            updated_at DATETIME NULL,
            is_active BIT DEFAULT 1,
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            FOREIGN KEY (parent_comment_id) REFERENCES post_comments (comment_id)
        );
        PRINT 'Created post_comments table.';

        -- Create index for faster querying
        CREATE NONCLUSTERED INDEX IX_post_comments_post_id ON post_comments (post_id);
        PRINT 'Created index on post_comments table.';
    END
    ELSE
        PRINT 'post_comments table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating post_comments table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_reactions table to track likes, etc.
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_reactions')
    BEGIN
        CREATE TABLE post_reactions (
            reaction_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            user_id INT NOT NULL,
            reaction_type VARCHAR(20) NOT NULL,              -- 'like', 'helpful', etc.
            created_at DATETIME DEFAULT GETDATE(),
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            -- Ensure a user can only have one reaction type per post
            CONSTRAINT UQ_user_post_reaction UNIQUE (user_id, post_id)
        );
        PRINT 'Created post_reactions table.';
    END
    ELSE
        PRINT 'post_reactions table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating post_reactions table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create post_views table to track unique views more accurately
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'post_views')
    BEGIN
        CREATE TABLE post_views (
            view_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NOT NULL,
            user_id INT NOT NULL,
            viewed_at DATETIME DEFAULT GETDATE(),
            FOREIGN KEY (post_id) REFERENCES posts (post_id) ON DELETE CASCADE,
            FOREIGN KEY (user_id) REFERENCES users (user_id),
            -- Ensure a user's view is counted only once
            CONSTRAINT UQ_user_post_view UNIQUE (user_id, post_id)
        );
        PRINT 'Created post_views table.';
    END
    ELSE
        PRINT 'post_views table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating post_views table: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create moderation log table to track post and comment moderation
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'moderation_log')
    BEGIN
        CREATE TABLE moderation_log (
            log_id INT IDENTITY(1,1) PRIMARY KEY,
            post_id INT NULL,
            comment_id INT NULL,
            moderator_id INT NOT NULL,                      -- User who moderated the content
            action_type VARCHAR(50) NOT NULL,               -- 'approve', 'reject', 'edit', etc.
            reason NVARCHAR(500) NULL,                      -- Reason for moderation
            moderated_at DATETIME DEFAULT GETDATE(),
            -- Remove the ON DELETE CASCADE from one or both constraints
            FOREIGN KEY (post_id) REFERENCES posts (post_id),
            FOREIGN KEY (comment_id) REFERENCES post_comments (comment_id),
            FOREIGN KEY (moderator_id) REFERENCES users (user_id),
            -- Ensure either post_id or comment_id is provided
            CONSTRAINT CHK_moderation_target CHECK (
                (post_id IS NOT NULL AND comment_id IS NULL) OR 
                (post_id IS NULL AND comment_id IS NOT NULL)
            )
        );
        PRINT 'Created moderation_log table.';
    END
    ELSE
        PRINT 'moderation_log table already exists.';
END TRY
BEGIN CATCH
    PRINT 'Error creating moderation_log table: ' + ERROR_MESSAGE();
END CATCH

GO

-- Create a trigger to update view_count in posts table whenever a new view is added
BEGIN TRY
    IF OBJECT_ID('UpdatePostViewCount', 'TR') IS NOT NULL
        DROP TRIGGER UpdatePostViewCount;
    
    EXEC('CREATE TRIGGER UpdatePostViewCount
        ON post_views
        AFTER INSERT
        AS
        BEGIN
            SET NOCOUNT ON;
            
            UPDATE p
            SET p.view_count = p.view_count + 1
            FROM posts p
            INNER JOIN inserted i ON p.post_id = i.post_id;
        END');
    
    PRINT 'Created UpdatePostViewCount trigger.';
END TRY
BEGIN CATCH
    PRINT 'Error creating UpdatePostViewCount trigger: ' + ERROR_MESSAGE();
END CATCH
GO

-- =========== STORED PROCEDURES FOR POST SYSTEM ===========

-- Create a stored procedure to create a new post
BEGIN TRY
    IF OBJECT_ID('CreatePost', 'P') IS NOT NULL
        DROP PROCEDURE CreatePost;
        
    PRINT 'Creating CreatePost procedure...';
    
    EXEC('
    CREATE PROCEDURE CreatePost
        @user_id INT,
        @post_title NVARCHAR(200),
        @post_content NVARCHAR(MAX),
        @subject_offering_id INT = NULL,
        @section_id INT = NULL,
        @program_id INT = NULL,
        @is_announcement BIT = 0,
        @is_moderated BIT = 0,
        @is_pinned BIT = 0,
        @created_post_id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that at least one of subject_offering_id or section_id is provided
        IF @subject_offering_id IS NULL AND @section_id IS NULL
        BEGIN
            RAISERROR(''Either subject_offering_id or section_id must be provided'', 16, 1);
            RETURN;
        END
        
        -- Insert the post
        INSERT INTO posts (
            user_id, 
            post_title, 
            post_content, 
            subject_offering_id, 
            section_id, 
            program_id, 
            is_announcement,
            is_moderated,
            is_pinned,
            created_at
        )
        VALUES (
            @user_id, 
            @post_title, 
            @post_content, 
            @subject_offering_id, 
            @section_id, 
            @program_id, 
            @is_announcement,
            @is_moderated,
            @is_pinned,
            GETDATE()
        );
        
        -- Get the ID of the created post
        SET @created_post_id = SCOPE_IDENTITY();
        
        -- Return the created post ID
        SELECT @created_post_id AS post_id;
    END
    ');
    
    PRINT 'Created CreatePost stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating CreatePost procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to add a file attachment to a post
BEGIN TRY
    IF OBJECT_ID('AddPostAttachment', 'P') IS NOT NULL
        DROP PROCEDURE AddPostAttachment;
        
    PRINT 'Creating AddPostAttachment procedure...';
    
    EXEC('
    CREATE PROCEDURE AddPostAttachment
        @post_id INT,
        @file_name NVARCHAR(255),
        @file_extension NVARCHAR(20),
        @file_content VARBINARY(MAX),
        @file_size INT,
        @content_type NVARCHAR(100) = NULL,
        @description NVARCHAR(500) = NULL,
        @created_attachment_id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the post exists
        IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = @post_id)
        BEGIN
            RAISERROR(''Post does not exist'', 16, 1);
            RETURN;
        END
        
        -- Insert the attachment
        INSERT INTO post_attachments (
            post_id,
            file_name,
            file_extension,
            file_content,
            file_size,
            content_type,
            description,
            upload_date
        )
        VALUES (
            @post_id,
            @file_name,
            @file_extension,
            @file_content,
            @file_size,
            @content_type,
            @description,
            GETDATE()
        );
        
        -- Get the ID of the created attachment
        SET @created_attachment_id = SCOPE_IDENTITY();
        
        -- Return the created attachment ID
        SELECT @created_attachment_id AS attachment_id;
    END
    ');
    
    PRINT 'Created AddPostAttachment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating AddPostAttachment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to add a comment to a post
BEGIN TRY
    IF OBJECT_ID('AddPostComment', 'P') IS NOT NULL
        DROP PROCEDURE AddPostComment;
        
    PRINT 'Creating AddPostComment procedure...';
    
    EXEC('
    CREATE PROCEDURE AddPostComment
        @post_id INT,
        @user_id INT,
        @comment_content NVARCHAR(MAX),
        @parent_comment_id INT = NULL,
        @is_moderated BIT = 0,
        @created_comment_id INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the post exists
        IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = @post_id)
        BEGIN
            RAISERROR(''Post does not exist'', 16, 1);
            RETURN;
        END
        
        -- Validate parent comment if provided
        IF @parent_comment_id IS NOT NULL AND 
           NOT EXISTS (SELECT 1 FROM post_comments WHERE comment_id = @parent_comment_id AND post_id = @post_id)
        BEGIN
            RAISERROR(''Parent comment does not exist or does not belong to the specified post'', 16, 1);
            RETURN;
        END
        
        -- Insert the comment
        INSERT INTO post_comments (
            post_id,
            user_id,
            parent_comment_id,
            comment_content,
            is_moderated,
            created_at
        )
        VALUES (
            @post_id,
            @user_id,
            @parent_comment_id,
            @comment_content,
            @is_moderated,
            GETDATE()
        );
        
        -- Get the ID of the created comment
        SET @created_comment_id = SCOPE_IDENTITY();
        
        -- Return the created comment ID
        SELECT @created_comment_id AS comment_id;
    END
    ');
    
    PRINT 'Created AddPostComment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating AddPostComment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to moderate a post
BEGIN TRY
    IF OBJECT_ID('ModeratePost', 'P') IS NOT NULL
        DROP PROCEDURE ModeratePost;
        
    PRINT 'Creating ModeratePost procedure...';
    
    EXEC('
    CREATE PROCEDURE ModeratePost
        @post_id INT,
        @moderator_id INT,
        @action_type VARCHAR(50),
        @reason NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the post exists
        IF NOT EXISTS (SELECT 1 FROM posts WHERE post_id = @post_id)
        BEGIN
            RAISERROR(''Post does not exist'', 16, 1);
            RETURN;
        END
        
        -- Update the post''s moderation status
        BEGIN TRANSACTION;
        
        -- Mark post as moderated
        UPDATE posts 
        SET is_moderated = 1,
            updated_at = GETDATE(),
            -- If action is to deactivate, set is_active to 0
            is_active = CASE WHEN @action_type = ''deactivate'' THEN 0 ELSE is_active END
        WHERE post_id = @post_id;
        
        -- Log the moderation action
        INSERT INTO moderation_log (
            post_id,
            moderator_id,
            action_type,
            reason,
            moderated_at
        )
        VALUES (
            @post_id,
            @moderator_id,
            @action_type,
            @reason,
            GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Return success
        SELECT 1 AS success;
    END
    ');
    
    PRINT 'Created ModeratePost stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating ModeratePost procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to moderate a comment
BEGIN TRY
    IF OBJECT_ID('ModerateComment', 'P') IS NOT NULL
        DROP PROCEDURE ModerateComment;
        
    PRINT 'Creating ModerateComment procedure...';
    
    EXEC('
    CREATE PROCEDURE ModerateComment
        @comment_id INT,
        @moderator_id INT,
        @action_type VARCHAR(50),
        @reason NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Validate that the comment exists
        IF NOT EXISTS (SELECT 1 FROM post_comments WHERE comment_id = @comment_id)
        BEGIN
            RAISERROR(''Comment does not exist'', 16, 1);
            RETURN;
        END
        
        -- Update the comment''s moderation status
        BEGIN TRANSACTION;
        
        -- Mark comment as moderated
        UPDATE post_comments 
        SET is_moderated = 1,
            updated_at = GETDATE(),
            -- If action is to deactivate, set is_active to 0
            is_active = CASE WHEN @action_type = ''deactivate'' THEN 0 ELSE is_active END
        WHERE comment_id = @comment_id;
        
        -- Log the moderation action
        INSERT INTO moderation_log (
            comment_id,
            moderator_id,
            action_type,
            reason,
            moderated_at
        )
        VALUES (
            @comment_id,
            @moderator_id,
            @action_type,
            @reason,
            GETDATE()
        );
        
        COMMIT TRANSACTION;
        
        -- Return success
        SELECT 1 AS success;
    END
    ');
    
    PRINT 'Created ModerateComment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating ModerateComment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to get posts by subject offering
BEGIN TRY
    IF OBJECT_ID('GetPostsBySubjectOffering', 'P') IS NOT NULL
        DROP PROCEDURE GetPostsBySubjectOffering;
        
    PRINT 'Creating GetPostsBySubjectOffering procedure...';
    
    EXEC('
    CREATE PROCEDURE GetPostsBySubjectOffering
        @subject_offering_id INT,
        @page_number INT = 1,
        @page_size INT = 10,
        @include_inactive BIT = 0,
        @only_announcements BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Get posts for the subject offering
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_comments pc WHERE pc.post_id = p.post_id) AS comment_count,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.subject_offering_id = @subject_offering_id
            AND (p.is_active = 1 OR @include_inactive = 1)
            AND (@only_announcements = 0 OR p.is_announcement = 1)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created GetPostsBySubjectOffering stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetPostsBySubjectOffering procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to get posts by section
BEGIN TRY
    IF OBJECT_ID('GetPostsBySection', 'P') IS NOT NULL
        DROP PROCEDURE GetPostsBySection;
        
    PRINT 'Creating GetPostsBySection procedure...';
    
    EXEC('
    CREATE PROCEDURE GetPostsBySection
        @section_id INT,
        @page_number INT = 1,
        @page_size INT = 10,
        @include_inactive BIT = 0,
        @only_announcements BIT = 0
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Get posts for the section
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_comments pc WHERE pc.post_id = p.post_id) AS comment_count,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.section_id = @section_id
            AND (p.is_active = 1 OR @include_inactive = 1)
            AND (@only_announcements = 0 OR p.is_announcement = 1)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created GetPostsBySection stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetPostsBySection procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a specialized stored procedure to get only announcements
BEGIN TRY
    IF OBJECT_ID('GetAnnouncements', 'P') IS NOT NULL
        DROP PROCEDURE GetAnnouncements;
        
    PRINT 'Creating GetAnnouncements procedure...';
    
    EXEC('
    CREATE PROCEDURE GetAnnouncements
        @section_id INT = NULL,
        @subject_offering_id INT = NULL,
        @program_id INT = NULL,
        @page_number INT = 1,
        @page_size INT = 10
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Get announcements based on filters
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_moderated,
            p.subject_offering_id,
            p.section_id,
            p.program_id,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.is_announcement = 1
            AND p.is_active = 1
            AND (@section_id IS NULL OR p.section_id = @section_id)
            AND (@subject_offering_id IS NULL OR p.subject_offering_id = @subject_offering_id)
            AND (@program_id IS NULL OR p.program_id = @program_id)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created GetAnnouncements stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetAnnouncements procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to get post details with comments
BEGIN TRY
    IF OBJECT_ID('GetPostDetails', 'P') IS NOT NULL
        DROP PROCEDURE GetPostDetails;
        
    PRINT 'Creating GetPostDetails procedure...';
    
    EXEC('
    CREATE PROCEDURE GetPostDetails
        @post_id INT,
        @user_id INT = NULL  -- Optional: to record view and check if user has reacted
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Get post details
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            p.subject_offering_id,
            p.section_id,
            p.program_id,
            u.user_id AS author_id,
            u.first_name + '' '' + u.last_name AS author_name,
            CASE WHEN @user_id IS NOT NULL AND EXISTS (
                SELECT 1 FROM post_reactions pr 
                WHERE pr.post_id = p.post_id AND pr.user_id = @user_id
            ) THEN 1 ELSE 0 END AS user_has_reacted,
            (SELECT COUNT(*) FROM post_reactions pr WHERE pr.post_id = p.post_id) AS reaction_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            p.post_id = @post_id;
            
        -- Get post comments
        SELECT 
            pc.comment_id,
            pc.post_id,
            pc.parent_comment_id,
            pc.comment_content,
            pc.created_at,
            pc.updated_at,
            pc.is_moderated,
            u.user_id,
            u.first_name + '' '' + u.last_name AS commenter_name
        FROM 
            post_comments pc
            INNER JOIN users u ON pc.user_id = u.user_id
        WHERE 
            pc.post_id = @post_id
            AND pc.is_active = 1
        ORDER BY 
            pc.created_at ASC;
            
        -- Get post attachments
        SELECT 
            pa.attachment_id,
            pa.post_id,
            pa.file_name,
            pa.file_extension,
            pa.file_size,
            pa.content_type,
            pa.description,
            pa.upload_date
        FROM 
            post_attachments pa
        WHERE 
            pa.post_id = @post_id;
            
        -- Record user view if user_id is provided
        IF @user_id IS NOT NULL
        BEGIN
            -- Try to insert a view record if one doesn''t already exist
            IF NOT EXISTS (SELECT 1 FROM post_views WHERE post_id = @post_id AND user_id = @user_id)
            BEGIN
                BEGIN TRY
                    INSERT INTO post_views (post_id, user_id, viewed_at)
                    VALUES (@post_id, @user_id, GETDATE());
                END TRY
                BEGIN CATCH
                    -- Handle duplicate key error quietly - this means the view was already recorded
                    IF ERROR_NUMBER() <> 2601 AND ERROR_NUMBER() <> 2627
                        THROW;
                END CATCH
            END
        END
    END
    ');
    
    PRINT 'Created GetPostDetails stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating GetPostDetails procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a stored procedure to search posts
BEGIN TRY
    IF OBJECT_ID('SearchPosts', 'P') IS NOT NULL
        DROP PROCEDURE SearchPosts;
        
    PRINT 'Creating SearchPosts procedure...';
    
    EXEC('
    CREATE PROCEDURE SearchPosts
        @search_term NVARCHAR(100),
        @program_id INT = NULL,
        @section_id INT = NULL,
        @subject_offering_id INT = NULL,
        @only_announcements BIT = 0,
        @page_number INT = 1,
        @page_size INT = 10
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Calculate the starting row
        DECLARE @offset INT = (@page_number - 1) * @page_size;
        
        -- Search posts
        SELECT 
            p.post_id,
            p.post_title,
            p.post_content,
            p.created_at,
            p.updated_at,
            p.view_count,
            p.is_pinned,
            p.is_announcement,
            p.is_moderated,
            p.subject_offering_id,
            p.section_id,
            p.program_id,
            u.user_id,
            u.first_name + '' '' + u.last_name AS author_name,
            (SELECT COUNT(*) FROM post_comments pc WHERE pc.post_id = p.post_id) AS comment_count,
            (SELECT COUNT(*) FROM post_attachments pa WHERE pa.post_id = p.post_id) AS attachment_count
        FROM 
            posts p
            INNER JOIN users u ON p.user_id = u.user_id
        WHERE 
            (p.post_title LIKE ''%'' + @search_term + ''%'' OR p.post_content LIKE ''%'' + @search_term + ''%'')
            AND p.is_active = 1
            AND (@program_id IS NULL OR p.program_id = @program_id)
            AND (@section_id IS NULL OR p.section_id = @section_id)
            AND (@subject_offering_id IS NULL OR p.subject_offering_id = @subject_offering_id)
            AND (@only_announcements = 0 OR p.is_announcement = 1)
        ORDER BY 
            p.is_pinned DESC,
            p.created_at DESC
        OFFSET @offset ROWS
        FETCH NEXT @page_size ROWS ONLY;
    END
    ');
    
    PRINT 'Created SearchPosts stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating SearchPosts procedure: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create a procedure to download a file attachment
BEGIN TRY
    IF OBJECT_ID('DownloadPostAttachment', 'P') IS NOT NULL
        DROP PROCEDURE DownloadPostAttachment;
        
    PRINT 'Creating DownloadPostAttachment procedure...';
    
    EXEC('
    CREATE PROCEDURE DownloadPostAttachment
        @attachment_id INT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Return file data and metadata
        SELECT 
            file_name,
            file_extension,
            file_content,
            file_size,
            content_type
        FROM 
            post_attachments
        WHERE 
            attachment_id = @attachment_id;
    END
    ');
    
    PRINT 'Created DownloadPostAttachment stored procedure.';
END TRY
BEGIN CATCH
    PRINT 'Error creating DownloadPostAttachment procedure: ' + ERROR_MESSAGE();
END CATCH
GO

PRINT '';
PRINT '======================================================================';
PRINT 'V9 SCHEMA installation complete!';
PRINT 'Successfully added post system with:';
PRINT '- File attachments';
PRINT '- Commenting functionality';
PRINT '- Announcement capability (is_announcement flag)';
PRINT '- Content moderation (is_moderated flag)';
PRINT '- Moderation logging';
PRINT '======================================================================';
PRINT '';
GO