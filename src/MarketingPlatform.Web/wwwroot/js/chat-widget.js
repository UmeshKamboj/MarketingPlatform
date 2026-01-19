/**
 * Chat Widget JavaScript - Handles SignalR real-time communication
 * Position: Bottom LEFT corner of the page
 */

(function() {
    'use strict';

    // Chat Widget State
    const chatState = {
        connection: null,
        chatRoomId: null,
        guestName: null,
        guestEmail: null,
        isConnected: false,
        typingTimeout: null
    };

    // DOM Elements
    const elements = {
        chatWidget: document.getElementById('chat-widget'),
        chatButton: document.getElementById('chat-button'),
        chatWindow: document.getElementById('chat-window'),
        chatCloseBtn: document.getElementById('chat-close-btn'),
        preChatForm: document.getElementById('pre-chat-form'),
        startChatBtn: document.getElementById('start-chat-btn'),
        guestNameInput: document.getElementById('guest-name'),
        guestEmailInput: document.getElementById('guest-email'),
        chatMessagesArea: document.getElementById('chat-messages-area'),
        chatMessages: document.getElementById('chat-messages'),
        chatInput: document.getElementById('chat-input'),
        sendMessageBtn: document.getElementById('send-message-btn'),
        typingIndicator: document.getElementById('typing-indicator'),
        unreadBadge: document.getElementById('chat-unread-badge')
    };

    // Initialize Chat Widget
    function initChatWidget() {
        console.log('Initializing chat widget...');
        
        // Event Listeners
        elements.chatButton.addEventListener('click', toggleChatWindow);
        elements.chatCloseBtn.addEventListener('click', closeChatWindow);
        elements.startChatBtn.addEventListener('click', handleStartChat);
        elements.sendMessageBtn.addEventListener('click', sendMessage);
        elements.chatInput.addEventListener('keydown', handleChatInputKeydown);
        elements.chatInput.addEventListener('input', handleTyping);

        // Check if there's a saved chat session
        checkSavedSession();
    }

    // Toggle chat window
    function toggleChatWindow() {
        elements.chatWindow.classList.toggle('show');
        if (elements.chatWindow.classList.contains('show')) {
            // Reset unread count
            elements.unreadBadge.style.display = 'none';
            elements.unreadBadge.textContent = '0';
        }
    }

    // Close chat window
    function closeChatWindow() {
        elements.chatWindow.classList.remove('show');
    }

    // Check for saved session in localStorage
    function checkSavedSession() {
        const savedChatRoomId = localStorage.getItem('chatRoomId');
        const savedGuestName = localStorage.getItem('guestName');
        const savedGuestEmail = localStorage.getItem('guestEmail');

        if (savedChatRoomId && savedGuestName && savedGuestEmail) {
            chatState.chatRoomId = parseInt(savedChatRoomId);
            chatState.guestName = savedGuestName;
            chatState.guestEmail = savedGuestEmail;

            // Show chat messages area
            elements.preChatForm.style.display = 'none';
            elements.chatMessagesArea.classList.add('show');

            // Connect to SignalR and load chat history
            connectToHub();
            loadChatHistory();
        }
    }

    // Handle Start Chat
    async function handleStartChat() {
        const name = elements.guestNameInput.value.trim();
        const email = elements.guestEmailInput.value.trim();

        // Validation
        if (!name || !email) {
            alert('Please enter your name and email');
            return;
        }

        if (!isValidEmail(email)) {
            alert('Please enter a valid email address');
            return;
        }

        // Disable button
        elements.startChatBtn.disabled = true;
        elements.startChatBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Starting...';

        try {
            // Create chat room via API
            const response = await fetch(`${window.chatConfig.apiBaseUrl}/api/chat/rooms`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    guestName: name,
                    guestEmail: email
                })
            });

            const result = await response.json();

            if (result.success && result.data) {
                chatState.chatRoomId = result.data.id;
                chatState.guestName = name;
                chatState.guestEmail = email;

                // Save to localStorage
                localStorage.setItem('chatRoomId', chatState.chatRoomId);
                localStorage.setItem('guestName', name);
                localStorage.setItem('guestEmail', email);

                // Hide pre-chat form and show chat area
                elements.preChatForm.style.display = 'none';
                elements.chatMessagesArea.classList.add('show');

                // Connect to SignalR hub
                await connectToHub();

                // Add welcome message
                addSystemMessage(`Welcome, ${name}! A support agent will be with you shortly.`);
            } else {
                throw new Error(result.message || 'Failed to start chat');
            }
        } catch (error) {
            console.error('Error starting chat:', error);
            alert('Failed to start chat. Please try again.');
        } finally {
            elements.startChatBtn.disabled = false;
            elements.startChatBtn.innerHTML = '<i class="bi bi-chat-dots"></i> Start Chat';
        }
    }

    // Connect to SignalR Hub
    async function connectToHub() {
        if (chatState.isConnected) {
            console.log('Already connected to hub');
            return;
        }

        try {
            chatState.connection = new signalR.HubConnectionBuilder()
                .withUrl(window.chatConfig.hubUrl)
                .withAutomaticReconnect()
                .build();

            // Set up event handlers
            setupSignalRHandlers();

            // Start connection
            await chatState.connection.start();
            console.log('Connected to chat hub');
            chatState.isConnected = true;

            // Join the chat room
            await chatState.connection.invoke('JoinChatRoom', chatState.chatRoomId);
        } catch (error) {
            console.error('Error connecting to hub:', error);
            addSystemMessage('Connection error. Trying to reconnect...');
        }
    }

    // Setup SignalR event handlers
    function setupSignalRHandlers() {
        // Receive message
        chatState.connection.on('ReceiveMessage', (message) => {
            console.log('Message received:', message);
            addChatMessage(message);
            
            // Play notification sound if window is not visible
            if (!elements.chatWindow.classList.contains('show')) {
                incrementUnreadCount();
            }
        });

        // User typing
        chatState.connection.on('UserTyping', (userId, isTyping) => {
            if (isTyping) {
                elements.typingIndicator.classList.add('show');
            } else {
                elements.typingIndicator.classList.remove('show');
            }
        });

        // Chat room closed
        chatState.connection.on('ChatRoomClosed', (chatRoomId) => {
            if (chatRoomId === chatState.chatRoomId) {
                addSystemMessage('Chat session has been closed. You will receive a transcript via email.');
                elements.chatInput.disabled = true;
                elements.sendMessageBtn.disabled = true;
                
                // Clear localStorage
                clearChatSession();
            }
        });

        // Connection closed
        chatState.connection.onclose(() => {
            console.log('Connection closed');
            chatState.isConnected = false;
            addSystemMessage('Connection lost. Reconnecting...');
        });

        // Reconnected
        chatState.connection.onreconnected(() => {
            console.log('Reconnected to hub');
            chatState.isConnected = true;
            addSystemMessage('Reconnected to chat');
            
            // Rejoin the chat room
            if (chatState.chatRoomId) {
                chatState.connection.invoke('JoinChatRoom', chatState.chatRoomId);
            }
        });
    }

    // Send message
    async function sendMessage() {
        const messageText = elements.chatInput.value.trim();
        
        if (!messageText || !chatState.isConnected) {
            return;
        }

        try {
            // Send via SignalR
            await chatState.connection.invoke('SendMessage', {
                chatRoomId: chatState.chatRoomId,
                messageText: messageText,
                messageType: 0 // Text message
            });

            // Clear input
            elements.chatInput.value = '';
            elements.chatInput.style.height = 'auto';
        } catch (error) {
            console.error('Error sending message:', error);
            addSystemMessage('Failed to send message. Please try again.');
        }
    }

    // Handle chat input keydown
    function handleChatInputKeydown(e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    }

    // Handle typing indicator
    function handleTyping() {
        // Auto-resize textarea
        elements.chatInput.style.height = 'auto';
        elements.chatInput.style.height = elements.chatInput.scrollHeight + 'px';

        if (!chatState.isConnected) return;

        // Clear previous typing timeout
        if (chatState.typingTimeout) {
            clearTimeout(chatState.typingTimeout);
        }

        // Notify typing
        chatState.connection.invoke('NotifyTyping', chatState.chatRoomId, true);

        // Stop typing after 3 seconds
        chatState.typingTimeout = setTimeout(() => {
            if (chatState.isConnected) {
                chatState.connection.invoke('NotifyTyping', chatState.chatRoomId, false);
            }
        }, 3000);
    }

    // Add chat message to UI
    function addChatMessage(message) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `chat-message ${message.isOwnMessage ? 'own' : 'other'}`;

        const bubble = document.createElement('div');
        bubble.className = 'message-bubble';

        if (!message.isOwnMessage) {
            const sender = document.createElement('div');
            sender.className = 'message-sender';
            sender.textContent = message.senderName || 'Support';
            bubble.appendChild(sender);
        }

        const text = document.createElement('div');
        text.className = 'message-text';
        text.textContent = message.messageText;
        bubble.appendChild(text);

        const time = document.createElement('div');
        time.className = 'message-time';
        time.textContent = formatTime(message.sentAt);
        bubble.appendChild(time);

        messageDiv.appendChild(bubble);
        elements.chatMessages.appendChild(messageDiv);

        // Scroll to bottom
        scrollToBottom();
    }

    // Add system message
    function addSystemMessage(message) {
        const messageDiv = document.createElement('div');
        messageDiv.className = 'system-message';
        messageDiv.textContent = message;
        elements.chatMessages.appendChild(messageDiv);
        scrollToBottom();
    }

    // Load chat history
    async function loadChatHistory() {
        try {
            const response = await fetch(`${window.chatConfig.apiBaseUrl}/api/chat/rooms/${chatState.chatRoomId}/messages`);
            const result = await response.json();

            if (result.success && result.data) {
                // Clear existing messages
                elements.chatMessages.innerHTML = '<div class="system-message">Chat session started</div>';
                
                // Add all messages
                result.data.forEach(message => {
                    addChatMessage(message);
                });
            }
        } catch (error) {
            console.error('Error loading chat history:', error);
        }
    }

    // Increment unread count
    function incrementUnreadCount() {
        let count = parseInt(elements.unreadBadge.textContent) || 0;
        count++;
        elements.unreadBadge.textContent = count;
        elements.unreadBadge.style.display = 'flex';
    }

    // Scroll chat to bottom
    function scrollToBottom() {
        elements.chatMessages.scrollTop = elements.chatMessages.scrollHeight;
    }

    // Format time
    function formatTime(dateString) {
        const date = new Date(dateString);
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        return `${hours}:${minutes}`;
    }

    // Validate email
    function isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    // Clear chat session
    function clearChatSession() {
        localStorage.removeItem('chatRoomId');
        localStorage.removeItem('guestName');
        localStorage.removeItem('guestEmail');
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initChatWidget);
    } else {
        initChatWidget();
    }
})();
