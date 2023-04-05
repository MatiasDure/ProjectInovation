var charBtns = [
    {
        id: "charA",
        action: function()
        {
            doSend(selfClient.id+":cs:charA")
        }
    },
    {
        id: "charB",
        action: function()
        {
            doSend(selfClient.id+":cs:charB")
        }
    },
    {
        id: "charC",
        action: function()
        {
            doSend(selfClient.id+":cs:charC")
        }
    },
    {
        id: "charD",
        action: function()
        {
            doSend(selfClient.id+":cs:charD")
        }
    },
]

var idHtml = document.querySelector("h1.id");
var charImg = document.querySelector("img.character");

document.addEventListener("charAccepted", (onCharAccepted) => {
    charImg.src = "imgs/"+onCharAccepted.detail.character+".png";
});

document.addEventListener("clientConnected", (onNewClientConnected) => 
{
    console.log("client connected from char selection: " + onNewClientConnected.detail.amountConnected);
    if(onNewClientConnected.detail.amountConnected >= 1) 
    {
        document.getElementById("controller").classList.remove("hidden");
        document.getElementById("charSelection").classList.add("hidden");
        doSend(selfClient.id+":ss");
        //should confirm scene switch!!!
        //for now add img here
        const onSwitchedScene = new CustomEvent("switchedScene");
        document.dispatchEvent(onSwitchedScene);
    }
})

document.addEventListener("idProvided", (onIdProvided) => {
    idHtml.innerHTML = onIdProvided.detail.idProvided;
    console.log(selfClient.id);
});

var btns = document.querySelectorAll("button");
var btnsObjs = [];

btns.forEach( e => 
    {
        charBtns.forEach( charBtn => 
        {
            if(e.className === charBtn.id)
            {
                e.addEventListener("click", charBtn.action);
            }
        })
    });
