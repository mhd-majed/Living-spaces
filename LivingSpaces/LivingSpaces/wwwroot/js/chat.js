const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

function startConnection() {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        connection.start().then(function () {
            console.log("Connected to SignalR hub.");
        }).catch(function (err) {
            console.error("Error connecting to SignalR hub:", err.toString());
        });
    }
}

startConnection();
function scrollToBottom() {
    const messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight;
}
async function sendMessage() {
    const userId = document.getElementById("recipientUserId").value;
    const message = document.getElementById("messageInput").value;

    if (userId && message) {
        await connection.invoke("SendMessage", userId, message);
        document.getElementById("messageInput").value = '';
    } else {
        alert("Please select a recipient and enter a message.");
    }
}
connection.on("ReceiveMessage", function (senderId, message) {
    const messageElement = document.createElement("div");
    messageElement.classList.add("message");
    console.log(senderId)

    const currentUserId = document.getElementById("recipientUserId").value;
    console.log(currentUserId)

    if (senderId != currentUserId) {
        messageElement.classList.add("sent");
    } else {
        messageElement.classList.add("received");

        const imgElement = document.createElement("img");
        imgElement.src = "https://via.placeholder.com/40";
        imgElement.alt = "User";
        messageElement.appendChild(imgElement);
    }

    const textElement = document.createElement("div");
    textElement.classList.add("text");
    textElement.textContent = message;
    messageElement.appendChild(textElement);

    document.getElementById("messagesList").appendChild(messageElement);

    scrollToBottom();
});
document.addEventListener('DOMContentLoaded', function () {
    const sendButton = document.getElementById("sendButton");
    if (sendButton) {
        sendButton.addEventListener("click", sendMessage);
    }
});
