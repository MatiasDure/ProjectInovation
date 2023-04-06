var btns = document.querySelectorAll("button");
var readyBtn = document.getElementById("ready");
var btnsObjs = [];
var selectedBtn = null;
var idHtml = document.querySelector("h1.id");
var charImg = document.querySelector("img.character");

//assign the ready button function
readyBtn.addEventListener("click", () =>
{
    if(selectedBtn == null) return;
    
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
        id: "charA",
        button: null,
        action: function(btn)
        {
            //doSend(selfClient.id+":cs:charA");
            DeselectBtn();
            selectBtn(btn);
        }
    },
    {
        id: "charB",
        button: null,
        action: function(btn)
        {
            // doSend(selfClient.id+":cs:charB");
            DeselectBtn();
            selectBtn(btn);
        }
    },
    {
        id: "charC",
        button: null,
        action: function(btn)
        {
            // doSend(selfClient.id+":cs:charC");
            DeselectBtn();
            selectBtn(btn);
        }
    },
    {
        id: "charD",
        button: null,
        action: function(btn)
        {
            // doSend(selfClient.id+":cs:charD");
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
        charBtn.action(charBtn.button);
    });   
});


//removes selected class from button previously pressed
function DeselectBtn()
{
    if (selectedBtn == null) return;
    
    selectedBtn.classList.remove("selectedBtn");

    selectedBtn = null;
}

//add selected class to button pressed
function selectBtn(btnToSelect)
{
    console.log(btnToSelect);
    if (btnToSelect == null) return;
    
    selectedBtn = btnToSelect;
    btnToSelect.classList.add("selectedBtn");
}

//adding custom event listeners to the document object -------------------- 
document.addEventListener("charAccepted", (onCharAccepted) => {
    charImg.src = "imgs/"+onCharAccepted.detail.character+".png";
    charImg.classList.remove("hide");
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

