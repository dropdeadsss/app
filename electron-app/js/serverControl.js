import * as userControl from './userControl.js';
import * as webSocketControl from './webSocketControl.js';
import * as voiceHubControl from './voiceChatHubControl.js';
import * as chatControl from './chatControl.js';
const  signalR  = require('@microsoft/signalr');

let micro;
await navigator.mediaDevices.getUserMedia({ audio: true})
.then(stream => {
    micro = stream;
});

let ws = null;
let hub = null; 


async function loadServerData(){
    await fetch('http://localhost:44358/api/server/getserver', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': localStorage.getItem('token')
        },

        body: JSON.stringify({
            _id: localStorage.getItem('currentServer'),
            userid: localStorage.getItem('user')
        })
    })
    .then(response => response.json())
    .then(data => {
        if(data != null){
            localStorage.setItem('server-'+localStorage.getItem('currentServer'),  JSON.stringify(data));
        }
       
    })
    .catch(error => {
        console.error('Ошибка:', error);
        setTimeout(loadServerData, 2000);
    });
}


export async function createServerHtml(serverid){
    document.getElementById('selectChChat').className = 'flex-col flex-center col-border elem-enable';
    document.getElementById('chchat').className = 'flex-col col-border elem-disable';

    if(localStorage.getItem('server-'+serverid) == null){
        await loadServerData();
    }
    try{
        document.getElementById('serversDiv').className = 'flex-row elem-enable';
        document.getElementById('usersDiv').className = 'flex-row elem-disable';
        
        const data = JSON.parse(localStorage.getItem('server-'+serverid));


        const chatChannels = data.ChatChannels;
        const voiceChannels = data.VoiceChannels;
        const members = data.Members;
        const serverName = data.Name;
        const serverDesc = data.Description;

        document.getElementById('messages').innerHTML = ''; 

        const chats = document.getElementById('chatchannels');
        chats.innerHTML = '';
        const chatLabel = document.createElement('p');
        chatLabel.textContent = 'Текcтовые каналы'
        chats.appendChild(chatLabel);
        const vchats = document.getElementById('voicechannels');
        const vchatLabel = document.createElement('p');
        vchatLabel.textContent = 'Голосовые каналы'
        vchats.innerHTML = '';
        vchats.appendChild(vchatLabel);
        
        chatChannels.forEach(ch => {
            const channelBtn = document.createElement('button');
            channelBtn.title = ch.Name;
            channelBtn.value = ch._id.$oid;
            channelBtn.textContent = ch.Name;
            channelBtn.name = 'channelsButtons';

            channelBtn.addEventListener('click', () =>{
                document.getElementById('selectChChat').className = 'flex-col flex-center col-border elem-disable';
                document.getElementById('chchat').className = 'flex-col col-border elem-enable';
                localStorage.setItem('currentServerChat', channelBtn.value)

                chatControl.getServerChat(channelBtn.value);
            });

            channelBtn.classList.add('channels-btn');
            chats.appendChild(channelBtn);
        });

        const voiceReciver = document.createElement('audio');
        voiceReciver.id = 'remoteAudio';
        voiceReciver.autoplay = true;
        voiceReciver.className = ('elem-disable');
        vchats.appendChild(voiceReciver);
    

        voiceChannels.forEach(ch => {
            const channelBtn = document.createElement('button');
            channelBtn.title = ch.Name;
            channelBtn.value = ch._id.$oid;
            channelBtn.textContent = ch.Name;
            channelBtn.name = 'voicesButtons';
            channelBtn.classList.add('channels-btn');

            channelBtn.onclick = async () => {

                if(channelBtn.value != localStorage.getItem('currentServerVoiceChat')){
                    if(hub && hub._connectionState === 'Connected'){
                        await voiceHubControl.leaveChannel(hub, localStorage.getItem('currentServerVoiceChat'));
                        hub.stop();  
                        hub = null;
                    }
                    localStorage.setItem('currentServerVoiceChat', channelBtn.value);
                }

                if(!ws){
                    ws = await webSocketControl.createWebSocketConnection();
                    ws = await webSocketControl.webSocketOptions(ws);
                }

                if(!hub){
                    hub = await voiceHubControl.createConnetion();
                    await voiceHubControl.hubOptions(hub, micro);
                    await voiceHubControl.start(hub);

                    await voiceHubControl.joinChannel(hub, localStorage.getItem('currentServerVoiceChat'), localStorage.getItem('currentWS'));
                }            
            };

            vchats.appendChild(channelBtn);   
        });


        const users = document.getElementById('serverusers');
        users.innerHTML = '';

        let usersArray = [];
        members.forEach(m =>{
            m.Users.forEach(u => {
                usersArray = [...usersArray, u.$oid];
            });
        });

        await userControl.getServerUserInfo(usersArray, users, members);
    }
    catch{

    }
}