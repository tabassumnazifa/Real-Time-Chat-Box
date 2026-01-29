// Connect to SignalR hub
var connection = new signalR.HubConnectionBuilder()
    .withUrl('/chathub')
    .build();

// Receive messages from server
connection.on('ReceiveMessage', (username, text, sentAt) => {
    const message = { userName: username, text: text, when: sentAt };
    addMessageToChat(message);
});

// Start connection
connection.start()
    .then(() => console.log("Connected to chat hub"))
    .catch(error => console.error(error));

// Send message to hub
function sendMessageToHub(message) {
    connection.invoke('SendMessage', message.text)
        .catch(err => console.error(err));
}
