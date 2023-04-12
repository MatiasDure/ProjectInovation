class SocketClient {
    constructor(id, websocket)
    {
        this.id = id;
        this.websocket = websocket;
    }
}
var selfClient = new SocketClient(-1, null);
var selfId = -1;
var origin = window.location.origin;
var words = origin.split(':'); // typically: words[0]= "http", words[1] = something like "//192.168.0.1", words[2] = "8000" (the http server port)	
var wsUri = "ws:"+words[1];    
var wsPortInlcusion = wsUri+':4444/';
var websocket = new WebSocket(wsPortInlcusion);

async function readFileAndConnect() {
try {
    const response = await fetch('../Assets/path.txt');
    const fileContent = await response.text();
    console.log(fileContent);
} catch (err) {
    console.error('Error reading file:', err);
}
}

readFileAndConnect();

// http://www.websocket.org/echo.html

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
    else if(arr[0] === "sc")
    {
        //arr[1] = id, arr[2] = charSelected name
        img.src = "imgs/"+arr[1]+".png";
    }
    else if(arr[0] === "csr")
    {
        var otherPlayerChar = arr[1];
        const onOtherPlayerSelectedChar = new CustomEvent("otherPlayerSelectedChar", {detail: { charSelected: otherPlayerChar }});
        document.dispatchEvent(onOtherPlayerSelectedChar);
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
}
}, 10); 

//dot & line code -------------------------------------- UI controller
const square = (number) => number ** 2;
var startPos = [ , ];   //start point
var endPos = [ , ];     //end point
var lastValidEndPos = [ , ] //valid end point
var vecLength;          //vector Length of start and end vector
var vectorString = "";

var maxLength = 1000;
var img = new Image();
var isImgLoaded = false;

var catapult = false;

document.addEventListener("switchedScene", (onSwitchedScene) => {
    img.src = "";
    img.onload = function()
    {
        ctx.drawImage(img, 0, 0, window.innerWidth,window.innerHeight);
        DrawText();
        isImgLoaded = true;
    };
});

//On Touching the screen
document.addEventListener("touchstart", e => {
if(document.getElementById("controller").classList.contains("hidden")) return;

[...e.changedTouches].forEach(touch => {        //e.changedTouches is technically a list--> it does not have arry functions --> need to convert it to array first [..]

    const centerX = touch.pageX;
    const centerY = touch.pageY;
    
    startPos[0] = centerX;
    startPos[1] = centerY;
    lastValidEndPos[0] = centerX;
    lastValidEndPos[1] = centerY;
})
})        

var rect = {
    x: 50,
    y: 150,
    width: 200,
    height: 100,
};

//canvas for controller gizmos
const canvas = document.querySelector("#canvas");
canvas.addEventListener("click", function(event){
    let mousePos = GetMousePosInCanvas(canvas, event);
    if (IsInside(mousePos, rect))
    {
        catapult = !catapult;
        
    }
});
var ctx; 
canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

// The rectangle should have x,y,width,height properties


if(canvas.getContext)
{
   ctx = canvas.getContext("2d");
   ClearCanvas();
   //DrawText(); //remove it afterwards------------
} 

function IsInside(mouseClick, rect)
{
    return (mouseClick.x >= rect.x && 
            mouseClick.x <= (rect.x + rect.width) &&
            mouseClick.y >= rect.y && 
            mouseClick.y <= (rect.y + rect.height));
}

function GetMousePosInCanvas(canvas, event)
{
    let canvasBounds = canvas.getBoundingClientRect();

    return {
        x: event.clientX - canvasBounds.left,
        y: event.clientY - canvasBounds.top,
    };
}

function DrawBtn(rec)
{
    ctx.beginPath();
    ctx.rect(rec.x, rec.y, rec.width, rec.height);
    ctx.fillStyle = "rgba(225, 225, 225, .5)";
    ctx.fill();
    ctx.lineWidth = 2;
    ctx.strokeStyle = '#000000';
    ctx.stroke();
    ctx.closePath();
    ctx.font = '40pt Kremlin Pro Web';
    ctx.fillStyle = '#000000';
    ctx.fillText('Toggle', rect.x + rect.width / 4, rect.y + 64);
}

function DrawText(text = "Swipe to move - Drag longer to move farther away")
{
    if(ctx === null) return;
    ctx.font = "40px Arial";
    ctx.fillStyle = "white"; 
    ctx.fillText(text, 20, 100);
}

function DrawLine(begin, end, stroke = "red", width = 5)
{
    if(document.getElementById("controller").classList.contains("hidden")) return;

    if(ctx === null) return;

    //clear canvas
    ClearCanvas();

    // set line stroke and line width
    ctx.strokeStyle = stroke;
    ctx.lineWidth = width;

    console.log("draw: "+begin);
    // draw a red line
    ctx.beginPath();
    ctx.moveTo(...begin);
    ctx.lineTo(...end);
    ctx.stroke();
}

function drawImage()
{
    if(!isImgLoaded) return;
    ctx.drawImage(img, 0, 0, window.innerWidth,window.innerHeight);
}

function drawArrow(ctx, fromx, fromy, tox, toy, arrowWidth, color){
    //variables to be used when creating the arrow
    var headlen = 30;
    var angle = Math.atan2(fromy-toy,fromx-tox);
 
    //clear canvas
    ClearCanvas();

    ctx.save();
    ctx.strokeStyle = color;
 
    //starting path of the arrow from the start square to the end square
    //and drawing the stroke
    ctx.beginPath();
    ctx.moveTo(fromx, fromy);
    ctx.lineTo(tox, toy);
    ctx.lineWidth = arrowWidth;
    ctx.stroke();
 
    //starting a new path from the head of the arrow to one of the sides of
    //the point
    let result1 = angle - Math.PI/7;
    let result2 = angle + Math.PI/7;

    ctx.beginPath();
    ctx.moveTo(fromx, fromy);
    ctx.lineTo(fromx-headlen*Math.cos(result1),
               fromy-headlen*Math.sin(result1));
 
    //path from the side point of the arrow, to the other side point
    ctx.lineTo(fromx-headlen*Math.cos(result2),
               fromy-headlen*Math.sin(result2));
 
    //path from the side point back to the tip of the arrow, and then
    //again to the opposite side point
    ctx.lineTo(fromx, fromy);
    ctx.lineTo(fromx-headlen*Math.cos(result1),
               fromy-headlen*Math.sin(result1));
 
    //draws the paths created above
    ctx.stroke();
    ctx.restore();
}

function ReverseArrow(ctx, fromx, fromy, tox, toy, arrowWidth, color){
    //variables to be used when creating the arrow
    var headlen = 30;
    var angle = Math.atan2(toy-fromy,tox-fromx);
 
    //clear canvas
    ClearCanvas();

    ctx.save();
    ctx.strokeStyle = color;
 
    //starting path of the arrow from the start square to the end square
    //and drawing the stroke
    ctx.beginPath();
    ctx.moveTo(fromx, fromy);
    ctx.lineTo(tox, toy);
    ctx.lineWidth = arrowWidth;
    ctx.stroke();
 
    //starting a new path from the head of the arrow to one of the sides of
    //the point
    let result1 = angle - Math.PI/7;
    let result2 = angle + Math.PI/7;

    ctx.beginPath();
    ctx.moveTo(tox, toy);
    ctx.lineTo(tox-headlen*Math.cos(result1),
               toy-headlen*Math.sin(result1));
 
    //path from the side point of the arrow, to the other side point
    ctx.lineTo(tox-headlen*Math.cos(result2),
               toy-headlen*Math.sin(result2));
 
    //path from the side point back to the tip of the arrow, and then
    //again to the opposite side point
    ctx.lineTo(tox, toy);
    ctx.lineTo(tox-headlen*Math.cos(result1),
               toy-headlen*Math.sin(result1));
 
    //draws the paths created above
    ctx.stroke();
    ctx.restore();
}

function ClearCanvas()
{
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    drawImage();
    //DrawText();
    DrawBtn(rect);
}

//On Touch moving
document.addEventListener("touchmove", e => {
    if(document.getElementById("controller").classList.contains("hidden")) return;
[...e.changedTouches].forEach(touch =>{     //for each touch

    const centerX = touch.pageX;
    const centerY = touch.pageY;

    let diffVec = SubVector(startPos,[centerX,centerY]);
    
    if(VectorLength(diffVec) > maxLength)
    {
        let unitVec = UnitVector(diffVec);
        let scaledVec = ScaleVector(unitVec, maxLength);

        let newEndVec = AddVector(startPos,scaledVec);
        
        if (catapult) drawArrow(ctx,startPos[0],startPos[1],newEndVec[0],newEndVec[1],13,"red");
        else ReverseArrow(ctx,startPos[0],startPos[1],newEndVec[0],newEndVec[1],13,"red"); 

        lastValidEndPos = newEndVec;
        return;
    } 

    if (catapult) drawArrow(ctx,startPos[0],startPos[1],centerX,centerY,13,"red");
    else ReverseArrow(ctx,startPos[0],startPos[1],centerX,centerY,13,"red");  

    lastValidEndPos[0] = centerX;
    lastValidEndPos[1] = centerY;
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

    //Clearing the arrow
    ClearCanvas();
    
    //Calculate Vector and send as string
    //var vector = VectorCalculation(startPos, lastValidEndPos); <------------ original implementation
    var vector = catapult ? VectorCalculation(startPos, lastValidEndPos) : VectorCalculation(lastValidEndPos, startPos);
    vectorString = (-vector[0]) + "," + vector[1];
    DrawText(vectorString);
    movementString = ":m:"+vectorString;
    websocket.send(selfId + movementString);
})
})

//vector math---------------------------------------

//Calculating Vector of touchstart and touchend
function VectorCalculation(_startPos, _endPos){
    //Vector of startVec , endVec
    var Vector = [ , ];
    Vector[0] = _endPos[0] - _startPos[0];      //X
    Vector[1] = _endPos[1] - _startPos[1];      //Y
    
    return Vector
}

function UnitVector(_startPos, _endPos)
{
    let directionVec = SubVector(_startPos,_endPos);
    let directionVecLength  = VectorLength(_startPos,_endPos);
    let unitVec = [directionVec[0]/directionVecLength, directionVec[1]/directionVecLength]; 
    return unitVec;
}

function UnitVector(_vec)
{
    let directionVecLength  = VectorLength(_vec);
    let unitVec = [_vec[0]/directionVecLength, _vec[1]/directionVecLength]; 
    return unitVec;
}


function SubVector(_startPos, _endPos)
{
    return [_endPos[0] - _startPos[0], _endPos[1] - _startPos[1]];
}

function AddVector(_startPos, _endPos)
{
    return [_startPos[0] + _endPos[0], _startPos[1] + _endPos[1]];
}

function ScaleVector(vec, scalar)
{
    return [vec[0]*scalar, vec[1]*scalar];
}

//Calculating Length of vector
function VectorLength(_startPos, _endPos){
    var Vector = [ , ];
    Vector[0] = _endPos[0] - _startPos[0];      //X
    Vector[1] = _endPos[1] - _startPos[1];      //Y
    
    //Length Calculator
    return Math.sqrt(square(Vector[0]) + square(Vector[1]));
}

function VectorLength(_vec){
    //Length Calculator
    return Math.sqrt(square(_vec[0]) + square(_vec[1]));
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