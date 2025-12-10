import * as commonFunctions from './commonFunctions.js';


async function createMessages(data, messageCont){
  let mesDate;
  let date = new Date();
  let currentDate = date.toLocaleDateString();
  date.setDate(date.getDate() - 1);
  let yesterdayDate = date.toLocaleDateString();
  let who = 0;

  data.forEach(el =>{
      if(el.From.$oid === localStorage.getItem('user'))
        who = 1;       
      else
        who = 0;

      if(mesDate != new Date(el.DateTime.$date).toLocaleDateString()){
        mesDate = new Date(el.DateTime.$date).toLocaleDateString();
        const mesDateDiv = document.createElement('div');
        mesDateDiv.className = 'mes-div flex-col flex-center';
        if(mesDate == currentDate)
          mesDateDiv.textContent = 'Сегодня';
        else if (mesDate == yesterdayDate)
          mesDateDiv.textContent = 'Вчера';
        else{
          mesDateDiv.textContent = mesDate;
        }
        messageCont.appendChild(mesDateDiv);
      }

      const mesCont = document.createElement('div');
      const mes = document.createElement('div');
      const time = document.createElement('div');
      const chatuserinfo = document.createElement('div');
      const chatuserimg = document.createElement('img');
      const chatusername = document.createElement('div');
      const audioContainer = document.createElement('div');

      if(who === 1){
        mesCont.className = 'flex-row own-message flex-center';
        time.className = 'flex-row own-message flex-center';
        chatuserinfo.className = 'flex-row own-message flex-center';
        audioContainer.className = 'flex-row own-message flex-center';
        chatuserimg.className = 'chat-profile-img';
        chatuserimg.src = localStorage.getItem('pfp');   
        chatusername.textContent = localStorage.getItem('nickname');
      }             
      else{
        mesCont.className = 'flex-row another-message flex-center';
        time.className = 'flex-row another-message flex-center';
        chatuserinfo.className = 'flex-row another-message flex-center';
        audioContainer.className = 'flex-row another-message flex-center';
        chatuserimg.className = 'chat-profile-img';
        chatuserimg.src = document.getElementById(userid + ' img').src;      
        chatusername.textContent = document.getElementById(userid + ' user').textContent;
      }

      mes.className = 'flex-col message';

      time.textContent = new Date(el.DateTime.$date).toLocaleTimeString();                            

      chatuserinfo.appendChild(chatuserimg);
      chatuserinfo.appendChild(chatusername)

      mes.appendChild(time);
      mes.appendChild(chatuserinfo);
      
      if (el.IsVoice === false){
          const messagetext = document.createElement('div');
          messagetext.className = 'text-cont-mes';
          messagetext.textContent = el.Content;
          messagetext.id = el._id.$oid;
          mes.appendChild(messagetext);
      }
      else if (el.IsVoice === true){
          const audioMes = document.createElement('audio');
          audioMes.controls = true;
          const audioBlob = commonFunctions.base64ToBlob(el.Content.$binary.base64,'audio/wav');
          const audioUrl = URL.createObjectURL(audioBlob);
          audioMes.src = audioUrl;
          audioMes.id = el._id.$oid;

          const audioDecodeBtn = document.createElement('button');
          audioDecodeBtn.id = el._id.$oid + ' b';
          audioDecodeBtn.textContent = 'Аа';
          audioDecodeBtn.className = 'decode';

          audioDecodeBtn.addEventListener('click', () => {
              audioDecodeBtn.classList.remove('elem-enable');
              audioDecodeBtn.classList.add('elem-disable');

              const file = new FormData();
              file.append('file', audioBlob, 'audio.wav');

              transcribe(file, mes, audioDecodeBtn);    
          });

          if(who === 1){
            audioContainer.appendChild(audioDecodeBtn);
            audioContainer.appendChild(audioMes);
          }
          else{
            audioContainer.appendChild(audioMes);
            audioContainer.appendChild(audioDecodeBtn);
          }
          mes.appendChild(audioContainer);
      }
      
      mesCont.appendChild(mes);
      messageCont.appendChild(mesCont);
      
  });

  messageCont.scrollTop = messageCont.scrollHeight;
}

export async function transcribe(audioFile, mes, audioDecodeBtn)
{
    await fetch('http://localhost:45000/transcribe', {
        method: 'POST',
        body: audioFile
    })
    .then(response => response.json())
    .then(data => {
        const decodedAudio = document.createElement('textarea');
        decodedAudio.cols = 40;
        decodedAudio.rows = 3;
        decodedAudio.className = 'text-cont';
        decodedAudio.textContent = data;
        mes.appendChild(decodedAudio);
    }) 
    .catch(error => {
        console.error('Ошибка:', error);
        audioDecodeBtn.classList.remove('elem-disable');
        audioDecodeBtn.classList.add('elem-enable');
    });
}

export async function getPersonalChat(userid){

    document.getElementById('selectChat').className = 'flex-col flex-center col-border elem-disable';
    document.getElementById('chat').className = 'flex-col col-border elem-enable';

    fetch('http://localhost:44390/api/personalchat/getmessages', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': localStorage.getItem('token')
        },

        body: JSON.stringify({
            _id: localStorage.getItem('user'),
            userid:  userid,
            offset: 0
        })
    })
    .then(response => response.json())
    .then(data => {

        const messageCont = document.getElementById('messages');
        messageCont.innerHTML = '';
        createMessages(data, messageCont);
    })
    .catch(error => {
        console.error('Ошибка:', error);
        setTimeout(getPersonalChat, 2000);
    });
}


const recordBtn = document.getElementById('sendvoicebtn');
const audioPlaybackCont = document.getElementById('recordcont');
const audioPlayback = document.getElementById('audioPlayback');
const recordBtnImg = document.getElementById('sendvoicebtnimg');
const sendMesBtn = document.getElementById('sendmessagebtn');
const messContent = document.getElementById('messagecontent');

let byteArray;
let mediaRecorder;
let chunks = [];

recordBtn.onclick = async () => {
  if (mediaRecorder && mediaRecorder.state === "recording") {
    mediaRecorder.stop();
    audioPlaybackCont.className = 'flex-row flex-center mes-audio elem-enable';
    recordBtnImg.src = 'imgs/voice.png';
  } 
  else {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      
      mediaRecorder = new MediaRecorder(stream);
      
      chunks = [];
      
      mediaRecorder.ondataavailable = e => {
        chunks.push(e.data);
      };
      
      mediaRecorder.onstop = () => {
        const blob = new Blob(chunks, { type: 'audio/wav' });
        const url = URL.createObjectURL(blob);
        
        blob.arrayBuffer().then(buffer => {
          byteArray = new Uint8Array(buffer);
        });
        audioPlayback.src = url;
      };
      
      mediaRecorder.start();
      audioPlaybackCont.className = 'elem-disable';
      recordBtnImg.src = 'imgs/stop.png';
    } catch (err) {
      alert("Ошибка доступа к микрофону: " + err);
    }
  }
};

sendMesBtn.onclick = async () => {
  if (mediaRecorder && mediaRecorder.state === "recording") {
    alert('Остановите запись');
  }
  else if (chunks.length > 0){
    fetch('http://localhost:44390/api/personalchat/sendvoicemessage', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': localStorage.getItem('token')
    },

    body: JSON.stringify({
      _id: localStorage.getItem('user'),
      userid:  localStorage.getItem('currentUserChat'),
      content: Array.from(byteArray),
      isvoice: true,
      duration: Math.ceil(audioPlayback.duration),
      datetime: new Date()
    })
    })
    .then(response => response.status)
    .then(data => {
      if(data == 200){
        audioPlayback.src = '';
        audioPlaybackCont.className = 'flex-row flex-center elem-disable';
        chunks = [];
      }
    })
    .catch(error => {
      console.error('Ошибка:', error);
      alert('Произошла ошибка отправки голосового сообщения.');
    });
  }
  else if (messContent.value.trim() !== '' && messContent.value.trim() !== undefined && messContent.value.trim() !== null){
    fetch('http://localhost:44390/api/personalchat/sendmessage', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': localStorage.getItem('token')
    },

    body: JSON.stringify({
      _id: localStorage.getItem('user'),
      userid:  localStorage.getItem('currentUserChat'),
      content: messContent.value,
      isvoice: false,
      imgs: [],
      files: [],
      datetime: new Date()
    })
    })
    .then(response => response.status)
    .then(data => {
      if(data == 200){
        messContent.value = '';
      }
    })
    .catch(error => {
      console.error('Ошибка:', error);
      alert('Произошла ошибка отправки текстового сообщения.');
    });
  }
};


export async function getServerChat(chatid){
  
  document.getElementById('selectChat').className = 'flex-col flex-center col-border elem-disable';
  document.getElementById('chat').className = 'flex-col col-border elem-enable';

  fetch('http://localhost:44390/api/channelchat/getmessages', {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': localStorage.getItem('token')
      },

      body: JSON.stringify({
          _id: chatid,
          userid:  localStorage.getItem('user'),
          offset: 0
      })
  })
  .then(response => response.json())
  .then(data => {

      const messageCont = document.getElementById('chmessages');
      messageCont.innerHTML = '';
      createMessages(data, messageCont);
  })
  .catch(error => {
      console.error('Ошибка:', error);
      setTimeout(getPersonalChat, 2000);
  });
}

const chrecordBtn = document.getElementById('chsendvoicebtn');
const chaudioPlaybackCont = document.getElementById('chrecordcont');
const chaudioPlayback = document.getElementById('chaudioPlayback');
const chrecordBtnImg = document.getElementById('chsendvoicebtnimg');
const chsendMesBtn = document.getElementById('chsendmessagebtn');
const chmessContent = document.getElementById('chmessagecontent');

let chbyteArray;
let chmediaRecorder;
let chchunks = [];

chrecordBtn.onclick = async () => {
  if (chmediaRecorder && chmediaRecorder.state === "recording") {
    chmediaRecorder.stop();
    chaudioPlaybackCont.className = 'flex-row flex-center mes-audio elem-enable';
    chrecordBtnImg.src = 'imgs/voice.png';
  } 
  else {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      
      chmediaRecorder = new MediaRecorder(stream);
      
      chchunks = [];
      
      chmediaRecorder.ondataavailable = e => {
        chchunks.push(e.data);
      };
      
      chmediaRecorder.onstop = () => {
        const blob = new Blob(chchunks, { type: 'audio/wav' });
        const url = URL.createObjectURL(blob);
        
        blob.arrayBuffer().then(chbuffer => {
          chbyteArray = new Uint8Array(chbuffer);
        });
        chaudioPlayback.src = url;
      };
      
      chmediaRecorder.start();
      chaudioPlaybackCont.className = 'elem-disable';
      chrecordBtnImg.src = 'imgs/stop.png';
    } catch (err) {
      alert("Ошибка доступа к микрофону: " + err);
    }
  }
};

chsendMesBtn.onclick = async () => {
  if (chmediaRecorder && chmediaRecorder.state === "recording") {
    alert('Остановите запись');
  }
  else if (chchunks.length > 0){
    fetch('http://localhost:44390/api/channelchat/sendvoicemessage', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': localStorage.getItem('token')
    },

    body: JSON.stringify({
      _id: localStorage.getItem('currentServerChat'),
      userid:  localStorage.getItem('user'),
      content: Array.from(chbyteArray),
      isvoice: true,
      duration: Math.ceil(chaudioPlayback.duration),
      datetime: new Date()
    })
    })
    .then(response => response.status)
    .then(data => {
      if(data == 200){
        chaudioPlayback.src = '';
        chaudioPlaybackCont.className = 'elem-disable';
        chchunks = [];
      }
    })
    .catch(error => {
      console.error('Ошибка:', error);
      alert('Произошла ошибка отправки голосового сообщения.');
    });
  }
  else if (chmessContent.value.trim() !== '' && chmessContent.value.trim() !== undefined && chmessContent.value.trim() !== null){
    fetch('http://localhost:44390/api/channelchat/sendmessage', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': localStorage.getItem('token')
    },

    body: JSON.stringify({
      _id: localStorage.getItem('currentServerChat'),
      userid:  localStorage.getItem('user'),
      content: chmessContent.value,
      isvoice: false,
      imgs: [],
      files: [],
      datetime: new Date()
    })
    })
    .then(response => response.status)
    .then(data => {
      if(data == 200){
        chmessContent.value = '';
      }
    })
    .catch(error => {
      console.error('Ошибка:', error);
      alert('Произошла ошибка отправки текстового сообщения.');
    });
  }
};



const deletebtn = document.getElementById('audioPlaybackdelete');
const chdeletebtn = document.getElementById('chaudioPlaybackdelete');

deletebtn.onclick = async () =>{
  audioPlayback.src = '';
  audioPlaybackCont.className = 'elem-disable';
};

chdeletebtn.onclick = async () =>{
  chaudioPlayback.src = '';
  chaudioPlaybackCont.className = 'elem-disable';
};