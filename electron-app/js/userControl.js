export async function getServerUserInfo(usersArray, users, members){

    await fetch('http://localhost:44348/api/userinfo/userinfo', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': localStorage.getItem('token')
        },
    
        body: JSON.stringify({
            _id: usersArray
        })
    })
    .then(response => response.json())
    .then(data => {
        usersArray = data;

        members.forEach(m => {
            const role = document.createElement('h1');
            role.textContent = m.Name
            role.classList.add('roles');
            users.appendChild(role);
        
            m.Users.forEach(u =>{
                usersArray.forEach(su => {
                    if (u.$oid === su._id.$oid ){
                        const user = document.createElement('div');
                        user.className = 'flex-row flex-center';
                        const userIcon = document.createElement('img');
                        userIcon.className = 'profile-img';
                        userIcon.src = 'data:image/png;base64,' + su.ProfileImg;
                        userIcon.id = u.$oid + ' img';
                        const userName = document.createElement('div');
                        userName.textContent = su.Nickname;
                        userName.id = u.$oid + ' user';

                        user.appendChild(userIcon);
                        user.appendChild(userName);
                        users.appendChild(user);
                    }
                });
            });
        });
    })
    .catch(error => {
        console.error('Ошибка:', error);
        setTimeout(getServerUserInfo, 2000);
    });
}