var btns = document.querySelectorAll("button");
var readyBtn = document.getElementById("ready");
var btnsObjs = [];
var selectedBtn = null;
var idHtml = document.querySelector("h1.id");
var charImg = document.querySelector("img.character");
var isReady = false;

//assign the ready button function
readyBtn.addEventListener("click", () =>
{
    if(selectedBtn == null ||
         isReady || 
         readyBtn.classList.contains("unavailableBtn")) return;
    
    isReady = true;
    readyBtn.classList.remove("availableBtn");
    readyBtn.classList.add("unavailableBtn");
    doSend(selfClient.id + ":cs:"+selectedBtn.id);
});

function FindBtnWithClassName(className)
{
    matchBtn = null;
    btns.forEach( e => {
        if(e.id === className)
        {
           matchBtn = e;
        }
    });
    return matchBtn;
}

var charBtns = [
    {
        available: true,
        id: "charA",
        button: null,
        action: function(btn)
        {
            if(!btn.available || isReady) return;
            DeselectBtn();
            selectBtn(btn);
        }
    },
    {
        available: true,
        id: "charB",
        button: null,
        action: function(btn)
        {
            if(!btn.available || isReady) return;
            DeselectBtn();
            selectBtn(btn);
        }
    },
    {
        available: true,
        id: "charC",
        button: null,
        action: function(btn)
        {
            if(!btn.available || isReady) return;
            DeselectBtn();
            selectBtn(btn);
        }
    },
    {
        available: true,
        id: "charD",
        button: null,
        action: function(btn)
        {
            if(!btn.available || isReady) return;
            DeselectBtn();
            selectBtn(btn);
        }
    },
]


//assign html buttons to charBtns objects
charBtns.forEach(e => 
{
    e.button = FindBtnWithClassName(e.id);
})

//assign the charBtns actions to each charBtn html button
charBtns.forEach( charBtn => 
{
    charBtn.button.addEventListener("click", () => {
        charBtn.action(charBtn);
    });   
});

//removes selected class from button previously pressed
function DeselectBtn()
{
    if (selectedBtn == null) return;
    
    selectedBtn.button.classList.remove("selectedBtn");

    selectedBtn = null;
}

//add selected class to button pressed
function selectBtn(btnToSelect)
{
    console.log(btnToSelect);
    if (btnToSelect == null) return;
    
    selectedBtn = btnToSelect;
    btnToSelect.button.classList.add("selectedBtn");
    readyBtn.classList.remove("unavailableBtn");
    readyBtn.classList.add("availableBtn");
}

function FindCharBtnWithId(idName)
{
    let matchCharBtn = null;
    charBtns.forEach(e => {
        if(e.id === idName)
        {
            matchCharBtn = e;
            return;
        }
    })
    return matchCharBtn;
}

//adding custom event listeners to the document object -------------------- 
document.addEventListener("charAccepted", (onCharAccepted) => {
    charImg.src = "imgs/"+onCharAccepted.detail.character+".png";
    charImg.classList.remove("hide");
});

document.addEventListener("otherPlayerSelectedChar", (onOtherPlayerSelectedChar) => {
    let charBtn = FindCharBtnWithId(onOtherPlayerSelectedChar.detail.charSelected);
    charBtn.button.classList.add("unavailableBtn");
    charBtn.available = false;
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

