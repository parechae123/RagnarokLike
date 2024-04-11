const io = require('socket.io-client');

// Socket.IO 서버 URL
const socket = io('http://localhost:3000'); // 여기에는 해당하는 서버의 URL을 작성해야 합니다.

// 연결됐을 때
socket.on('connect', () => {
  console.log('Connected to server');
  // 추가 작업 가능
});

// 서버로부터 메시지 받으면 실행
socket.on('chat message', (msg) => {
  console.log('Received message:', msg);
  // 메시지 수신 후 추가 작업 가능
});

// 예시: 클라이언트에서 서버로 메시지 보내기
function sendMessage(message) {
  socket.emit('chat message', message);
}
// 예시 메시지 보내기
sendMessage('Hello from client');
const keypress = require('keypress');

// 'keypress' 라이브러리 초기화
keypress(process.stdin);

// 키 입력 이벤트 핸들러
process.stdin.on('keypress', function (ch, key) {
  // 스페이스바를 눌렀을 때 실행되는 로직
  if (key && key.name === 'space') {
    console.log('스페이스바를 눌렀습니다.');
    sendMessage('아잉');
    
    // 스페이스바를 눌렀을 때 실행할 함수 또는 코드 작성
  }
  console.log(ch.key,'넌뭐냐 ch!');
  if (key&&key.name ==='escape'){
    process.exit();
  }
});

// stdin을 'keypress' 이벤트에 바인딩
process.stdin.setRawMode(true);
process.stdin.resume();
console.log('키 입력을 기다리고 있습니다. 스페이스바를 누르면 메시지가 출력됩니다.');