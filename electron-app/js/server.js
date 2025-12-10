const { remote } = require('electron');
const { ipcRenderer } = require('electron');
let currentGroup = null;
let currentUsers = [];
const signalR = require('@microsoft/signalr');

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:44321/voicechat')
  .build();

const serverUrl = 'ws://localhost:40000';
let ws = new WebSocket(serverUrl);
let myId = null;
const peers = {};
let localStream = null;

  function loadServerData(){
  fetch('https://localhost:44358/api/server/getserver', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
                'Authorization': localStorage.getItem('token')
              },
  
              body: JSON.stringify({
                _id: localStorage.getItem('server'),
                userid: localStorage.getItem('user')
              })
              })
              .then(response => response.json())
              .then(data => {
                const chatChannels = data.ChatChannels;
                const voiceChannels = data.VoiceChannels;
                const members = data.Members;
                const serverName = data.Name;
                const serverDesc = data.Description;
  
               //document.getElementById('serverbody').className = 'flex-row elem-on';
               //document.getElementById('userinfobody').className = 'flex-row elem-off';
               document.getElementById('messages').innerHTML = ''; 
              
                const chats = document.getElementById('chatchannels');
                const vchats = document.getElementById('voicechannels');
  
               chatChannels.forEach(ch => {
                const channelBtn = document.createElement('button');
                channelBtn.title = ch.Name;
                channelBtn.value = ch._id.$oid;
                channelBtn.textContent = ch.Name;
                channelBtn.name = 'channelsButtons';
                chats.appendChild(channelBtn);
               });
  
               const voiceReciver = document.createElement('audio');
                voiceReciver.id = 'remoteAudio';
                voiceReciver.autoplay = true;
                voiceReciver.className = ('audio-off');
                vchats.appendChild(voiceReciver);
                
  
               voiceChannels.forEach(ch => {
                const channelBtn = document.createElement('button');
                channelBtn.title = ch.Name;
                channelBtn.value = ch._id.$oid;
                channelBtn.textContent = ch.Name;
                channelBtn.name = 'voicesButtons';
  
  
                channelBtn.onclick = async () => {

                  await navigator.mediaDevices.getUserMedia({ audio: true }).then((stream) => {
                    localStream = stream;
                  });

                  currentGroup = ch._id.$oid;

                  if(connection.state === 'Disconnected'){
                    await start();
                  }

                  try{
                    await joinChannel(currentGroup, myId);
                  }
                  catch{
                    console.log('соединение еще не установлено');
                    setTimeout(joinChannel, 2000);
                  }
                  
                };
  
                vchats.appendChild(channelBtn);   
              });
  
  
              const susers = document.getElementById('serverusers');
              let susersForInfo = [];
              members.forEach(m =>{
                m.Users.forEach(u => {
                  susersForInfo = [...susersForInfo, u.$oid];
                });
              });
  
              fetch('https://localhost:44348/api/userinfo/userinfo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': localStorage.getItem('token')
                },
  
                body: JSON.stringify({
                  _id: susersForInfo
                })
                })
                .then(response => response.json())
                .then(data => {
                  susersForInfo = data;
  
                  members.forEach(m => {
                  const role = document.createElement('p');
                  role.textContent = m.Name
                  susers.appendChild(role);
                  
                  m.Users.forEach(u =>{
                    susersForInfo.forEach(su => {
                      if (u.$oid === su._id.$oid ){
                        const suser = document.createElement('div');
                        suser.className = 'flex-row flex-center';
                        const suserIcon = document.createElement('img');
                        suserIcon.className = 'profile-img';
                        suserIcon.src = 'data:image/png;base64,' + su.ProfileImg;
                        const suserName = document.createElement('div');
                        suserName.textContent = su.Nickname;
  
                        suser.appendChild(suserIcon);
                        suser.appendChild(suserName);
                        susers.appendChild(suser);
                      }
                    });
                  });
                });
                })
                .catch(error => {
                console.error('Ошибка:', error);
                alert('Произошла ошибка при получении участников сервера.');
                });
              })
              .catch(error => {
              console.error('Ошибка:', error);
              alert('Произошла ошибка получения инфы о сервере.');
              setTimeout(loadData, 5000);
              });
          };
  
  
  loadServerData();



async function start() {
try {
  await connection.start();
  console.log("Подключено к хабу");
} catch (err) {
  console.error("Ошибка подключения:", err);
  setTimeout(start, 5000);
}
}

async function joinChannel(channelName, WebSocketId) {
   await connection.invoke('JoinChannel', channelName, WebSocketId);
}

async function leaveChannel(channelName) {
  await connection.invoke('LeaveChannel', channelName);
}

async function WebSocketResponse(ConnectionId, WebSocketId) {
  await connection.invoke('WebSocketResponse', ConnectionId, WebSocketId);
}

connection.on('UserJoined', (WebSocketId, ConnectionId) => {
  createOffer(WebSocketId);
  //WebSocketResponse(ConnectionId, myId);
});

connection.on('WebSocketResponse', (WebSocketId) => {
  createOffer(WebSocketId);
});












ws.onopen = () => {
console.log('Подключено к серверу');
};

ws.onmessage = (event) => {
const message = JSON.parse(event.data);
if (message.type === 'registered') {
  myId = message.id;
  console.log('Мой ID:', myId);
} else {
  // Обработка сигналов
  handleSignal(message);
}
};

function sendSignal(to, data) {
ws.send(JSON.stringify({ to: to, data: data }));
}

async function createOffer(remoteId) {
const pc = createPeerConnection(remoteId);
const offer = await pc.createOffer();
await pc.setLocalDescription(offer);
sendSignal(remoteId, { sdp: pc.localDescription });
}

function createPeerConnection(remoteId) {
const pc = new RTCPeerConnection();

// Добавляем локальный поток
if (localStream) {
  localStream.getTracks().forEach(track => pc.addTrack(track, localStream));
}

// Обработка ICE-кандидатов
pc.onicecandidate = (event) => {
  if (event.candidate) {
    sendSignal(remoteId, { candidate: event.candidate });
  }
};

// Обработка получаемых потоков
pc.ontrack = (event) => {
  let remoteAudio = document.getElementById('remoteAudio');
  if (!remoteAudio) {
    remoteAudio = document.createElement('audio');
    remoteAudio.id = 'remoteAudio';
    remoteAudio.autoplay = true;
    document.body.appendChild(remoteAudio);
  }
  remoteAudio.srcObject = event.streams[0];
};

peers[remoteId] = pc;
return pc;
}

async function handleSignal(message) {
const fromId = message.from;
const data = message.data;

if (!peers[fromId]) {
  createPeerConnection(fromId);
}
const pc = peers[fromId];

if (data.sdp) {
  await pc.setRemoteDescription(new RTCSessionDescription(data.sdp));
  if (data.sdp.type === 'offer') {
    const answer = await pc.createAnswer();
    await pc.setLocalDescription(answer);
    sendSignal(fromId, { sdp: pc.localDescription });
  }
} else if (data.candidate) {
  await pc.addIceCandidate(new RTCIceCandidate(data.candidate));
}
}
  