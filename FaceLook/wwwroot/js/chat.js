"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

const addUserToGroupAsync = async () => {
    const userName = getUsernameValue();
    if (userName === null) return;
    
    try {
        await connection.invoke("JoinGroupAsync", userName);
        console.log(`User ${userName} joined their group`);
    } catch (err) {
        console.error("Error joining group:", err);
    }
};

const removeUserFromGroupAsync = async () => {
    const userName = getUsernameValue();
    if (userName === null) return;
    
    try {
        await connection.invoke("LeaveGroupAsync", userName);
        console.log(`User ${userName} left their group`);
    } catch (err) {
        console.error("Error leaving group:", err);
    }
};

async function startAsync() {
    try {
        await connection.start();
        await addUserToGroupAsync();
        console.log("SignalR Connected.");
    } catch (err) {
        console.error("SignalR connection error:", err);
        // Retry connection after 5 seconds
        setTimeout(startAsync, 5000);
    }
}

connection.on("ReceiveMessage", function (user, message) {
    console.log(`Received message from ${user}: ${message}`);
    displayMessage(user, message);
});

// Display message in the messages list with glass morphism styling
function displayMessage(user, message) {
    const messagesList = document.getElementById("messagesList");
    if (!messagesList) {
        console.warn("messagesList element not found");
        return;
    }

    const li = document.createElement("li");
    li.className = "message-item";
    
    const timestamp = new Date().toLocaleTimeString('en-US', { 
        hour: '2-digit', 
        minute: '2-digit' 
    });
    
    li.innerHTML = `
        <div class="d-flex justify-content-between align-items-start mb-2">
            <div class="d-flex align-items-center">
                <i class="bi bi-envelope-fill me-2" style="color: #764ba2;"></i>
                <strong class="text-white">${escapeHtml(user)}</strong>
            </div>
            <small class="text-white opacity-75">
                <i class="bi bi-clock"></i> ${timestamp}
            </small>
        </div>
        <div class="text-white ps-4">
            ${escapeHtml(message)}
        </div>
    `;
    
    // Add message to the top of the list
    messagesList.insertBefore(li, messagesList.firstChild);
    
    // Limit to 10 most recent messages
    while (messagesList.children.length > 10) {
        messagesList.removeChild(messagesList.lastChild);
    }
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Handle reconnection
connection.onreconnecting((error) => {
    console.warn("Connection lost. Reconnecting...", error);
});

connection.onreconnected((connectionId) => {
    console.log("Reconnected with ID:", connectionId);
    addUserToGroupAsync();
});

connection.onclose(async () => {
    await removeUserFromGroupAsync();
    console.log("SignalR connection closed");
});

// Start the connection when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', startAsync);
} else {
    startAsync();
}
