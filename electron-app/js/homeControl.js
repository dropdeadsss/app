import * as chatControl from './chatControl.js';
import * as serverControl from './serverControl.js';

async function loadIndexData(){
    await fetch('http://localhost:44348/api/userinfo/getselfuserinfo', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': localStorage.getItem('token')
        },
        body: JSON.stringify({})
    })
    .then(response => response.json())
    .then(data => {
      if(data != null)
      {
        localStorage.setItem('homeData', JSON.stringify(data))
      }
    })
    .catch(error => {
        console.error('Ошибка:', error);
        setTimeout(loadIndexData, 2000)
    });
};

export async function createHomeHtml(){
    document.getElementById('selectChat').className = 'flex-col flex-center col-border elem-enable';
    document.getElementById('chat').className = 'flex-col col-border elem-disable';

    if(localStorage.getItem('homeData') != null){
        document.getElementById('usersDiv').className = 'flex-row elem-enable';
        document.getElementById('serversDiv').className = 'flex-row elem-disable';
    }
    else{
        try{    
            await loadIndexData();

            const data = JSON.parse(localStorage.getItem('homeData'));
            
            const userid = data._id.$oid;
            const blocks = data.Blocked;
            const friends = data.Friends;
            const friendinvs = data.FriendInvites;
            const friendreqs = data.FriendRequests;
            const servers = data.Servers;
            const nick = data.Nickname;
            const name = data.Username;

            localStorage.removeItem('user');
            localStorage.setItem('user', userid);
            
            const userSelfImg = document.createElement('img');
            userSelfImg.src = 'data:image/png;base64,' + data.ProfileImg;
            userSelfImg.className = 'profile-img';
            userSelfImg.id = userid + ' img'; 

            const userSelfName = document.createElement('div');
            userSelfName.textContent = nick;
            userSelfName.value = name;
            userSelfName.id = userid + ' user'; 

            localStorage.setItem('username', name);
            localStorage.setItem('nickname', nick);
            localStorage.setItem('pfp', 'data:image/png;base64,' + data.ProfileImg);

            const userSelfLogOutBtn = document.createElement('button');
            userSelfLogOutBtn.id = 'logoutBtn';

            userSelfLogOutBtn.addEventListener('click', () => {
                localStorage.clear('token');
                ipcRenderer.send('logout');
            });


            const userSelfLogOutImg = document.createElement('img');
            userSelfLogOutImg.src = 'imgs/exit.png';
            userSelfLogOutImg.className = 'button-img';

            userSelfLogOutBtn.appendChild(userSelfLogOutImg);

            const userSelf = document.getElementById('userSelf');
            userSelf.appendChild(userSelfImg);
            userSelf.appendChild(userSelfName);
            userSelf.appendChild(userSelfLogOutBtn);

        //document.getElementById('serverbody').className = 'flex-row elem-off';

            const serversCont = document.getElementById('servers');
            const homeBtn = document.createElement('button');
            homeBtn.id = 'homebtn';
            homeBtn.title = 'Главная';
            homeBtn.className = 'server-list-elems';
            homeBtn.onclick = () => {

                document.getElementById('serversDiv').className = 'flex-row elem-disable';
                document.getElementById('usersDiv').className = 'flex-row elem-enable';

                createHomeHtml();
            };

            const homeBtnImg = document.createElement('img');
            homeBtnImg.src = 'imgs/home.png';
            homeBtnImg.className = 'home-img';
            homeBtnImg.alt = 'Главная';
            homeBtn.appendChild(homeBtnImg);

            const addBtn = document.createElement('button');
            addBtn.id = 'addbtn';
            addBtn.title = 'Добавить';
            addBtn.className = 'server-list-elems';
            const addBtnImg = document.createElement('img');
            addBtnImg.src = 'imgs/add.png';
            addBtnImg.className = 'server-img';
            addBtnImg.alt = 'Добавить';
            addBtn.appendChild(addBtnImg);
        
            serversCont.appendChild(homeBtn);

            servers.forEach(element => {
                const serv = document.createElement('button');
                serv.id = 'serverbtn';
                serv.title = element.Name;
                serv.value = element._id.$oid;
                serv.className = 'server-list-elems';

                serv.addEventListener('click', () => {
                    document.getElementById('usersDiv').className = 'flex-row elem-disable';
                    document.getElementById('serversDiv').className = 'flex-row elem-enable';
                    localStorage.setItem('currentServer', serv.value)
                    serverControl.createServerHtml(localStorage.getItem('currentServer'));
                });


                const servImg = document.createElement('img');
                servImg.src = 'data:image/png;base64,' + element.Icon;
                servImg.alt = element.Name;
                servImg.className = 'server-img';
                serv.appendChild(servImg);
                serversCont.appendChild(serv);
            });


            serversCont.appendChild(addBtn);

            const friendCont = document.getElementById('friends');
            friends.forEach(el => {
                createUserContainer(el, friendCont);
            });

            const inviteCont = document.getElementById('invites');
            friendinvs.forEach(el => {
                createUserContainer(el, inviteCont);
            });

            const reqCont = document.getElementById('requests');
            friendreqs.forEach(el => {
                createUserContainer(el, reqCont);
            });

            const blockCont = document.getElementById('blocks');
            blocks.forEach(el => {
                createUserContainer(el, blockCont);
            });


            const userButtons = document.getElementsByName('userbtn')
            userButtons.forEach(b =>{
                localStorage.setItem('currentUserChat', b.value) 

                b.addEventListener('click', () => {
                    chatControl.getPersonalChat(localStorage.getItem('currentUserChat'));
                });
            });            
        }
        catch(error){
            console.log(error)
            setTimeout(createHomeHtml,5000)
        }
    }
}


async function createUserContainer(el, container){
    const chel = document.createElement('button');
    chel.className = 'flex-row flex-center people-list-elems';
    chel.title = el.Nickname + '('+ el.Username + ')';
    chel.name = 'userbtn';
    chel.value = el._id.$oid;
    const chelImg = document.createElement('img');
    chelImg.src = 'data:image/png;base64,' + el.ProfileImg;
    chelImg.className = 'profile-img';
    chelImg.id = el._id.$oid + ' img'; 
    const chelName = document.createElement('div');
    chelName.textContent = el.Nickname;
    chelName.value = el.Username;
    chelName.id = el._id.$oid + ' user'; 
    chel.appendChild(chelImg);
    chel.appendChild(chelName);
    container.appendChild(chel);
}