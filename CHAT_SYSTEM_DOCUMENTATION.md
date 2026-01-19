# Real-Time Chat System Documentation

## Overview
The Marketing Platform now includes a real-time chat/messaging system that allows end users (guests or customers) to communicate with support staff (admin and employees) in real-time. The chat system is built using SignalR for bidirectional real-time communication.

## Features Implemented

### 1. Real-Time Communication
- ✅ **SignalR Integration**: Bidirectional real-time communication between customers and support staff
- ✅ **Instant Message Delivery**: Messages are delivered instantly without page refresh
- ✅ **Typing Indicators**: Shows when the other party is typing
- ✅ **Online/Offline Status**: Tracks and displays user online status
- ✅ **Automatic Reconnection**: Handles connection drops gracefully

### 2. Guest Chat Support
- ✅ **Pre-Chat Form**: Collects guest name and email before starting chat
- ✅ **Session Persistence**: Chat sessions persist across page refreshes using localStorage
- ✅ **Email Transcripts**: Automatically sends chat transcript to guest email when chat is closed
- ✅ **No Authentication Required**: Guests can chat without creating an account

### 3. Chat Widget (Bottom Left Position)
- ✅ **Fixed Position**: Widget positioned on bottom-left corner of landing page
- ✅ **Floating Button**: Minimized chat button with unread message badge
- ✅ **Responsive Design**: Works on desktop, tablet, and mobile devices
- ✅ **Smooth Animations**: Slide-up animations and fade effects
- ✅ **Modern UI**: Purple gradient theme with clean, professional appearance

### 4. Message Features
- ✅ **Text Messages**: Send and receive text messages up to 2000 characters
- ✅ **Message Timestamps**: Display message sent time
- ✅ **Read Status**: Track which messages have been read
- ✅ **Message Bubbles**: Differentiate own messages (right, purple) from others (left, white)
- ✅ **System Messages**: Display connection and session status messages

### 5. Backend Architecture
- ✅ **SignalR Hub**: ChatHub handles real-time communication
- ✅ **RESTful API**: Complete CRUD operations for chat rooms and messages
- ✅ **Repository Pattern**: Clean separation of data access logic
- ✅ **Service Layer**: Business logic encapsulated in ChatService
- ✅ **Database**: EF Core migrations for chat tables (ChatRoom, ChatMessage)

## Architecture

### Database Schema

#### ChatRoom Entity
```csharp
- Id (int) - Primary key
- GuestName (string, nullable) - Name for guest users
- GuestEmail (string, nullable) - Email for guest users
- CustomerId (string, nullable) - FK to ApplicationUser (for authenticated users)
- AssignedEmployeeId (string, nullable) - FK to ApplicationUser (support staff)
- Status (ChatRoomStatus) - Open, InProgress, Closed
- CreatedAt (DateTime) - Chat room creation time
- UpdatedAt (DateTime) - Last activity timestamp
```

#### ChatMessage Entity
```csharp
- Id (int) - Primary key
- ChatRoomId (int) - FK to ChatRoom
- SenderId (string, nullable) - FK to ApplicationUser (null for guests)
- MessageText (string) - Message content (max 2000 chars)
- IsRead (bool) - Read status
- SentAt (DateTime) - Message timestamp
- MessageType (MessageType) - Text, Image, File, System
- AttachmentUrl (string, nullable) - File attachment URL
- AttachmentFileName (string, nullable) - Original filename
```

#### ApplicationUser Extensions
```csharp
- IsOnline (bool) - Current online status
- LastSeenAt (DateTime, nullable) - Last activity timestamp
- ChatMessages (ICollection<ChatMessage>) - Sent messages
- AssignedChatRooms (ICollection<ChatRoom>) - Assigned as employee
- CustomerChatRooms (ICollection<ChatRoom>) - Created as customer
```

### SignalR Hub Methods

#### ChatHub
**Client-to-Server Methods:**
- `JoinChatRoom(int chatRoomId)` - Join a specific chat room
- `LeaveChatRoom(int chatRoomId)` - Leave a chat room
- `SendMessage(SendMessageDto)` - Send a message
- `NotifyTyping(int chatRoomId, bool isTyping)` - Toggle typing indicator
- `MarkMessagesAsRead(int chatRoomId)` - Mark messages as read
- `AssignChatRoom(int chatRoomId, string employeeId)` - Assign employee (admin only)
- `CloseChatRoom(int chatRoomId)` - Close chat session

**Server-to-Client Events:**
- `ReceiveMessage(ChatMessageDto)` - New message received
- `UserTyping(string userId, bool isTyping)` - User typing status
- `UserStatusChanged(string userId, bool isOnline)` - User online status changed
- `ChatRoomClosed(int chatRoomId)` - Chat room closed
- `ChatRoomAssigned(int chatRoomId, string employeeId)` - Chat assigned
- `UserJoinedRoom(string userId)` - User joined chat
- `UserLeftRoom(string userId)` - User left chat

### API Endpoints

#### Chat API Controller (`/api/chat`)

**Chat Rooms:**
- `GET /api/chat/rooms` - Get all active chat rooms (auth required)
- `GET /api/chat/rooms/unassigned` - Get unassigned chats (auth required)
- `GET /api/chat/rooms/employee/{employeeId}` - Get employee's chats (auth required)
- `GET /api/chat/rooms/{id}` - Get specific chat room
- `POST /api/chat/rooms` - Create new chat room
- `PUT /api/chat/rooms/{id}/assign` - Assign employee to chat (auth required)
- `PUT /api/chat/rooms/{id}/close` - Close chat session
- `POST /api/chat/rooms/{id}/send-transcript` - Email transcript (auth required)

**Messages:**
- `GET /api/chat/rooms/{id}/messages` - Get chat history
- `POST /api/chat/messages` - Send message (fallback for non-SignalR)
- `POST /api/chat/rooms/{id}/mark-read` - Mark messages as read (auth required)
- `GET /api/chat/unread-count/{employeeId}` - Get unread count (auth required)

## Configuration

### appsettings.json
```json
{
  "ChatSettings": {
    "MaxMessageLength": 2000,
    "AllowFileUpload": true,
    "MaxFileSize": 5242880,
    "AllowedFileTypes": [".jpg", ".png", ".pdf", ".doc", ".docx"]
  }
}
```

### CORS Configuration
SignalR requires CORS with credentials enabled:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});
```

### SignalR Setup
```csharp
// Program.cs
builder.Services.AddSignalR();

// Map hub endpoint
app.MapHub<ChatHub>("/hubs/chat");
```

## User Guide

### For Customers/Guests

1. **Starting a Chat**
   - Navigate to the landing page (Home)
   - Click the purple chat button in the bottom-left corner
   - Enter your name and email in the pre-chat form
   - Click "Start Chat"

2. **Sending Messages**
   - Type your message in the input field
   - Press Enter or click the send button
   - Messages appear instantly in the chat window

3. **During Chat**
   - See when support staff is typing
   - Receive instant responses from support
   - Chat history is preserved across page refreshes

4. **After Chat**
   - When support closes the chat, you receive an email transcript
   - Email includes complete conversation history
   - Can reference chat ID for future support

### For Support Staff (Admin/Employees)

**Coming in Phase 5:**
- Dashboard to view all active chats
- Ability to pick up unassigned chats
- Assign chats to specific employees
- View chat history and customer information
- Use canned responses for common questions
- Search and filter chat sessions

## Technical Details

### Frontend Technologies
- **SignalR Client**: Microsoft SignalR JavaScript library (v8.0.0)
- **HTML5 & CSS3**: Modern, responsive design
- **Bootstrap Icons**: Icon library for UI elements
- **Vanilla JavaScript**: No framework dependencies for the widget

### Backend Technologies
- **ASP.NET Core 8.0**: Web framework
- **SignalR Core**: Real-time communication
- **Entity Framework Core 8.0**: ORM for database operations
- **SQL Server**: Database
- **AutoMapper**: DTO mapping (future implementation)

### Security Features
- **Input Sanitization**: Message text is sanitized to prevent XSS
- **Max Length Validation**: Messages limited to 2000 characters
- **Rate Limiting**: Prevents spam (to be implemented)
- **HTTPS Only**: All communication over secure connection
- **CORS Protection**: Restricted origins

## Migration Guide

### Database Migration
```bash
cd src/MarketingPlatform.Infrastructure
dotnet ef migrations add AddChatEntities --startup-project ../MarketingPlatform.API
dotnet ef database update --startup-project ../MarketingPlatform.API
```

### Running the Application
1. Start the API:
   ```bash
   cd src/MarketingPlatform.API
   dotnet run
   ```

2. Start the Web app:
   ```bash
   cd src/MarketingPlatform.Web
   dotnet run
   ```

3. Navigate to https://localhost:7061 (or configured port)
4. Chat widget appears on the landing page (bottom-left)

## Customization

### Chat Widget Position
To change the position from bottom-left to bottom-right:

**CSS** (`chat-widget.css`):
```css
#chat-widget {
    position: fixed;
    bottom: 20px;
    left: 20px;  /* Change to right: 20px; */
    z-index: 9999;
}

#chat-window {
    bottom: 100px;
    left: 20px;  /* Change to right: 20px; */
}
```

### Color Theme
To change the purple gradient theme:

**CSS** (`chat-widget.css`):
```css
/* Current gradient: purple */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Example alternatives: */
/* Blue gradient */
background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);

/* Green gradient */
background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
```

### Maximum Message Length
Update in `appsettings.json`:
```json
"ChatSettings": {
    "MaxMessageLength": 5000  // Default is 2000
}
```

## Troubleshooting

### Chat Widget Not Appearing
1. Verify you're on the Home/Index page
2. Check browser console for JavaScript errors
3. Ensure SignalR CDN is accessible
4. Check CSP (Content Security Policy) settings

### Messages Not Sending
1. Check SignalR connection status in browser console
2. Verify API is running and accessible
3. Check CORS configuration
4. Verify database connection

### Email Transcripts Not Sending
1. Ensure email provider is configured
2. Check guest email is valid
3. Verify ChatService has IEmailProvider dependency
4. Check application logs for errors

## Future Enhancements

### Planned Features
- [ ] File/image upload capability
- [ ] Admin/Employee dashboard
- [ ] Canned responses library
- [ ] Chat analytics and reporting
- [ ] Multi-language support
- [ ] Chat bot integration
- [ ] Video/voice call support
- [ ] Screen sharing
- [ ] Customer satisfaction surveys
- [ ] Chat history export (PDF, CSV)

## Support

For issues, questions, or feature requests:
- Check application logs in `Logs/` directory
- Review SignalR documentation: https://docs.microsoft.com/aspnet/core/signalr
- Contact development team

## License
This chat system is part of the Marketing Platform and follows the same license.

---

**Version**: 1.0.0  
**Last Updated**: January 2026  
**Author**: Marketing Platform Development Team
