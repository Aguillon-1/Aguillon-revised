-- =================================================================
-- Manual user checker: Shows account info/credentials, personal, and student info for user_id = 1
-- =================================================================

SELECT
    -- =========================
    -- Account Information & Credentials
    -- =========================
    u.user_id,
    u.username,
    u.password_hash,
    u.email,
    u.is_archived,
    u.user_type,
    u.created_at,
    u.updated_at,

    -- =========================
    -- Personal Information
    -- =========================
    u.first_name +
        CASE WHEN u.middle_name IS NOT NULL AND u.middle_name <> '' THEN ' ' + u.middle_name ELSE '' END +
        ' ' + u.last_name +
        CASE WHEN u.suffix IS NOT NULL AND u.suffix <> '' THEN ' ' + u.suffix ELSE '' END
        AS full_name,
    u.first_name,
    u.middle_name,
    u.last_name,
    u.suffix,
    u.birthday,
    u.sex,
    u.contact_number,
    u.address,

    -- =========================
    -- Student Information (if applicable)
    -- =========================
    sp.student_id AS student_number,
    p.program_name,
    yl.year_name AS year_level,
    s.section_name,
    sp.student_status

FROM users u
LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
LEFT JOIN programs p ON sp.program_id = p.program_id
LEFT JOIN year_levels yl ON sp.year_level_id = yl.year_level_id
LEFT JOIN sections s ON sp.section_id = s.section_id
WHERE u.user_id = 1;

PRINT 'Manual user checker: User information query executed for user_id = 1 (ordered by account, personal, student info)';
GO