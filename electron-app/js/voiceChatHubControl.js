import { createOffer } from './webSocketControl.js';

const  signalR  = require('@microsoft/signalr');



export async function createConnetion() {
    let connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:44321/voicechat', {
        transport: signalR.HttpTransportType.LongPolling
    })
    .build();

    return connection;
}

export async function start(connection) {
    try {
        await connection.start();
        console.log("Подключено к хабу");
    } catch (err) {
        console.error("Ошибка подключения:", err);
        setTimeout(start, 2000);
    }
}
    
export async function joinChannel(connection, channelName, WebSocketId) {
    await connection.invoke('JoinChannel', channelName, WebSocketId);
}
    
export async function leaveChannel(connection, channelName) {
    await connection.invoke('LeaveChannel', channelName);
}

/*export async function WebSocketResponse(connection, ConnectionId, WebSocketId) {
    await connection.invoke('WebSocketResponse', ConnectionId, WebSocketId);
}*/

export async function hubOptions(connection, micro){
    connection.on('UserJoined', (WebSocketId, ConnectionId) => {
        createOffer(WebSocketId, micro);
        //WebSocketResponse(connection, ConnectionId, WebSocketId)
    });

   /*connection.on('WebSocketResponse', (WebSocketId) => {
        createOffer(WebSocketId, micro);
    });*/
}