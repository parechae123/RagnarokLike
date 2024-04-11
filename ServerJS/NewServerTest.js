const io = require('socket.io-client');

const socket = io('http://localhost:3000');

let UserCount = 0;


socket.on('Connect',(asdf)=>{
    UserCount++;
    console.log(UserCount);
});

socket.on('InputSomeThing',(text)=>{

    console.log(text+'Said Client');
});

console.log('http://localhost:3000 에서 서버 열림');

const keypress = require('keypress');
keypress(process.stdin);
// 키 입력 이벤트 핸들러
process.stdin.on('keypress', function (ch, key) {
    // 스페이스바를 눌렀을 때 실행되는 로직
    if (key && key.name === 'space') {
      console.log('스페이스바를 눌렀습니다.');
      
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