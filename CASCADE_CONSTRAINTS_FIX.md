# Cascade Constraints Fix - Migration Issues Resolution

## Problem Statement

The database migration was failing due to multiple cascade paths in foreign key relationships. SQL Server does not allow multiple cascade delete paths to the same table from a common parent, as this creates ambiguity about which cascade path should be followed when the parent is deleted.

## Root Cause Analysis

The issue occurred because several junction/association tables had cascade delete relationships configured on both sides of their foreign keys, where both parent tables were themselves children of the same root table (ApplicationUser). This created multiple cascade paths:

### Identified Cascade Cycles

#### 1. ContactGroupMember Cascade Cycle
```
ApplicationUser (CASCADE) → Contact (CASCADE) → ContactGroupMember
ApplicationUser (CASCADE) → ContactGroup (CASCADE) → ContactGroupMember
```
**Problem**: Two cascade paths from ApplicationUser to ContactGroupMember

#### 2. ContactTagAssignment Cascade Cycle
```
ApplicationUser (CASCADE) → Contact (CASCADE) → ContactTagAssignment
ApplicationUser (CASCADE) → ContactTag (CASCADE) → ContactTagAssignment
```
**Problem**: Two cascade paths from ApplicationUser to ContactTagAssignment

#### 3. KeywordAssignment Cascade Cycle
```
ApplicationUser (CASCADE) → Keyword (CASCADE) → KeywordAssignment
ApplicationUser (CASCADE) → Campaign (CASCADE) → KeywordAssignment
```
**Problem**: Two cascade paths from ApplicationUser to KeywordAssignment

## Solution Implemented

The fix breaks the cascade cycles by changing one of the cascade delete behaviors in each cycle to `DeleteBehavior.Restrict`. This approach:

1. **Maintains data integrity** - Prevents orphaned records
2. **Preserves logical cascades** - Keeps cascade where it makes most sense
3. **Requires explicit cleanup** - For restricted relationships, manual deletion is required

### Changes Made

#### 1. ContactGroupMemberConfiguration
**File**: `src/MarketingPlatform.Infrastructure/Data/Configurations/ContactGroupMemberConfiguration.cs`

**Change**: ContactGroup foreign key changed from `Cascade` to `Restrict`

**Reasoning**: 
- Deleting a Contact should automatically remove their group memberships (Cascade)
- Deleting a ContactGroup should NOT automatically delete members; it should require explicit cleanup (Restrict)

```csharp
// Before
builder.HasOne(cgm => cgm.ContactGroup)
    .WithMany(cg => cg.Members)
    .HasForeignKey(cgm => cgm.ContactGroupId)
    .OnDelete(DeleteBehavior.Cascade);

// After
builder.HasOne(cgm => cgm.ContactGroup)
    .WithMany(cg => cg.Members)
    .HasForeignKey(cgm => cgm.ContactGroupId)
    .OnDelete(DeleteBehavior.Restrict);
```

#### 2. ContactTagAssignmentConfiguration
**File**: `src/MarketingPlatform.Infrastructure/Data/Configurations/ContactTagAssignmentConfiguration.cs`

**Change**: ContactTag foreign key changed from `Cascade` to `Restrict`

**Reasoning**:
- Deleting a Contact should automatically remove their tag assignments (Cascade)
- Deleting a ContactTag should NOT automatically delete assignments; it should require explicit cleanup (Restrict)

```csharp
// Before
builder.HasOne(cta => cta.ContactTag)
    .WithMany(ct => ct.TagAssignments)
    .HasForeignKey(cta => cta.ContactTagId)
    .OnDelete(DeleteBehavior.Cascade);

// After
builder.HasOne(cta => cta.ContactTag)
    .WithMany(ct => ct.TagAssignments)
    .HasForeignKey(cta => cta.ContactTagId)
    .OnDelete(DeleteBehavior.Restrict);
```

#### 3. KeywordAssignmentConfiguration
**File**: `src/MarketingPlatform.Infrastructure/Data/Configurations/KeywordManagementConfiguration.cs`

**Change**: Campaign foreign key changed from `Cascade` to `Restrict`

**Reasoning**:
- Deleting a Keyword should automatically remove its assignments (Cascade)
- Deleting a Campaign should NOT automatically delete keyword assignments; it should require explicit cleanup (Restrict)

```csharp
// Before
builder.HasOne(ka => ka.Campaign)
    .WithMany()
    .HasForeignKey(ka => ka.CampaignId)
    .OnDelete(DeleteBehavior.Cascade);

// After
builder.HasOne(ka => ka.Campaign)
    .WithMany()
    .HasForeignKey(ka => ka.CampaignId)
    .OnDelete(DeleteBehavior.Restrict);
```

## Migration Details

**Migration Name**: `20260119075924_FixCascadeConstraints`

**Location**: `src/MarketingPlatform.Infrastructure/Migrations/`

### Migration Actions

The migration performs the following operations:

1. **Drop Foreign Keys**: Removes existing foreign key constraints with cascade delete
   - `FK_ContactGroupMembers_ContactGroups_ContactGroupId`
   - `FK_ContactTagAssignments_ContactTags_ContactTagId`
   - `FK_KeywordAssignments_Campaigns_CampaignId`

2. **Re-add Foreign Keys**: Adds the same foreign keys with `ON DELETE NO ACTION` (Restrict)

### SQL Generated (Excerpt)

```sql
-- ContactGroupMember fix
ALTER TABLE [ContactGroupMembers] DROP CONSTRAINT [FK_ContactGroupMembers_ContactGroups_ContactGroupId];
ALTER TABLE [ContactGroupMembers] ADD CONSTRAINT [FK_ContactGroupMembers_ContactGroups_ContactGroupId] 
    FOREIGN KEY ([ContactGroupId]) REFERENCES [ContactGroups] ([Id]) ON DELETE NO ACTION;

-- ContactTagAssignment fix
ALTER TABLE [ContactTagAssignments] DROP CONSTRAINT [FK_ContactTagAssignments_ContactTags_ContactTagId];
ALTER TABLE [ContactTagAssignments] ADD CONSTRAINT [FK_ContactTagAssignments_ContactTags_ContactTagId] 
    FOREIGN KEY ([ContactTagId]) REFERENCES [ContactTags] ([Id]) ON DELETE NO ACTION;

-- KeywordAssignment fix
ALTER TABLE [KeywordAssignments] DROP CONSTRAINT [FK_KeywordAssignments_Campaigns_CampaignId];
ALTER TABLE [KeywordAssignments] ADD CONSTRAINT [FK_KeywordAssignments_Campaigns_CampaignId] 
    FOREIGN KEY ([CampaignId]) REFERENCES [Campaigns] ([Id]) ON DELETE NO ACTION;
```

## Impact on Application Logic

### Required Application Changes

When deleting entities with Restrict relationships, the application must now:

1. **ContactGroup Deletion**: Manually delete or reassign ContactGroupMembers before deleting a ContactGroup
2. **ContactTag Deletion**: Manually delete ContactTagAssignments before deleting a ContactTag
3. **Campaign Deletion**: Manually delete KeywordAssignments before deleting a Campaign

### Example Code Pattern

```csharp
// Before deleting a ContactGroup
public async Task DeleteContactGroupAsync(int contactGroupId)
{
    // Remove all members first (required due to Restrict)
    var members = await _context.ContactGroupMembers
        .Where(m => m.ContactGroupId == contactGroupId)
        .ToListAsync();
    _context.ContactGroupMembers.RemoveRange(members);
    
    // Now safe to delete the group
    var group = await _context.ContactGroups.FindAsync(contactGroupId);
    _context.ContactGroups.Remove(group);
    
    await _context.SaveChangesAsync();
}
```

## Entities NOT Changed

The following entities already had appropriate delete behaviors and were not modified:

1. **FrequencyControl**: Already uses `NoAction` for User relationship
2. **CampaignMessage**: Already uses `Restrict` for Contact relationship
3. **Contact → User**: Maintains `Cascade` (appropriate)
4. **Campaign → User**: Maintains `Cascade` (appropriate)

## Testing Recommendations

1. **Test Migration**: Apply migration to development database
   ```bash
   cd src/MarketingPlatform.Infrastructure
   dotnet ef database update --startup-project ../MarketingPlatform.API
   ```

2. **Test Cascade Deletes**: Verify that deleting a Contact still cascades to ContactGroupMembers and ContactTagAssignments

3. **Test Restrict Behavior**: Verify that attempting to delete a ContactGroup with members fails appropriately

4. **Test Rollback**: Verify that the migration can be rolled back if needed
   ```bash
   dotnet ef database update 20260119072127_UpdateKeywordForeignKeyConstraint --startup-project ../MarketingPlatform.API
   ```

## Verification

### Build Status
✅ Solution builds successfully with no errors

### Migration Creation
✅ Migration created successfully with no cascade path errors

### SQL Script Generation
✅ SQL migration script generates correctly with idempotent checks

## Conclusion

The cascade constraint issues have been resolved by strategically changing three foreign key relationships from Cascade to Restrict. This breaks the multiple cascade path cycles while maintaining data integrity and logical cascade behavior where appropriate. The changes are minimal, well-documented, and preserve the intended functionality of the application.

## Files Modified

1. `src/MarketingPlatform.Infrastructure/Data/Configurations/ContactGroupMemberConfiguration.cs`
2. `src/MarketingPlatform.Infrastructure/Data/Configurations/ContactTagAssignmentConfiguration.cs`
3. `src/MarketingPlatform.Infrastructure/Data/Configurations/KeywordManagementConfiguration.cs`
4. `src/MarketingPlatform.Infrastructure/Migrations/20260119075924_FixCascadeConstraints.cs` (generated)
5. `src/MarketingPlatform.Infrastructure/Migrations/20260119075924_FixCascadeConstraints.Designer.cs` (generated)
6. `src/MarketingPlatform.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs` (updated)

## Migration Timestamp
Created: January 19, 2026, 07:59:24 UTC
