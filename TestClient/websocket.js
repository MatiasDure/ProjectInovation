class SocketClient {
    constructor(id, websocket)
    {
        this.id = id;
        this.websocket = websocket;
    }
}

var selfClient = new SocketClient(-1,null);
var selfId = -1;
var origin = window.location.origin;
var words = origin.split(':'); // typically: words[0]= "http", words[1] = something like "//192.168.0.1", words[2] = "8000" (the http server port)	
var wsUri = "ws:"+words[1];    
var wsPortInlcusion = wsUri+":4444/";
//wsUri = "ws://127.0.0.1/",    // works
//wsUri = "ws://192.168.2.8/",  // This works if this is the LAN address of the web socket server
var websocket = new WebSocket(wsPortInlcusion);
// http://www.websocket.org/echo.html


//button.addEventListener("click", onClickButton);

// document.addEventListener("switchedScene", (onSwitchedScene) => {
//     websocket.doSend(selfId + ":ss");
// });

websocket.onopen = function (e) {
};

websocket.onclose = function (e) {
};

websocket.onmessage = function (e) {
let arr = e.data.split(":");
    if(arr[0] === "ja" && selfId < 0)
    {
        selfClient = new SocketClient(arr[1], websocket);
        console.log("Client id is: " + selfClient.id);

        selfId = arr[1];

        const onIdProvided = new CustomEvent("idProvided",{ detail : { idProvided: selfId }});
        document.dispatchEvent(onIdProvided);
    }
    else if(arr[0] === "ca")
    {
        const onCharAccepted = new CustomEvent("charAccepted",{ detail : { character: arr[1] }});
        document.dispatchEvent(onCharAccepted);
    }
    else if(arr[0] === "cc")
    {
        console.log("clients conneced " + arr[1]);
        //inform amount of clients connected through dispatch
        const onNewClientConnected = new CustomEvent("clientConnected", { detail: { amountConnected: arr[1] }});
        document.dispatchEvent(onNewClientConnected);
    }
    else if(arr[0] === "gf")
    {
        const onPlayerWon = new CustomEvent("playerWon", { detail: { playerName: arr[1] }});
        document.dispatchEvent(onPlayerWon);
    }
};

websocket.onerror = function (e) {
};

var lastMessageSent = 0;
function doSend(message) {
    websocket.send(message);
    lastMessageSent = Date.now();
}

//sends some data through to keep the connection awake and avoid extreme package grouping done by the nagle algorithm
setInterval(function() {
if (Date.now() - lastMessageSent >= 30) {
    doSend("filler data");
    console.log("Sent filler data");
}
}, 10); 

//dot code --------------------------------------
const square = (number) => number ** 2;
var startPos = [ , ];   //start point
var endPos = [ , ];     //end point
var vecLength;          //vector Length of start and end vector
var vectorString = "";

//On Touching the screen
document.addEventListener("touchstart", e => {
if(document.getElementById("controller").classList.contains("hidden")) return;
[...e.changedTouches].forEach(touch => {        //e.changedTouches is technically a list--> it does not have arry functions --> need to convert it to array first [..]
    const dot = document.createElement("div")
    dot.classList.add("dot");                    //create a class and assign dot to dot class
    
    // Store vector 
    startPos[0] = touch.pageX;
    startPos[1] = touch.pageY;
    
    //Set the dot pos to center of touchpoint (for showing)
    const dotWidth = dot.offsetWidth;
    const dotHeight = dot.offsetHeight;
    const centerX = touch.pageX - (dotWidth / 2);
    const centerY = touch.pageY - (dotHeight / 2);
    dot.style.top = `${centerY}px`;
    dot.style.left = `${centerX}px`;
    
    dot.id = touch.identifier;           //each touch receives an id
    document.body.append(dot);           //append to see
})
})           

//On Touch moving
document.addEventListener("touchmove", e => {
    if(document.getElementById("controller").classList.contains("hidden")) return;
[...e.changedTouches].forEach(touch =>{     //for each touch
    const dot = document.getElementById(touch.identifier);
    
    //set dot position to center of touchpoint
    const dotWidth = dot.offsetWidth;
    const dotHeight = dot.offsetHeight;
    const centerX = touch.pageX - (dotWidth / 2);
    const centerY = touch.pageY - (dotHeight / 2);
    
    dot.style.top = `${centerY}px`;
    dot.style.left = `${centerX}px`;
})
})

//Disables default browser events when moving (NO ZOOM, NO REFRESH ON PULL)
document.addEventListener('touchmove', function(event) {
    if(document.getElementById("controller").classList.contains("hidden")) return;
event.preventDefault();
}, { passive: false }); // can call preventDefault() without causing any performance issues.


//When letting go of touchscreen
document.addEventListener("touchend", e => {
    if(document.getElementById("controller").classList.contains("hidden")) return;
[...e.changedTouches].forEach(touch =>{     //for each touch
    const dot = document.getElementById(touch.identifier);
    dot.remove();
    
    endPos[0] = touch.pageX;
    endPos[1] = touch.pageY;
    
    //console.log(startPos + " startVector");
    //console.log(endPos + " endVector");
    
    //Calculate Vector and send as string
    var vector = VectorCalculation(startPos, endPos);
    vectorString = (-vector[0]) + "," + vector[1];
    movementString = ":m:"+vectorString;
    websocket.send(selfId + movementString);
    h2.innerHTML = vectorString;
})
})

//Calculating Vector of touchstart and touchend
function VectorCalculation(_startPos, _endPos){
    //Vector of startVec , endVec
    var Vector = [ , ];
    Vector[0] = _endPos[0] - _startPos[0];      //X
    Vector[1] = _endPos[1] - _startPos[1];      //Y
    
    return Vector
}

//Calculating Length of vector
function VectorLength(_startPos, _endPos){
    var Vector = [ , ];
    Vector[0] = _endPos[0] - _startPos[0];      //X
    Vector[1] = _endPos[1] - _startPos[1];      //Y
    
    //Length Calculator
    return Math.sqrt(square(Vector[0]) + square(Vector[1]));
}
                

//whole default script-------------------------
// var selfId = -1;
// var origin = window.location.origin;
// h2 = document.querySelector("h2");//.innerHTML = origin;
// var words = origin.split(':'); // typically: words[0]= "http", words[1] = something like "//192.168.0.1", words[2] = "8000" (the http server port)	
// var wsUri = "ws:"+words[1]+"/";    

// // http://www.websocket.org/echo.html

// var button = document.querySelector("button"),
// output = document.querySelector("#output"),
// textarea = document.querySelector("textarea"),
// //wsUri = "ws://127.0.0.1/",    // works
// //wsUri = "ws://192.168.2.8/",  // This works if this is the LAN address of the web socket server
// websocket = new WebSocket(wsUri);

// button.addEventListener("click", onClickButton);

// websocket.onopen = function (e) {
//     writeToScreen("CONNECTED");
// };

// websocket.onclose = function (e) {
//     writeToScreen("DISCONNECTED");
// };

// websocket.onmessage = function (e) {
//     writeToScreen("<span>RESPONSE: " + e.data + "</span>");
//     let arr = e.data.split(":");
//     if(arr[0] === "ja" && 
//     selfId < 0)
//     {
//         selfId = arr[1];
//         writeToScreen("Your id is: "+selfId);
//     }      
// };

// websocket.onerror = function (e) {
//     writeToScreen("<span class=error>ERROR:</span> " + e.data);
// };

// var lastMessageSent = 0;
// function doSend(message) {
//     //writeToScreen("SENT: " + message);
//     websocket.send(message);
//     lastMessageSent = Date.now();
// }

// setInterval(function() {
//     if (Date.now() - lastMessageSent >= 30) {
//         websocket.send("filler data");
//     }
// }, 10); 

// function writeToScreen(message) {
//     output.insertAdjacentHTML("afterbegin", "<p>" + message + "</p>");
// }

// function onClickButton() {
//     var text = textarea.value;
    
//     text && doSend(text);
//     textarea.value = "";
//     textarea.focus();
// }

// writeToScreen("Websocket address: "+wsUri);

// //dot code -----------------------------------------------------------------
// const square = (number) => number ** 2;
// var startPos = [ , ];   //start point
// var endPos = [ , ];     //end point
// var vecLength;          //vector Length of start and end vector
// var vectorString = "";

// //On Touching the screen
// document.addEventListener("touchstart", e => {
    
//     [...e.changedTouches].forEach(touch => {        //e.changedTouches is technically a list--> it does not have arry functions --> need to convert it to array first [..]
//         const dot = document.createElement("div")
//         dot.classList.add("dot");                    //create a class and assign dot to dot class
        
//         // Store vector 
//         startPos[0] = touch.pageX;
//         startPos[1] = touch.pageY;
        
//         //Set the dot pos to center of touchpoint (for showing)
//         const dotWidth = dot.offsetWidth;
//         const dotHeight = dot.offsetHeight;
//         const centerX = touch.pageX - (dotWidth / 2);
//         const centerY = touch.pageY - (dotHeight / 2);
//         dot.style.top = `${centerY}px`;
//         dot.style.left = `${centerX}px`;
        
//         dot.id = touch.identifier;           //each touch receives an id
//         document.body.append(dot);           //append to see
        
//     })
// })           

// //On Touch moving
// document.addEventListener("touchmove", e => {
//     [...e.changedTouches].forEach(touch =>{     //for each touch
//         const dot = document.getElementById(touch.identifier);
        
//         //set dot position to center of touchpoint
//         const dotWidth = dot.offsetWidth;
//         const dotHeight = dot.offsetHeight;
//         const centerX = touch.pageX - (dotWidth / 2);
//         const centerY = touch.pageY - (dotHeight / 2);
        
//         dot.style.top = `${centerY}px`;
//         dot.style.left = `${centerX}px`;
//     })
// })

// //Disables default browser events when moving (NO ZOOM, NO REFRESH ON PULL)
// document.addEventListener('touchmove', function(event) {
//     event.preventDefault();
// }, { passive: false }); // can call preventDefault() without causing any performance issues.


// //When letting go of touchscreen
// document.addEventListener("touchend", e => {
//     [...e.changedTouches].forEach(touch =>{     //for each touch
//         const dot = document.getElementById(touch.identifier);
//         dot.remove();
        
//         endPos[0] = touch.pageX;
//         endPos[1] = touch.pageY;
        
//         //console.log(startPos + " startVector");
//         //console.log(endPos + " endVector");
        
//         //Calculate Vector and send as string
//         var vector = VectorCalculation(startPos, endPos);
//         vectorString = (-vector[0]) + "," + vector[1];
//         movementString = ":m:"+vectorString;
//         //sending vector to server
//         websocket.send(selfId + movementString);
//         h2.innerHTML = vectorString;
//     })
// })

// //Calculating Vector of touchstart and touchend
// function VectorCalculation(_startPos, _endPos){
//     //Vector of startVec , endVec
//     var Vector = [ , ];
//     Vector[0] = _endPos[0] - _startPos[0];      //X
//     Vector[1] = _endPos[1] - _startPos[1];      //Y
    
//     return Vector
// }

// //Calculating Length of vector
// function VectorLength(_startPos, _endPos){
//     var Vector = [ , ];
//     Vector[0] = _endPos[0] - _startPos[0];      //X
//     Vector[1] = _endPos[1] - _startPos[1];      //Y
    
//     //Length Calculator
//     return Math.sqrt(square(Vector[0]) + square(Vector[1]));
// }