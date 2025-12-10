const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:44321/server')
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
    
async function joinServer(channelName) {
    await connection.invoke('JoinServer', channelName);
}
    
async function leaveServer(channelName) {
    await connection.invoke('LeaveServer', channelName);
}

async function joinResponse(ConnectionId, user){
    await connection.invoke('JoinServerResponse', ConnectionId, user)
}

async function channelChange(channelName, user, from, to){
    await connection.invoke('ChannelChange', user, from, to)
}

export async function hubOptions(connection, myuser){

    connection.on('UserJoinedServer', (ConnectionId) => {
        joinResponse(ConnectionId, myuser);
    });
          
    connection.on('UserJoinedServerResponse', (user) => {
        
    });

    return connection;
}