class Message {
    constructor(username, text, when) {
        this.userName = username;
        this.text = text;
        this.when = when;
    }
}

// Username provided by Razor view (Chat.cshtml)
const username = (typeof window !== "undefined" && window.chatUsername) ? window.chatUsername : "Anonymous";

const textInput = document.getElementById('messageText');
const chat = document.getElementById('chat');
const submitButton = document.getElementById('submitButton');

// Build SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Receive messages from server
connection.on("ReceiveMessage", (sender, text, sentAt) => {
    const message = new Message(sender, text, sentAt);
    addMessageToChat(message);
});

// Connection closed
connection.onclose((err) => {
    console.error("SignalR connection closed.", err);
});

// Start connection
connection.start()
    .then(() => console.info("Connected to chat hub"))
    .catch(err => console.error("SignalR start error:", err));

// Send message function
function sendMessageToHub(message) {
    if (!connection) return;
    connection.invoke("SendMessage", message.text)
        .catch(err => console.error("SendMessage error:", err));
}

// UI Event Listeners
if (submitButton && textInput) {
    submitButton.addEventListener('click', () => {
        let text = textInput.value.trim();
        if (!text) return;

        const message = new Message(username, text, new Date().toISOString());
        sendMessageToHub(message);
        textInput.value = "";
    });

    textInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            submitButton.click();
        }
    });
}

/**
 * REWRITTEN: Generates the Broad-Box Instagram Style UI
 */
function addMessageToChat(message) {
    if (!chat) return;

    const isMe = (message.userName || "") === (username || "");

    // 1. Create main wrapper
    const messageItem = document.createElement('div');
    messageItem.className = isMe ? "message-item sent" : "message-item";

    // 2. Format the time
    let timeString = "";
    try {
        timeString = new Date(message.when).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    } catch {
        timeString = message.when || "";
    }

    // 3. Set the inner HTML structure (Broad Box Style)
    messageItem.innerHTML = `
        <div class="msg-bubble shadow-sm">
            <span class="sender-name">${message.userName || "Unknown"}</span>
            <div class="msg-content">${message.text || ""}</div>
            <span class="msg-time">${timeString}</span>
        </div>
    `;

    // 4. Add to chat and scroll
    chat.appendChild(messageItem);

    // Smooth scroll to the newest message
    chat.scrollTo({
        top: chat.scrollHeight,
        behavior: 'smooth'
    });
}