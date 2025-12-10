const serverUrl = 'ws://localhost:40000';

let ws;

export async function createWebSocketConnection(){
  ws = await new WebSocket(serverUrl);
  return ws;
}

let myId = null;
const peers = {};
//let localStream;

export async function webSocketOptions(ws){
  ws.onopen = () => {
    console.log('Подключено к серверу');
  };
  
  ws.onmessage = (event) => {
  const message = JSON.parse(event.data);
  if (message.type === 'registered') {
    myId = message.id;
    localStorage.setItem('currentWS', message.id);
    console.log('Мой ID:', myId);
  } else {
    handleSignal(message);
  }
  };

  return ws;
}
  
  function sendSignal(to, data) {
  ws.send(JSON.stringify({ to: to, data: data }));
  }
  
  export async function createOffer(remoteId, localStream) {
    const pc = createPeerConnection(remoteId, localStream);
    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);
    sendSignal(remoteId, { sdp: pc.localDescription });
  }
  
  function createPeerConnection(remoteId, localStream) {
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