-- Loader for full curriculum with unique subject_code handling, logging, and robust error handling
-- Designed for the V10 schema (subject_code is globally unique in 'subjects')
-- 2025-04-16

SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
  BEGIN TRANSACTION;

  -- Drop temp tables if they already exist
  IF OBJECT_ID('tempdb..#curriculum_data') IS NOT NULL DROP TABLE #curriculum_data;
  IF OBJECT_ID('tempdb..#subject_batch') IS NOT NULL DROP TABLE #subject_batch;
  IF OBJECT_ID('tempdb..#subject_log') IS NOT NULL DROP TABLE #subject_log;

  -- Log mapping: original code+name --> final code
  CREATE TABLE #subject_log (
    orig_subject_code VARCHAR(32),
    orig_subject_name VARCHAR(255),
    final_subject_code VARCHAR(32)
  );

  -- All unique subjects to process (with resolved code)
  CREATE TABLE #subject_batch (
    orig_subject_code VARCHAR(32),
    orig_subject_name VARCHAR(255),
    lecture_units INT,
    lab_units INT,
    final_subject_code VARCHAR(32)
  );

  -- All curriculum data
  CREATE TABLE #curriculum_data (
    program_code VARCHAR(16),
    year_level VARCHAR(32),
    semester_name VARCHAR(32),
    subject_code VARCHAR(32),
    subject_name VARCHAR(255),
    lecture_units INT,
    lab_units INT
  );

  -- ===================================================================
  -- BSIT 1st Year First Semester
  -- ===================================================================
  INSERT INTO #curriculum_data VALUES
  ('BSIT', 'Year 1', 'First Semester', 'CC 101', 'Introduction to Computing', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'CC 102', 'Computer Programming 1', 5, 5),
  ('BSIT', 'Year 1', 'First Semester', 'CC 109', 'Business Application Software', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'GEC 001', 'Understanding the Self', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'GEC 002', 'Readings in the Philippine History', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'GEC 004', 'Mathematics in the Modern World', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'GEE 002', 'Living in the IT Era', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'NSTP 111', 'Civic Welfare Training Services 1', 3, 0),
  ('BSIT', 'Year 1', 'First Semester', 'PATHFIT 1', 'Movement Competency Training', 2, 0),

  -- BSIT 1st Year Second Semester
  ('BSIT', 'Year 1', 'Second Semester', 'CC 103', 'Computer Programming 2', 5, 5),
  ('BSIT', 'Year 1', 'Second Semester', 'CC 108', 'Technical Computer Concepts', 0, 3),
  ('BSIT', 'Year 1', 'Second Semester', 'CSM 121', 'College Algebra', 3, 0),
  ('BSIT', 'Year 1', 'Second Semester', 'GEC 003', 'The Contemporary World', 3, 0),
  ('BSIT', 'Year 1', 'Second Semester', 'GEE 003', 'Gender and Society', 3, 0),
  ('BSIT', 'Year 1', 'Second Semester', 'ITE 101', 'Web Systems and Technologies', 5, 5),
  ('BSIT', 'Year 1', 'Second Semester', 'ITP 101', 'Information Technology Fundamentals', 3, 0),
  ('BSIT', 'Year 1', 'Second Semester', 'NSTP 122', 'Civic Welfare Training Services 2', 3, 0),
  ('BSIT', 'Year 1', 'Second Semester', 'PATHFIT 2', 'Exercise-Based Fitness Activities', 2, 0),

  -- BSIT 2nd Year First Semester
  ('BSIT', 'Year 2', 'First Semester', 'CC 104', 'Data Structure and Algorithms', 0, 3),
  ('BSIT', 'Year 2', 'First Semester', 'CC 105', 'Information Management', 0, 5),
  ('BSIT', 'Year 2', 'First Semester', 'CC 110', 'Digital Graphics', 0, 3),
  ('BSIT', 'Year 2', 'First Semester', 'CC 123', 'Discrete Mathematics', 3, 0),
  ('BSIT', 'Year 2', 'First Semester', 'CSM 315', 'Probability and Statistics', 3, 0),
  ('BSIT', 'Year 2', 'First Semester', 'ITP 102', 'Object-Oriented Programming', 5, 5),
  ('BSIT', 'Year 2', 'First Semester', 'ITP 103', 'Networking 1', 3, 0),
  ('BSIT', 'Year 2', 'First Semester', 'PATHFIT 3', 'Dance and Fitness', 2, 0),

  -- BSIT 2nd Year Second Semester
  ('BSIT', 'Year 2', 'Second Semester', 'CC 106', 'Application Development and Emerging Tech', 0, 5),
  ('BSIT', 'Year 2', 'Second Semester', 'ITP 104', 'Integrative Programming and Technologies', 5, 5),
  ('BSIT', 'Year 2', 'Second Semester', 'ITP 105', 'Fundamentals of Database Systems', 5, 5),
  ('BSIT', 'Year 2', 'Second Semester', 'ITP 106', 'Logic Design and Switching', 3, 0),
  ('BSIT', 'Year 2', 'Second Semester', 'ITP 107', 'Networking 2', 3, 0),
  ('BSIT', 'Year 2', 'Second Semester', 'PATHFIT 4', 'Sports and Fitness', 2, 0),

  -- BSIT 3rd Year First Semester
  ('BSIT', 'Year 3', 'First Semester', 'CC 111', 'Systems Analysis and Design', 3, 0),
  ('BSIT', 'Year 3', 'First Semester', 'CC 113', 'Information Assurance and Security 1', 3, 0),
  ('BSIT', 'Year 3', 'First Semester', 'ITE 102', 'IT Major Elective 1', 3, 0),
  ('BSIT', 'Year 3', 'First Semester', 'ITP 108', 'Event-Driven Programming', 5, 5),
  ('BSIT', 'Year 3', 'First Semester', 'ITP 109', 'Advanced Database Systems', 5, 5),
  ('BSIT', 'Year 3', 'First Semester', 'ITP 110', 'Systems Integration and Architecture', 3, 0),
  ('BSIT', 'Year 3', 'First Semester', 'LIT 001', 'Literature in English', 3, 0),

  -- BSIT 3rd Year Second Semester
  ('BSIT', 'Year 3', 'Second Semester', 'CSR 101', 'Computer Studies Research', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'GEC 007', 'Science, Technology & Society', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'GEE 005', 'Reading Visual Art', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'ITE 103', 'IT Major Elective 2', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'ITP 111', 'Information Assurance and Security 2', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'ITP 113', 'Software Engineering', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'ITP 115', 'System Administration and Maintenance', 3, 0),
  ('BSIT', 'Year 3', 'Second Semester', 'ITP 116', 'Introduction to Human Computer Interaction', 3, 0),

  -- BSIT Summer
  ('BSIT', 'Year 3', 'Summer', 'CC 114A', 'Practicum 1', 6, 0),

  -- BSIT 4th Year First Semester
  ('BSIT', 'Year 4', 'First Semester', 'CC 119', 'Capstone Project 1', 5, 0),
  ('BSIT', 'Year 4', 'First Semester', 'GEC 006', 'Art Appreciation', 3, 0),
  ('BSIT', 'Year 4', 'First Semester', 'GEC 008A', 'Ethics with Peace Studies', 3, 0),
  ('BSIT', 'Year 4', 'First Semester', 'ITE 104', 'IT Major Elective 3', 3, 0),
  ('BSIT', 'Year 4', 'First Semester', 'ITP 112', 'Quantitative Methods', 3, 0),
  ('BSIT', 'Year 4', 'First Semester', 'ITP 114', 'Social and Professional Issues', 3, 0),

  -- BSIT 4th Year Second Semester
  ('BSIT', 'Year 4', 'Second Semester', 'CC 115', 'Current Trends in IT and Seminars', 3, 0),
  ('BSIT', 'Year 4', 'Second Semester', 'CC 121', 'Capstone Project 2', 5, 0),
  ('BSIT', 'Year 4', 'Second Semester', 'CCS 114B', 'Practicum 2', 3, 0),
  ('BSIT', 'Year 4', 'Second Semester', 'GEM 001', 'Life and Works of Rizal', 3, 0),
  ('BSIT', 'Year 4', 'Second Semester', 'ITE 105', 'IT Major Elective 4', 3, 0),

  -- ===================================================================
  -- BSIS 1st Year First Semester
  -- ===================================================================
  ('BSIS', 'Year 1', 'First Semester', 'CC 101', 'INTRODUCTION TO COMPUTING', 3, 0),
  ('BSIS', 'Year 1', 'First Semester', 'CC 102', 'COMPUTER PROGRAMMING 1', 5, 5),
  ('BSIS', 'Year 1', 'First Semester', 'CC 109', 'BUSINESS APPLICATION SOFTWARE', 3, 0),
  ('BSIS', 'Year 1', 'First Semester', 'GEC 001', 'UNDERSTANDING THE SELF', 3, 0),
  ('BSIS', 'Year 1', 'First Semester', 'GEC 002', 'READINGS IN THE PHILIPPINE HISTORY WITH INDIGENOUS PEOPLE''S STUDIES', 3, 0),
  ('BSIS', 'Year 1', 'First Semester', 'GEC 003', 'THE CONTEMPORARY WORLD', 3, 0),
  ('BSIS', 'Year 1', 'First Semester', 'NSTP 111', 'NSTP - CWTS 1', 3, 0),
  ('BSIS', 'Year 1', 'First Semester', 'PATHFIT 1', 'MOVEMENT COMPETENCY TRAINING', 2, 0),

  -- BSIS 1st Year Second Semester
  ('BSIS', 'Year 1', 'Second Semester', 'CC 103', 'COMPUTER PROGRAMMING 2', 5, 5),
  ('BSIS', 'Year 1', 'Second Semester', 'CC 108', 'TECHNICAL COMPUTER CONCEPTS', 0, 3),
  ('BSIS', 'Year 1', 'Second Semester', 'CCS 107', 'WEB DEVELOPMENT 1', 0, 5),
  ('BSIS', 'Year 1', 'Second Semester', 'GEC 008', 'ETHICS', 3, 0),
  ('BSIS', 'Year 1', 'Second Semester', 'ISP 101', 'FUNDAMENTALS OF INFORMATION SYSTEM', 3, 0),
  ('BSIS', 'Year 1', 'Second Semester', 'NSTP 122', 'CIVIC WELFARE TRAINING SERVICES 2', 3, 0),
  ('BSIS', 'Year 1', 'Second Semester', 'PATHFIT 2', 'EXERCISE-BASED FITNESS ACTIVITIES', 2, 0),
  ('BSIS', 'Year 1', 'Second Semester', 'PR 001', 'COLLEGE ALGEBRA', 3, 0),

  -- BSIS 2nd Year First Semester
  ('BSIS', 'Year 2', 'First Semester', 'CC 104', 'DATA STRUCTURE AND ALGORITHMS', 0, 3),
  ('BSIS', 'Year 2', 'First Semester', 'CC 105', 'INFORMATION MANAGEMENT', 0, 5),
  ('BSIS', 'Year 2', 'First Semester', 'CC 111', 'SYSTEMS ANALYSIS AND DESIGN', 3, 0),
  ('BSIS', 'Year 2', 'First Semester', 'CC 112', 'OPERATING SYSTEMS & APPLICATION', 3, 0),
  ('BSIS', 'Year 2', 'First Semester', 'GEC 005', 'PURPOSIVE COMMUNICATION', 3, 0),
  ('BSIS', 'Year 2', 'First Semester', 'GEC 007', 'SCIENCE, TECHNOLOGY & SOCIETY', 3, 0),
  ('BSIS', 'Year 2', 'First Semester', 'ISP 101', 'FINANCIAL MANAGEMENT', 3, 0),
  ('BSIS', 'Year 2', 'First Semester', 'PATHFIT 3', 'DANCE AND FITNESS', 2, 0),

  -- BSIS 2nd Year Second Semester
  ('BSIS', 'Year 2', 'Second Semester', 'CC 106', 'APPLICATION DEVELOPMENT AND EMERGING TECHNOLOGIES', 0, 5),
  ('BSIS', 'Year 2', 'Second Semester', 'CC 110', 'DIGITAL GRAPHICS', 0, 3),
  ('BSIS', 'Year 2', 'Second Semester', 'CC 116', 'ADVANCED WEB SYSTEMS AND TECHNOLOGIES', 0, 5),
  ('BSIS', 'Year 2', 'Second Semester', 'GEE 002', 'LIVING IN THE IT ERA', 3, 0),
  ('BSIS', 'Year 2', 'Second Semester', 'GEE 005', 'READING VISUAL ART', 3, 0),
  ('BSIS', 'Year 2', 'Second Semester', 'PE 004', 'TEAM SPORTS', 2, 0),
  ('BSIS', 'Year 2', 'Second Semester', 'PR 002', 'QUANTITATIVE METHODS', 3, 0),

  -- BSIS 3rd Year First Semester
  ('BSIS', 'Year 3', 'First Semester', 'CCS 118', 'MULTIMEDIA SYSTEMS', 3, 0),
  ('BSIS', 'Year 3', 'First Semester', 'GEE 003', 'GENDER AND SOCIETY', 3, 0),
  ('BSIS', 'Year 3', 'First Semester', 'IS 102', 'ENTERPRISE RESOURCE PLANNING', 3, 0),
  ('BSIS', 'Year 3', 'First Semester', 'IS 103', 'DATABASE SYSTEM ENTERPRISE', 5, 0),
  ('BSIS', 'Year 3', 'First Semester', 'IS 104', 'IS INNOVATIONS & NEW TECHNOLOGIES', 3, 0),
  ('BSIS', 'Year 3', 'First Semester', 'IS 105', 'ENTERPRISE ARCHITECTURE', 3, 0),
  ('BSIS', 'Year 3', 'First Semester', 'IS 106', 'IS MAJOR ELECTIVE 1', 3, 0),
  ('BSIS', 'Year 3', 'First Semester', 'RES 001', 'METHODS OF RESEARCH', 3, 0),

  -- BSIS 3rd Year Second Semester
  ('BSIS', 'Year 3', 'Second Semester', 'CCS 111', 'SOFTWARE ENGINEERING', 3, 0),
  ('BSIS', 'Year 3', 'Second Semester', 'GEC 006', 'ART APPRECIATION', 3, 0),
  ('BSIS', 'Year 3', 'Second Semester', 'IS 107', 'ORGANIZATION AND MANAGEMENT CONCEPT', 3, 0),
  ('BSIS', 'Year 3', 'Second Semester', 'IS 108', 'DATA MINING', 0, 3),
  ('BSIS', 'Year 3', 'Second Semester', 'IS 109', 'CUSTOMER RELATION MANAGEMENT', 3, 0),
  ('BSIS', 'Year 3', 'Second Semester', 'IS 110', 'IS ELECTIVE 2', 0, 3),
  ('BSIS', 'Year 3', 'Second Semester', 'IS 118', 'IT INFRASTRUCTURE AND NETWORK TECHNOLOGIES', 0, 3),
  ('BSIS', 'Year 3', 'Second Semester', 'IS 119', 'PROFESSIONAL ISSUES IN INFORMATION SYSTEM', 3, 0),

  -- BSIS 4th Year First Semester
  ('BSIS', 'Year 4', 'First Semester', 'CCS 119', 'CAPSTONE PROJECT 1', 5, 0),
  ('BSIS', 'Year 4', 'First Semester', 'CCS 122', 'IS PROJECT MANAGEMENT', 3, 0),
  ('BSIS', 'Year 4', 'First Semester', 'GEM 001', 'RIZAL''S LIFE, WORKS AND WRITINGS', 3, 0),
  ('BSIS', 'Year 4', 'First Semester', 'IS 111', 'IS STRATEGY MANAGEMENT AND ACQUISITION', 3, 0),
  ('BSIS', 'Year 4', 'First Semester', 'IS 112', 'IT SECURITY AND MANAGEMENT', 3, 0),
  ('BSIS', 'Year 4', 'First Semester', 'IS 114', 'IS MAJOR ELECTIVE 3', 3, 0),
  ('BSIS', 'Year 4', 'First Semester', 'IS 120', 'BUSINESS ETHICS', 3, 0),

  -- BSIS 4th Year Second Semester
  ('BSIS', 'Year 4', 'Second Semester', 'CCS 114B', 'PRACTICUM 2', 3, 0),
  ('BSIS', 'Year 4', 'Second Semester', 'CCS 115', 'CURRENT TRENDS IN IT AND SEMINARS', 3, 0),
  ('BSIS', 'Year 4', 'Second Semester', 'CCS 120', 'CAPSTONE PROJECT AND RESEARCH 2', 0, 5),
  ('BSIS', 'Year 4', 'Second Semester', 'CCS 121', 'CAPSTONE PROJECT 2', 5, 0),
  ('BSIS', 'Year 4', 'Second Semester', 'IS 423', 'IT SERVICE MANAGEMENT', 0, 3),
  ('BSIS', 'Year 4', 'Second Semester', 'ISE 110', 'IS MAJOR ELECTIVE 4', 3, 0),
  ('BSIS', 'Year 4', 'Second Semester', 'ISP 110', 'IS ADVANCED SPECIAL TOPICS', 3, 0),

  -- ===================================================================
  -- BSCS 1st Year First Semester
  -- ===================================================================
  ('BSCS', 'Year 1', 'First Semester', 'CC 101', 'INTRODUTION TO COMPUTING', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'CC 102', 'COMPUTER PROGRAMMING 1', 5, 5),
  ('BSCS', 'Year 1', 'First Semester', 'CC 109', 'BUSINESS APPLICATION SOFTWARE', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'GEC 001', 'UNDERSTANDING THE SELF', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'GEC 002', 'READINGS IN THE PHILLIPINE HISTORY WITH INDIGENOUS PEOPLE''S STUDIES', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'GEC 004', 'MATHEMATHICS IN THE MODERN WORLD', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'GEC 005', 'PURPOSIVE COMMUNICATION', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'NSTP 111', 'CIVIC WELFARE TRAINING SERVICES 1', 3, 0),
  ('BSCS', 'Year 1', 'First Semester', 'PATHFIT 1', 'MOVEMENT COMPETENCY TRAINING', 3, 0),

  -- BSCS 1st Year Second Semester
  ('BSCS', 'Year 1', 'Second Semester', 'CC 103', 'COMPUTER PROGRAMMING 2', 5, 5),
  ('BSCS', 'Year 1', 'Second Semester', 'CC 104', 'WEB SYSTEM AND TECHNOLOGIES', 0, 5),
  ('BSCS', 'Year 1', 'Second Semester', 'CC 108', 'TECHNICAL COMPUTER CONCEPTS', 0, 3),
  ('BSCS', 'Year 1', 'Second Semester', 'CSM 121', 'COLLEGE ALGEBRA', 3, 0),
  ('BSCS', 'Year 1', 'Second Semester', 'GEC 003', 'THE CONTEMPORARY WORLD', 3, 0),
  ('BSCS', 'Year 1', 'Second Semester', 'LIT 001', 'PHILLIPINE LITERATURE IN ENGLISH', 3, 0),
  ('BSCS', 'Year 1', 'Second Semester', 'NSTP 122', 'CIVIC WELFARE TRAINING SERVICES 2', 3, 0),
  ('BSCS', 'Year 1', 'Second Semester', 'PATHFIT 2', 'MOVEMENT COMPETENCY TRAINING', 2, 0),

  -- BSCS 2nd Year First Semester
  ('BSCS', 'Year 2', 'First Semester', 'CC 104', 'DATA STRUCTURE AND ALGORITHMS', 0, 3),
  ('BSCS', 'Year 2', 'First Semester', 'CC 105', 'INFORMATION MANAGEMENT', 0, 5),
  ('BSCS', 'Year 2', 'First Semester', 'CSM 212', 'DIFFERENTIAL CALCULUS', 3, 0),
  ('BSCS', 'Year 2', 'First Semester', 'CSP 101', 'OBJECT ORIENTED PROGRAMMING', 0, 5),
  ('BSCS', 'Year 2', 'First Semester', 'CSP 102', 'LOGIC CIRCUIT AND SWITCHING THEORY', 0, 3),
  ('BSCS', 'Year 2', 'First Semester', 'CSP 103', 'DISCRETE STRUCTURES 1', 3, 0),
  ('BSCS', 'Year 2', 'First Semester', 'PATHFIT 3', 'PHILIPPINE FOLK DANCE', 2, 0),

  -- BSCS 2nd Year Second Semester
  ('BSCS', 'Year 2', 'Second Semester', 'CC 110', 'DIGITAL GRAPHICS', 0, 3),
  ('BSCS', 'Year 2', 'Second Semester', 'CSM 223', 'INTEGRAL CALCULUS', 3, 0),
  ('BSCS', 'Year 2', 'Second Semester', 'CSP 104', 'DISCRETE STRUCTURES 2', 3, 0),
  ('BSCS', 'Year 2', 'Second Semester', 'CSP 105', 'PROGRAMMING LANGUAGES', 0, 5),
  ('BSCS', 'Year 2', 'Second Semester', 'CSP 106', 'EMBEDDED PROGRAMMING', 0, 5),
  ('BSCS', 'Year 2', 'Second Semester', 'CSP 115', 'HUMAN COMPUTER INTERACTION', 0, 3),
  ('BSCS', 'Year 2', 'Second Semester', 'PATHFIT 4', 'SPORTS AND FITNESS', 2, 0),

  -- BSCS 3rd Year First Semester
  ('BSCS', 'Year 3', 'First Semester', 'CC 116', 'ADVANCED WEB SYSTEMS AND TECHNOLOGIES', 0, 5),
  ('BSCS', 'Year 3', 'First Semester', 'CC 125', 'SOFTWARE ENGINEERING 1', 3, 0),
  ('BSCS', 'Year 3', 'First Semester', 'CSE 101', 'SYSTEM FUNDAMENTALS', 3, 0),
  ('BSCS', 'Year 3', 'First Semester', 'CSP 108', 'NETWORKS AND COMMUNICATIONS', 3, 0),
  ('BSCS', 'Year 3', 'First Semester', 'CSP 111', 'ALGORITHM AND COMPLEXITY', 0, 3),
  ('BSCS', 'Year 3', 'First Semester', 'GEC 007', 'SCIENCE, TECHNOLOGY & SOCIETY', 3, 0),
  ('BSCS', 'Year 3', 'First Semester', 'GEC 008', 'ETHICS WITH PEACE STUDIES', 3, 0),

  -- BSCS 3rd Year Second Semester
  ('BSCS', 'Year 3', 'Second Semester', 'CC 106', 'APPLICATION DEVELOPMENT AND EMERGING TECHNOLOGIES', 0, 5),
  ('BSCS', 'Year 3', 'Second Semester', 'CSE 102', 'GRAPHICS AND VISUAL COMPUTING', 3, 0),
  ('BSCS', 'Year 3', 'Second Semester', 'CSP 107', 'SOFTWARE ENGINEERING 2', 3, 0),
  ('BSCS', 'Year 3', 'Second Semester', 'CSP 109', 'ARCHITECTURE AND ORGANIZATION', 3, 0),
  ('BSCS', 'Year 3', 'Second Semester', 'CSR 101', 'COMPUTER STUDIES RESEARCH', 3, 0),
  ('BSCS', 'Year 3', 'Second Semester', 'GEC 006', 'ART APPRECIATION', 3, 0),
  ('BSCS', 'Year 3', 'Second Semester', 'GEM 001', 'LIFE AND WORKS OF RIZAL', 3, 0),

  -- BSCS Summer
  ('BSCS', 'Year 3', 'Summer', 'CC 114A', 'PRACTICUM 1', 6, 0),

  -- BSCS 4th Year First Semester
  ('BSCS', 'Year 4', 'First Semester', 'CSE 103', 'COMPUTATIONAL SCIENCE', 3, 0),
  ('BSCS', 'Year 4', 'First Semester', 'CSE 104', 'INTELLIGENT SYSTEMS', 3, 0),
  ('BSCS', 'Year 4', 'First Semester', 'CSP 110', 'AUTOMATA AND LANGUAGE THEORY', 3, 0),
  ('BSCS', 'Year 4', 'First Semester', 'CSP 112', 'CS THESIS WRITING 1', 5, 0),
  ('BSCS', 'Year 4', 'First Semester', 'CSP 114', 'SOCIAL ISSUES AND PROFESSIONAL PRACTICE', 3, 0),

  -- BSCS 4th Year Second Semester
  ('BSCS', 'Year 4', 'Second Semester', 'CC 112', 'OPERATING SYSTEMS & APPLICATION', 3, 0),
  ('BSCS', 'Year 4', 'Second Semester', 'CC 113', 'INFORMATION ASSURANCE AND SECURITY 1', 3, 0),
  ('BSCS', 'Year 4', 'Second Semester', 'CC 114B', 'PRACTICUM 2', 3, 0),
  ('BSCS', 'Year 4', 'Second Semester', 'CC 115', 'CURRENT TRENDS AND IN IT AND SEMINARS', 3, 0),
  ('BSCS', 'Year 4', 'Second Semester', 'CSE 105', 'PARALLEL AND DISTRIBUTED COMPUTING', 3, 0),
  ('BSCS', 'Year 4', 'Second Semester', 'CSP 113', 'CS THESIS WRITING 2', 5, 0),

  -- ===================================================================
  -- BSEMC 1st Year First Semester
  -- ===================================================================
  ('BSEMC', 'Year 1', 'First Semester', 'CC 101', 'INTRODUCTION TO COMPUTING', 3, 0),
  ('BSEMC', 'Year 1', 'First Semester', 'CC 102', 'COMPUTER PROGRAMMING 1', 5, 5),
  ('BSEMC', 'Year 1', 'First Semester', 'EMCP 101', 'DRAFTING', 3, 0),
  ('BSEMC', 'Year 1', 'First Semester', 'GEC 001', 'UNDERSTANDING THE SELF', 3, 0),
  ('BSEMC', 'Year 1', 'First Semester', 'GEC 004', 'MATHEMATICS IN THE MODERN WORLD', 3, 0),
  ('BSEMC', 'Year 1', 'First Semester', 'NSTP 111', 'CIVIC WELFARE TRAINING SERVICES 1', 3, 0),
  ('BSEMC', 'Year 1', 'First Semester', 'PATHFIT 1', 'MOVEMENT COMPETENCY TRAINING', 2, 0),

  -- BSEMC 1st Year Second Semester
  ('BSEMC', 'Year 1', 'Second Semester', 'CC 103', 'COMPUTER PROGRAMMING 2', 5, 5),
  ('BSEMC', 'Year 1', 'Second Semester', 'CC 109', 'BUSINESS APPLICATION SOFTWARE', 3, 0),
  ('BSEMC', 'Year 1', 'Second Semester', 'EMCP 102', 'INTRODUCTION TO GAME DESIGN AND DEVELOPMENT', 0, 5),
  ('BSEMC', 'Year 1', 'Second Semester', 'EMCP 103', 'FREEHAND AND DIGITAL DRAWING', 3, 0),
  ('BSEMC', 'Year 1', 'Second Semester', 'MTH 111C', 'ALGEBRA AND TRIGONOMETRY', 3, 0),
  ('BSEMC', 'Year 1', 'Second Semester', 'NSTP 122', 'CIVIC WELFARE TRAINING SERVICES 2', 3, 0),
  ('BSEMC', 'Year 1', 'Second Semester', 'PATHFIT 2', 'EXERCISE-BASED FITNESS ACTIVITIES', 2, 0),

  -- BSEMC 2nd Year First Semester
  ('BSEMC', 'Year 2', 'First Semester', 'CC 104', 'DATA STRUCTURE AND ALGORITHMS', 0, 3),
  ('BSEMC', 'Year 2', 'First Semester', 'CC 108', 'TECHNICAL COMPUTER CONCEPTS', 0, 3),
  ('BSEMC', 'Year 2', 'First Semester', 'EMCP 104', 'PRINCIPLES OF 2D ANIMATION', 3, 0),
  ('BSEMC', 'Year 2', 'First Semester', 'EMCP 105', 'GAME PROGRAMMING 1', 5, 0),
  ('BSEMC', 'Year 2', 'First Semester', 'EMCP 117', 'USABILITY, HCI, AND USER INTERACTION DESIGN', 0, 3),
  ('BSEMC', 'Year 2', 'First Semester', 'GEE 003', 'GENDER AND SOCIETY', 3, 0),
  ('BSEMC', 'Year 2', 'First Semester', 'MCC 212', 'ANALYTIC GEOMETRY', 3, 0),
  ('BSEMC', 'Year 2', 'First Semester', 'PATHFIT 3', 'DANCE AND FITNESS', 2, 0),

  -- BSEMC 2nd Year Second Semester
  ('BSEMC', 'Year 2', 'Second Semester', 'CC 105A', 'INFORMATION MANAGEMENT', 3, 0),
  ('BSEMC', 'Year 2', 'Second Semester', 'CC 106', 'APPLICATION DEVELOPMENT AND EMERGING TECHNOLOGIES', 0, 5),
  ('BSEMC', 'Year 2', 'Second Semester', 'EMCP 106', 'SCRIPT WRITING AND STORYBOARD DESIGN', 3, 0),
  ('BSEMC', 'Year 2', 'Second Semester', 'EMCP 107', 'GAME PROGRAMMING 2', 0, 5),
  ('BSEMC', 'Year 2', 'Second Semester', 'EMCP 119', 'COMPUTER GRAPHICS PROGRAMMING', 0, 3),
  ('BSEMC', 'Year 2', 'Second Semester', 'GEC 002A', 'READINGS IN THE PHILIPPINE HISTORY WITH INDIGENOUS PEOPLE''S STUDIES', 3, 0),
  ('BSEMC', 'Year 2', 'Second Semester', 'PATHFIT 4', 'SPORTS AND FITNESS', 2, 0),
  ('BSEMC', 'Year 2', 'Second Semester', 'PHY 001', 'PHYSICS 1', 3, 0),

  -- BSEMC 3rd Year First Semester
  ('BSEMC', 'Year 3', 'First Semester', 'CSR 101', 'COMPUTER STUDIES RESEARCH', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'EMCE 101', 'EMC PROFESSIONAL ELECTIVE 1', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'EMCP 108', 'PRINCIPLES OF 3D ANIMATION', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'EMCP 109', 'AUDIO DESIGN AND SOUND ENGINEERING', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'EMCP 110', 'APPLIED MATHEMATICS FOR GAMES', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'EMCP 111', 'ARTIFICIAL INTELLIGENCE (AI) IN GAMES', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'GEC 003', 'THE CONTEMPORARY WORLD', 3, 0),
  ('BSEMC', 'Year 3', 'First Semester', 'GEC 005', 'PURPOSIVE COMMUNICATION', 3, 0),

  -- BSEMC 3rd Year Second Semester
  ('BSEMC', 'Year 3', 'Second Semester', 'EMCE 102', 'EMC PROFESSIONAL ELECTIVE 2', 3, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'EMCP 113', 'DESIGN AND PRODUCTION PROCESS', 3, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'EMCP 114', 'APPLIED GAME PHYSICS', 3, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'EMCP 115', 'GAME NETWORKING', 3, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'EMCP 116', 'GAME PROGRAMMING 3', 5, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'GEC 006', 'ART APPRECIATION', 3, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'GEE 005', 'READING VISUAL ART', 3, 0),
  ('BSEMC', 'Year 3', 'Second Semester', 'GEE 009', 'ENTREPRENEURIAL MIND', 3, 0),

  -- BSEMC Summer
  ('BSEMC', 'Year 3', 'Summer', 'CC 114A', 'PRACTICUM 1', 6, 0),

  -- BSEMC 4th Year First Semester
  ('BSEMC', 'Year 4', 'First Semester', 'CC 119', 'CAPSTONE PROJECT 1', 5, 0),
  ('BSEMC', 'Year 4', 'First Semester', 'CC 122A', 'EMC PROJECT MANAGEMENT', 3, 0),
  ('BSEMC', 'Year 4', 'First Semester', 'EMCE 103', 'EMC PROFESSIONAL ELECTIVE 3', 3, 0),
  ('BSEMC', 'Year 4', 'First Semester', 'EMCP 118', 'ADVANCED GAME DESIGN', 3, 0),
  ('BSEMC', 'Year 4', 'First Semester', 'GEC 007', 'SCIENCE, TECHNOLOGY & SOCIETY', 3, 0),
  ('BSEMC', 'Year 4', 'First Semester', 'GEM 001', 'RIZAL''S LIFE, WORKS AND WRITINGS', 3, 0),

  -- BSEMC 4th Year Second Semester
  ('BSEMC', 'Year 4', 'Second Semester', 'CC 114B', 'PRACTICUM 2', 3, 0),
  ('BSEMC', 'Year 4', 'Second Semester', 'CC 115', 'CURRENT TRENDS AND IN IT AND SEMINARS', 3, 0),
  ('BSEMC', 'Year 4', 'Second Semester', 'CC 121', 'CAPSTONE PROJECT 2', 5, 0),
  ('BSEMC', 'Year 4', 'Second Semester', 'EMCE 104', 'EMC PROFESSIONAL ELECTIVE 4', 3, 0),
  ('BSEMC', 'Year 4', 'Second Semester', 'EMCP 112', 'GAME PRODUCTION', 3, 0),
  ('BSEMC', 'Year 4', 'Second Semester', 'GEC 008A', 'ETHICS WITH PEACE STUDIES', 3, 0);

  -- ===================================================================
  -- END OF CURRICULUM DATA
  -- ===================================================================

  -- Step 1: Assign unique subject codes for each (subject_code, subject_name)
  DECLARE subj_cur CURSOR FOR
    SELECT DISTINCT subject_code, subject_name, lecture_units, lab_units
    FROM #curriculum_data;

  DECLARE @orig_code VARCHAR(32), @orig_name VARCHAR(255), @lecture INT, @lab INT;
  DECLARE @final_code VARCHAR(32), @suffix CHAR(1), @charcode INT, @num INT;

  OPEN subj_cur;
  FETCH NEXT FROM subj_cur INTO @orig_code, @orig_name, @lecture, @lab;
  WHILE @@FETCH_STATUS = 0
  BEGIN
    SET @final_code = @orig_code;
    SET @charcode = 65; -- ASCII 'A'
    SET @num = 1;

    -- Find unique code not used in DB or this batch so far
    WHILE EXISTS (
      SELECT 1 FROM subjects WHERE subject_code = @final_code
      UNION
      SELECT 1 FROM #subject_batch WHERE final_subject_code = @final_code
    )
    BEGIN
      IF @charcode <= 90 -- 'Z'
      BEGIN
        SET @final_code = @orig_code + CHAR(@charcode);
        SET @charcode = @charcode + 1;
      END
      ELSE
      BEGIN
        SET @final_code = @orig_code + CAST(@num AS VARCHAR);
        SET @num = @num + 1;
      END
    END

    INSERT INTO #subject_batch (orig_subject_code, orig_subject_name, lecture_units, lab_units, final_subject_code)
    VALUES (@orig_code, @orig_name, @lecture, @lab, @final_code);

    INSERT INTO #subject_log (orig_subject_code, orig_subject_name, final_subject_code)
    VALUES (@orig_code, @orig_name, @final_code);

    FETCH NEXT FROM subj_cur INTO @orig_code, @orig_name, @lecture, @lab;
  END
  CLOSE subj_cur; DEALLOCATE subj_cur;

  -- Step 2: Insert new subjects with final codes if not already present
  DECLARE ins_cur CURSOR FOR
    SELECT final_subject_code, orig_subject_name, lecture_units, lab_units
    FROM #subject_batch;

  DECLARE @ins_code VARCHAR(32), @ins_name VARCHAR(255), @ins_lec INT, @ins_lab INT, @ins_units INT;

  OPEN ins_cur;
  FETCH NEXT FROM ins_cur INTO @ins_code, @ins_name, @ins_lec, @ins_lab;
  WHILE @@FETCH_STATUS = 0
  BEGIN
    SET @ins_units = ISNULL(@ins_lec, 0) + ISNULL(@ins_lab, 0);
    IF NOT EXISTS (SELECT 1 FROM subjects WHERE subject_code = @ins_code)
    BEGIN
      INSERT INTO subjects(subject_code, subject_name, units, lecture_units, lab_units, is_active)
      VALUES (@ins_code, @ins_name, @ins_units, @ins_lec, @ins_lab, 1);
    END
    FETCH NEXT FROM ins_cur INTO @ins_code, @ins_name, @ins_lec, @ins_lab;
  END
  CLOSE ins_cur; DEALLOCATE ins_cur;

  -- Step 3: Insert curriculum using the resolved subject codes
  DECLARE cur_cur CURSOR FOR
    SELECT c.program_code, c.year_level, c.semester_name, b.final_subject_code, c.subject_name
    FROM #curriculum_data c
    JOIN #subject_batch b ON c.subject_code = b.orig_subject_code AND c.subject_name = b.orig_subject_name;

    DECLARE @program_code VARCHAR(16), @year_level VARCHAR(32), @semester_name VARCHAR(32);
  DECLARE @cur_code VARCHAR(32), @cur_name VARCHAR(255);
  DECLARE @program_id INT, @year_level_id INT, @semester_id INT, @school_year_id INT, @subject_id INT;
  DECLARE @curriculum_year VARCHAR(9) = '2024-2025';

  -- Add this line to get the school_year_id
  SELECT @school_year_id = school_year_id FROM school_years WHERE year_name = @curriculum_year;

  -- Add this check and insertion if school year doesn't exist
  IF @school_year_id IS NULL
  BEGIN
      -- Insert the school year if it doesn't exist
      INSERT INTO school_years (year_name, is_current, start_date, end_date)
      VALUES (@curriculum_year, 0, 
             CAST('2024-06-01' AS DATE), -- Start date for 2024-2025
             CAST('2025-05-31' AS DATE)); -- End date for 2024-2025
             
      -- Get the new school_year_id
      SET @school_year_id = SCOPE_IDENTITY();
  END

  OPEN cur_cur;
  FETCH NEXT FROM cur_cur INTO @program_code, @year_level, @semester_name, @cur_code, @cur_name;

  WHILE @@FETCH_STATUS = 0
  BEGIN
    SELECT @program_id = program_id FROM programs WHERE program_code = @program_code;
    SELECT @year_level_id = year_level_id FROM year_levels WHERE year_name = @year_level;
    SELECT @semester_id = semester_id FROM semesters WHERE semester_name = @semester_name;
    SELECT @subject_id = subject_id FROM subjects WHERE subject_code = @cur_code AND subject_name = @cur_name;

    IF @program_id IS NOT NULL AND @year_level_id IS NOT NULL AND @semester_id IS NOT NULL AND @subject_id IS NOT NULL
    BEGIN
      DELETE FROM curriculum WHERE
        program_id = @program_id AND subject_id = @subject_id
        AND year_level_id = @year_level_id AND semester_id = @semester_id
        AND school_year_id = @school_year_id AND curriculum_year = @curriculum_year;

      INSERT INTO curriculum (
        program_id, subject_id, school_year_id, semester_id, year_level_id,
        curriculum_year, subject_status, faculty_id, is_active, is_default
      )
      VALUES (
        @program_id, @subject_id, @school_year_id, @semester_id, @year_level_id,
        @curriculum_year, 'active', NULL, 1, 1
      );
    END

    FETCH NEXT FROM cur_cur INTO @program_code, @year_level, @semester_name, @cur_code, @cur_name;
  END
  CLOSE cur_cur; DEALLOCATE cur_cur;

  -- Output mapping log
  SELECT * FROM #subject_log;

  DROP TABLE #curriculum_data;
  DROP TABLE #subject_batch;
  DROP TABLE #subject_log;

  COMMIT TRANSACTION;
END TRY
BEGIN CATCH
  PRINT 'ERROR: ' + ERROR_MESSAGE();
  IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
END CATCH