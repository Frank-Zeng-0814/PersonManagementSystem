-- Check migrations history
SELECT * FROM "__EFMigrationsHistory";

-- Check existing tables
SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';
