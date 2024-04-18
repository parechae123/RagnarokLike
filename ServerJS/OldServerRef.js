const express = require('express');
const http = require('http');
const { userInfo } = require('os');
const { parse } = require('path');
const socketIO = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = socketIO(server);
let UserList = [];
let emptyListArray = [];




// 정적 파일 제공을 위해 public 폴더를 사용합니다.
app.use(express.static(__dirname + '/public'));

// 클라이언트가 Socket.IO에 연결될 때 실행됩니다.
io.on('connection', (socket) => {
  console.log('a user connected');
  
  const tempName = genKey(8);
  console.log("유저등록 : ");

  let UserInfo = 
  {
    userListIndex: -1,
    userServerID: ''
  };
  UserInfo.userServerID = tempName;

  for (let i = 0; i < UserList.length; i++) 
  {
    if(UserList[i] !== '')
    {
      socket.emit('connectUser',UserList[i]);
    }
  }
  if(emptyListArray.length>0)
  {
    const tempNum =emptyListArray.pop();
    emptyListArray.splice(emptyListArray.length);
    UserList[tempNum] = tempName;
    UserInfo.userListIndex = tempNum;
    console.log(emptyListArray,'빈 리스트 사용했나');
  }
  else
  {
    UserList.push(tempName);
    UserInfo.userListIndex = UserList.length-1;
  }
  io.emit('connectUser',tempName);
  socket.emit('setUserInfo',UserInfo);
  console.log(UserInfo.userServerID,': ID   ',UserInfo.userListIndex,': Index ');
  console.log(UserList);

//여기까지 각 플레이어 인포 세팅



  // 클라이언트로부터 'chat message' 이벤트를 수신합니다.
  socket.on('chat message', (msg) => {
    console.log('message: ' + msg);
    
    // 모든 연결된 클라이언트에게 'chat message' 이벤트를 방출합니다.
    io.emit('chat message',msg);
    //socketIO에서는 앞에 있는 emit(주소,자료)형태로 전송해줌
  });

  socket.on('PlayerPosPacket',(pos)=>{
    console.log('들어오긴함');
    console.log(pos);
    const tempUserPos =JSON.parse(pos);
    io.emit('PlayerPosPacket',tempUserPos)
  });
  // 클라이언트가 연결을 종료할 때 실행됩니다.
  socket.on('RemoveUserInList', (disconnectedUser) => {

    console.log(disconnectedUser,'유저 제이슨파일 원본');
    const tempUserInfo = JSON.parse(disconnectedUser);
    io.emit('logOutUserInfo',tempUserInfo.userServerID);
    UserList[tempUserInfo.userListIndex] = '';
    emptyListArray.push(tempUserInfo.userListIndex);
    console.log(tempUserInfo.userServerID,'지워짐');
    console.log(tempUserInfo.userListIndex,'인덱스');
    console.log(emptyListArray,'비어있는 배열');
    console.log(UserList);
    console.log('user disconnected');
  });
});

// 서버를 3000번 포트에서 실행합니다.
server.listen(3000, () => {
  console.log('Server listening on port 3000');
});

function genKey(length){
  let result = '';
  const characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

  for(let i = 0; i< length; i++)
  {
      result += characters.charAt(Math.floor(Math.random()* characters.length));
  }
  return result;
}