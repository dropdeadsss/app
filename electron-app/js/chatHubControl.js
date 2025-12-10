const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:44321/chat')
  .build();

async function start() {
    try {
        await connection.start();
        console.log("Подключено к хабу");
    } catch (err) {
        console.error("Ошибка подключения:", err);
        setTimeout(start, 2000);
    }
}
    
async function joinChannel(channelName) {
    await connection.invoke('JoinChat', channelName);
}
    
async function leaveChannel(channelName) {
    await connection.invoke('LeaveChat', channelName);
}

async function sendMessge(channelName, message) {
    await connection.invoke('SendMessage', channelName, message);
}

async function isTyping(channelName, user) {
    await connection.invoke('Typing', channelName, user);
}
  
export async function hubOptions(connection){
    connection.on('ReceiveMessage', (message) => {
        
    });
        
    connection.on('UserTyping', (user) => {

    });
}