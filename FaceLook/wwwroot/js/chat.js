"use strict";
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

const addUserToGroup = async () => {
    const userName = getUsernameValue();
    if (userName === null)
        return;
    connection.invoke("JoinGroupAsync", userName);
};
const removeUserFromGroup = async () => {
    const userName = getUsernameValue();
    if (userName === null)
        return;
    connection.invoke("LeaveGroupAsync", userName);
};

async function start() {
    try {
        await connection.start();
        addUserToGroup();
        console.log("SignalR Connected.");
    }
    catch (err) {
        console.error(err);
    }
};

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const sender = getUsernameValue();
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageToGroupAsync", sender, user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


connection.onclose(async () => {
    removeUserFromGroup();
});
// Start the connection.
start();
