-- Adding schema information for previous versions (V4.0 to V8.1) to schema_version and schema_version_details

-- Check and insert data into schema_version table
INSERT INTO schema_version (version_number, description, applied_date)
SELECT version_number, description, applied_date
FROM (VALUES
    ('4.0.0', 'Initial version with basic user and classroom management functionality.', '2024-01-01'),
    ('5.0.0', 'Introduced school years, sections, and student profiles.', '2024-03-01'),
    ('5.2.0', 'Added curriculum management with program and subject associations.', '2024-05-01'),
    ('6.0.0', 'Enhanced schema with program/subject status and multiple faculty assignments.', '2024-07-01'),
    ('6.2.0', 'Added is_active columns for subjects and subject offerings.', '2024-09-01'),
    ('7.0.0', 'Introduced lecture/lab units and comprehensive curriculum management.', '2024-11-01'),
    ('7.2.0', 'Fixed missing fields in student profiles table.', '2025-01-01'),
    ('7.3.0', 'Enhanced curriculum management with missing columns.', '2025-02-01'),
    ('8.0.0', 'Implemented system-wide configurations and enrollment workflows.', '2025-03-01'),
    ('8.1.0', 'Added default subject enrollment and semester-specific capabilities.', '2025-04-01')
) AS versions(version_number, description, applied_date)
WHERE NOT EXISTS (
    SELECT 1 FROM schema_version sv WHERE sv.version_number = versions.version_number
);

-- Check and insert data into schema_version_details table
INSERT INTO schema_version_details (version_number, feature_description, release_date, changelog)
SELECT version_number, feature_description, release_date, changelog
FROM (VALUES
    ('4.0.0', 'Initial setup with basic tables: users, sections, and programs.', '2024-01-01', 'Setup core tables for user and classroom management.'),
    ('5.0.0', 'Added school years, sections, and student profiles.', '2024-03-01', 'Introduced school year and section tracking. Established basic student profiles.'),
    ('5.2.0', 'Implemented curriculum management with program and subject association.', '2024-05-01', 'Added curriculum and program_subjects tables. Enhanced associations between programs and subjects.'),
    ('6.0.0', 'Improved program/subject management and added faculty assignments.', '2024-07-01', 'Introduced status columns for programs and subjects. Added multiple faculty assignments for subjects.'),
    ('6.2.0', 'Added is_active columns for subjects and subject offerings.', '2024-09-01', 'Enhanced schema with active status tracking for subjects and offerings.'),
    ('7.0.0', 'Added lecture/lab units and comprehensive curriculum management.', '2024-11-01', 'Introduced lecture_units and lab_units columns for subjects. Expanded curriculum management functionality.'),
    ('7.2.0', 'Fixed missing fields in student profiles table.', '2025-01-01', 'Added school_year_id and enrollment_date columns to student_profiles.'),
    ('7.3.0', 'Enhanced curriculum management with missing columns.', '2025-02-01', 'Added program_code and is_active columns to programs. Enhanced year levels and subjects.'),
    ('8.0.0', 'Implemented system-wide configurations and enrollment workflows.', '2025-03-01', 'Introduced configuration table and workflows for enrollment management.'),
    ('8.1.0', 'Added default subject enrollment and semester-specific capabilities.', '2025-04-01', 'Enhanced schema with automatic enrollment and semester-specific default subjects.')
) AS details(version_number, feature_description, release_date, changelog)
WHERE NOT EXISTS (
    SELECT 1 FROM schema_version_details svd WHERE svd.version_number = details.version_number
);