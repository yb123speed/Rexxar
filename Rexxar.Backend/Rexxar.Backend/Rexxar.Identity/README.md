# Rexxar Identity

## Database Migration
```powershell
Add-Migration init -Context RexxarDbContext -OutputDir Migrations/IdentityDbMigrations
Add-Migration init -Context ConfigurationDbContext -OutputDir Migrations/ConfigurationDbMigrations
Add-Migration init -Context PersistedGrantDbContext -OutputDir Migrations/PersistedGrantDbMigrations
```