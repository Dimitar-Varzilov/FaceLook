import {
    HubConnectionBuilder,
    LogLevel,
    type HubConnection,
} from "@microsoft/signalr";


function getUsernameValue(): string | null {
    return (document.getElementById("userNameSpan") as HTMLSpanElement | null)
        ?.textContent?.trim() ?? null;
}

function escapeHtml(text: string): string {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}

function displayMessage(user: string, message: string): void {
    const messagesList = document.getElementById("messagesList");
    if (!messagesList) {
        console.warn("messagesList element not found");
        return;
    }

    const li = document.createElement("li");
    li.className = "message-item";

    const timestamp = new Date().toLocaleTimeString("en-US", {
        hour: "2-digit",
        minute: "2-digit",
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
        <div class="text-white ps-4">${escapeHtml(message)}</div>
    `;

    messagesList.insertBefore(li, messagesList.firstChild);

    while (messagesList.children.length > 10) {
        messagesList.removeChild(messagesList.lastChild!);
    }
}

// ── connection ─────────────────────────────────────────────────────────────

const connection: HubConnection = new HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect()
    .build();

// ── group helpers ──────────────────────────────────────────────────────────

async function addUserToGroupAsync(): Promise<void> {
    const userName = getUsernameValue();
    if (userName === null) return;
    try {
        await connection.invoke("JoinGroupAsync", userName);
        console.log(`User ${userName} joined their group`);
    } catch (err) {
        console.error("Error joining group:", err);
    }
}

async function removeUserFromGroupAsync(): Promise<void> {
    const userName = getUsernameValue();
    if (userName === null) return;
    try {
        await connection.invoke("LeaveGroupAsync", userName);
        console.log(`User ${userName} left their group`);
    } catch (err) {
        console.error("Error leaving group:", err);
    }
}

// ── lifecycle ──────────────────────────────────────────────────────────────

async function startAsync(): Promise<void> {
    try {
        await connection.start();
        await addUserToGroupAsync();
        console.log("SignalR Connected.");
    } catch (err) {
        console.error("SignalR connection error:", err);
        setTimeout(startAsync, 5000);
    }
}

connection.on("ReceiveMessage", (user: string, message: string) => {
    console.log(`Received message from ${user}: ${message}`);
    displayMessage(user, message);
});

connection.onreconnecting((error?: Error) => {
    console.warn("Connection lost. Reconnecting...", error);
});

connection.onreconnected((connectionId?: string) => {
    console.log("Reconnected with ID:", connectionId);
    void addUserToGroupAsync();
});

connection.onclose(async () => {
    await removeUserFromGroupAsync();
    console.log("SignalR connection closed");
});

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => void startAsync());
} else {
    void startAsync();
}
