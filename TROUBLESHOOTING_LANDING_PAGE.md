# 🔧 Landing Page Troubleshooting Guide

## Problem: Features/FAQs Not Showing on Landing Page

If the landing page shows "Loading..." spinners but no features or FAQs appear, follow these steps:

---

## ✅ Step 1: Verify Database Tables Exist

### Run the Status Check Script:

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d MarketingPlatformDb -i "check_landing_page_status.sql"
```

**Expected Output:**
```
✓ LandingFeatures table EXISTS
  - Total active features: 6
✓ LandingFaqs table EXISTS
  - Total active FAQs: 8
```

**If tables don't exist or have no data:**

1. Run the SQL scripts:
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d MarketingPlatformDb -i "add_landing_features.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -d MarketingPlatformDb -i "add_landing_faqs.sql"
```

2. Verify data was inserted:
```sql
SELECT COUNT(*) FROM LandingFeatures WHERE IsActive = 1;
SELECT COUNT(*) FROM LandingFaqs WHERE IsActive = 1;
```

---

## ✅ Step 2: Verify API is Running

### Check if API is accessible:

1. Make sure **both** projects are running:
   - `MarketingPlatform.API` (usually https://localhost:7001)
   - `MarketingPlatform.Web` (usually https://localhost:7173)

2. Test API directly in browser:
   ```
   https://localhost:7001/api/landingfeatures
   https://localhost:7001/api/landingfaqs
   ```

   **Expected Response:**
   ```json
   {
     "success": true,
     "data": [ /* array of features/faqs */ ],
     "message": null
   }
   ```

3. Or use the test page:
   - Open `test_features_api.html` in browser
   - Update API Base URL to your API port
   - Click "Test Both APIs"

### If API returns 404:

Check that the controllers are properly registered:

**File:** `src/MarketingPlatform.API/Program.cs`
```csharp
// Should have:
builder.Services.AddControllers();
app.MapControllers();
```

---

## ✅ Step 3: Check Browser Console for Errors

1. Open landing page in browser
2. Press **F12** to open Developer Tools
3. Go to **Console** tab
4. Look for errors

### Common Errors and Fixes:

#### Error: "CORS policy"
```
Access to fetch at 'https://localhost:7001/api/landingfeatures'
from origin 'https://localhost:7173' has been blocked by CORS policy
```

**Fix:** Update `src/MarketingPlatform.API/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
    {
        policy.WithOrigins("https://localhost:7173", "http://localhost:7173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Before app.MapControllers(), add:
app.UseCors("AllowWeb");
```

#### Error: "Failed to fetch" or "net::ERR_CONNECTION_REFUSED"
```
Failed to load features: TypeError: Failed to fetch
```

**Fix:**
- API is not running
- Start the API project: `dotnet run --project src/MarketingPlatform.API`
- Check the port matches in `app-urls.js`

#### Error: "AppUrls is not defined"
```
Uncaught ReferenceError: AppUrls is not defined
```

**Fix:** Check that `app-urls.js` is loaded before `landing-features.js` in `_Layout.cshtml`:

```html
<script src="~/js/app-urls.js"></script>
<script src="~/js/landing-features.js"></script>
<script src="~/js/landing-faqs.js"></script>
```

---

## ✅ Step 4: Verify DbContext Has Entities

**File:** `src/MarketingPlatform.Infrastructure/Data/ApplicationDbContext.cs`

Should contain:
```csharp
public DbSet<LandingFeature> LandingFeatures { get; set; }
public DbSet<LandingFaq> LandingFaqs { get; set; }
```

If missing, add them and rebuild:
```bash
dotnet build
```

---

## ✅ Step 5: Check Network Requests

1. Open browser DevTools (F12)
2. Go to **Network** tab
3. Refresh the page
4. Look for requests to `/api/landingfeatures` and `/api/landingfaqs`

### Analyze the response:

- **Status 200**: Good! Check if data is in response
- **Status 404**: API endpoint not found (check Program.cs has MapControllers)
- **Status 500**: Server error (check API console for stack trace)
- **Status 0**: CORS error or API not running

---

## ✅ Step 6: Verify JavaScript Files are Loaded

Check browser DevTools > Network tab for these files:
- ✅ `app-urls.js` - Status 200
- ✅ `landing-features.js` - Status 200
- ✅ `landing-faqs.js` - Status 200
- ✅ `landing-enhancements.css` - Status 200

If any show **404**:
1. Verify files exist in `src/MarketingPlatform.Web/wwwroot/js/`
2. Rebuild the Web project: `dotnet build src/MarketingPlatform.Web`
3. Clear browser cache (Ctrl+Shift+Delete)

---

## ✅ Step 7: Test API with curl (Advanced)

```bash
# Test Features API
curl -k https://localhost:7001/api/landingfeatures

# Test FAQs API
curl -k https://localhost:7001/api/landingfaqs
```

Expected output:
```json
{"success":true,"data":[...],"message":null}
```

---

## 🔍 Quick Diagnostic Commands

### Check if tables exist:
```sql
USE MarketingPlatformDb;
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('LandingFeatures', 'LandingFaqs');
```

### Check if data exists:
```sql
SELECT 'Features' AS Type, COUNT(*) AS Count FROM LandingFeatures WHERE IsActive = 1
UNION ALL
SELECT 'FAQs', COUNT(*) FROM LandingFaqs WHERE IsActive = 1;
```

### Check if API is listening:
```bash
netstat -an | findstr "7001"
```

---

## 📋 Complete Checklist

- [ ] SQL scripts have been run
- [ ] Database tables exist and have data
- [ ] Both API and Web projects are running
- [ ] API returns JSON when accessed directly in browser
- [ ] No CORS errors in browser console
- [ ] JavaScript files are loading (Network tab shows 200)
- [ ] `AppUrls` is defined (check Console)
- [ ] No JavaScript errors in Console
- [ ] Features and FAQs render on the page

---

## 🆘 Still Not Working?

### Enable Detailed Logging:

1. Open `src/MarketingPlatform.Web/wwwroot/js/landing-features.js`
2. Check console logs (they're already there!)
3. Look for messages like:
   - "Fetching landing features from: ..."
   - "Response status: ..."
   - "API result: ..."
   - "Rendering X features"

### Common Issues Summary:

| Symptom | Likely Cause | Solution |
|---------|-------------|----------|
| Spinner never stops | API not responding | Start API project |
| "No features available" | Database empty | Run SQL scripts |
| CORS error | Cross-origin blocked | Configure CORS in API |
| 404 error | Wrong API URL | Check app-urls.js |
| 500 error | Server exception | Check API console output |
| Nothing happens | JavaScript error | Check browser console |

---

## 📞 Debug Output Locations

1. **Browser Console** (F12): JavaScript errors and logs
2. **Browser Network Tab** (F12): HTTP requests/responses
3. **API Console**: Server-side errors and logs
4. **SQL Output**: Database query results

---

## ✨ Expected Behavior When Working:

1. Page loads with spinners
2. API calls are made (visible in Network tab)
3. Features render with flip cards (6 cards)
4. FAQs render in accordion (8 FAQs)
5. All animations work smoothly
6. No errors in console

---

**Last Updated**: January 2026
