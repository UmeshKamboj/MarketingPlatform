# Privacy Policy & Terms of Service Implementation

## Overview
This document describes the implementation of Privacy Policy and Terms of Service pages with full admin management capabilities, including image upload support.

## Features Implemented

### 1. Database Schema
- **PageContent Entity**: Stores page content with support for:
  - Unique page keys (e.g., "privacy-policy", "terms-of-service")
  - Title and meta description for SEO
  - Rich HTML content
  - Multiple images stored as JSON array
  - Publishing status (draft/published)
  - Audit fields (created, updated, last modified by)
  - Soft delete support

### 2. Repository & Service Layer
- **IPageContentRepository / PageContentRepository**: Data access layer
- **IPageContentService / PageContentService**: Business logic layer
  - Content management (create, update, retrieve)
  - Image upload with validation (size, format)
  - Image deletion
  - Integration with file storage service

### 3. Public-Facing Pages
Created two professional, responsive pages:

#### Privacy Policy Page (`/Home/Privacy`)
- Clean, modern design with gradient header
- Displays dynamic content from database
- Shows images if uploaded
- "Back to Home" navigation button
- Responsive layout
- Default content includes:
  - Information collection
  - Data usage
  - Security measures
  - User rights
  - Contact information

#### Terms of Service Page (`/Home/Terms`)
- Similar design to Privacy Policy
- Displays dynamic content from database
- Shows images if uploaded
- "Back to Home" navigation button
- Default content includes:
  - Acceptance of terms
  - Use license
  - Account terms
  - Prohibited uses
  - Limitation of liability
  - Contact information

### 4. Admin Management Interface

#### Page Content Index (`/PageContent/Index`)
- Overview of all managed pages
- Quick access to edit Privacy Policy and Terms of Service
- Status indicators (Published/Draft)
- Preview links

#### Page Content Editor (`/PageContent/Edit/{pageKey}`)
Features:
- Rich text content editor (HTML support)
- Title and meta description editing
- Publish/draft toggle
- Image gallery with upload functionality
- Image management (upload, delete, copy URL)
- Real-time image preview
- Formatting help guide
- Preview link to public page

Image Upload Features:
- Drag-and-drop or click to upload
- File type validation (JPG, PNG, GIF, WebP)
- File size validation (max 5MB)
- Progress indicator
- Thumbnail gallery
- Copy URL to clipboard
- Delete images with confirmation

### 5. Navigation Improvements
Updated all authentication pages with "Back to Home" links:
- Login page
- Register page
- Forgot Password page

Added admin menu item:
- "Privacy & Terms Pages" link in Super Admin menu
- Accessible via navigation bar

Updated footer:
- Links to Privacy Policy and Terms of Service
- Properly styled and responsive

### 6. Data Seeding
Automatic seeding on application startup:
- Creates default Privacy Policy content
- Creates default Terms of Service content
- Only seeds if pages don't already exist
- Production-ready default content

## Technical Details

### File Structure
```
MarketingPlatform.Core/
└── Entities/
    └── PageContent.cs

MarketingPlatform.Core/Interfaces/Repositories/
└── IPageContentRepository.cs

MarketingPlatform.Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs (updated)
├── Repositories/
│   └── PageContentRepository.cs
└── Migrations/
    └── 20260118222800_AddPageContentEntity.cs

MarketingPlatform.Application/
├── DTOs/
│   └── PageContentDto.cs
├── Interfaces/
│   └── IPageContentService.cs
└── Services/
    └── PageContentService.cs

MarketingPlatform.Web/
├── Controllers/
│   ├── HomeController.cs (updated)
│   └── PageContentController.cs
├── Views/
│   ├── Home/
│   │   ├── Privacy.cshtml (updated)
│   │   └── Terms.cshtml (new)
│   ├── PageContent/
│   │   ├── Index.cshtml (new)
│   │   └── Edit.cshtml (new)
│   ├── Auth/
│   │   ├── Login.cshtml (updated)
│   │   ├── Register.cshtml (updated)
│   │   └── ForgotPassword.cshtml (updated)
│   └── Shared/
│       └── _Layout.cshtml (updated)
├── DatabaseSeeder.cs (new)
└── Program.cs (updated)
```

### Database Migration
```sql
CREATE TABLE PageContents (
    Id INT PRIMARY KEY IDENTITY,
    PageKey NVARCHAR(100) UNIQUE NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    MetaDescription NVARCHAR(500) NULL,
    ImageUrls NVARCHAR(MAX) NULL,
    IsPublished BIT NOT NULL DEFAULT 1,
    LastModifiedBy NVARCHAR(450) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_PageContents_PageKey ON PageContents(PageKey);
CREATE INDEX IX_PageContents_IsPublished ON PageContents(IsPublished);
CREATE INDEX IX_PageContents_LastModifiedBy ON PageContents(LastModifiedBy);

ALTER TABLE PageContents
ADD CONSTRAINT FK_PageContents_AspNetUsers_LastModifiedBy
FOREIGN KEY (LastModifiedBy) REFERENCES AspNetUsers(Id)
ON DELETE SET NULL;
```

### API Endpoints
- `GET /Home/Privacy` - View Privacy Policy page
- `GET /Home/Terms` - View Terms of Service page
- `GET /PageContent/Index` - List all page contents (Admin)
- `GET /PageContent/Edit/{pageKey}` - Edit page content (Admin)
- `POST /PageContent/Edit` - Save page content (Admin)
- `POST /PageContent/UploadImage` - Upload image (Admin)
- `POST /PageContent/DeleteImage` - Delete image (Admin)

### Security Features
- Admin pages require authentication
- Image upload validation (file type, size)
- XSS protection through proper HTML encoding
- CSRF protection on all forms
- Foreign key constraint on LastModifiedBy

### Responsive Design
- Mobile-first approach
- Bootstrap 5 styling
- Professional gradient headers
- Card-based layouts
- Responsive navigation

## Usage

### For End Users
1. Navigate to Privacy Policy: Click "Privacy Policy" link in footer
2. Navigate to Terms of Service: Click "Terms of Service" link in footer
3. Return to home: Click "Back to Home" button

### For Administrators
1. Navigate to Super Admin menu → "Privacy & Terms Pages"
2. Click "Edit Privacy Policy" or "Edit Terms of Service"
3. Update content using HTML tags or plain text
4. Upload images:
   - Click "Upload Image" button
   - Select image file (max 5MB)
   - Image appears in gallery
5. Manage images:
   - Click trash icon to delete
   - Click clipboard icon to copy URL
6. Toggle "Published" checkbox to make visible to users
7. Click "Save Changes"
8. Preview changes by clicking "Preview Page"

## Future Enhancements
- WYSIWYG editor integration (e.g., TinyMCE, CKEditor)
- Version history and rollback
- Multi-language support
- More page types (About Us, FAQ, etc.)
- Image cropping and resizing
- Markdown support option
- Content approval workflow

## Testing Checklist
- [ ] Privacy Policy page displays correctly
- [ ] Terms of Service page displays correctly
- [ ] Back to Home buttons work on all pages
- [ ] Footer links navigate correctly
- [ ] Admin can edit content
- [ ] Image upload works
- [ ] Image deletion works
- [ ] Published/Draft toggle works
- [ ] Content saves correctly
- [ ] SEO meta tags appear
- [ ] Responsive on mobile devices
- [ ] Authentication required for admin pages

## Notes
- Default content is automatically seeded on first run
- Images are stored using the existing file storage service
- Supports local storage, Azure Blob, and S3 backends
- Content supports full HTML markup
- Pages use soft delete (IsDeleted flag)
