namespace MarketingPlatform.Core.Enums
{
    [Flags]
    public enum Permission
    {
        // Campaign Permissions
        ViewCampaigns = 1 << 0,           // 1
        CreateCampaigns = 1 << 1,         // 2
        EditCampaigns = 1 << 2,           // 4
        DeleteCampaigns = 1 << 3,         // 8
        
        // Contact Permissions
        ViewContacts = 1 << 4,            // 16
        CreateContacts = 1 << 5,          // 32
        EditContacts = 1 << 6,            // 64
        DeleteContacts = 1 << 7,          // 128
        
        // Template Permissions
        ViewTemplates = 1 << 8,           // 256
        CreateTemplates = 1 << 9,         // 512
        EditTemplates = 1 << 10,          // 1024
        DeleteTemplates = 1 << 11,        // 2048
        
        // Analytics Permissions
        ViewAnalytics = 1 << 12,          // 4096
        ViewDetailedAnalytics = 1 << 13,  // 8192
        ExportAnalytics = 1 << 14,        // 16384
        
        // User Management Permissions
        ViewUsers = 1 << 15,              // 32768
        CreateUsers = 1 << 16,            // 65536
        EditUsers = 1 << 17,              // 131072
        DeleteUsers = 1 << 18,            // 262144
        
        // Role Management Permissions
        ViewRoles = 1 << 19,              // 524288
        ManageRoles = 1 << 20,            // 1048576
        
        // Settings Permissions
        ViewSettings = 1 << 21,           // 2097152
        ManageSettings = 1 << 22,         // 4194304
        
        // Compliance Permissions
        ViewCompliance = 1 << 23,         // 8388608
        ManageCompliance = 1 << 24,       // 16777216
        
        // Workflow Permissions
        ViewWorkflows = 1 << 25,          // 33554432
        CreateWorkflows = 1 << 26,        // 67108864
        EditWorkflows = 1 << 27,          // 134217728
        DeleteWorkflows = 1 << 28,        // 268435456
        
        // Audit Log Permissions
        ViewAuditLogs = 1 << 29,          // 536870912
        
        // Super Admin Permissions
        ViewSuperAdminLogs = 1 << 30,     // 1073741824
        ManageSuperAdmins = 1 << 31,      // 2147483648
        ViewPlatformConfig = 1 << 32,     // 4294967296
        ManagePlatformConfig = 1 << 33,   // 8589934592
        
        // All Permissions (SuperAdmin)
        All = ~0
    }
}
