var charNameElement = document.getElementById("finishScreen").querySelector("h2");
var winnerImg = document.getElementById("finishScreen").querySelector("img");
var restartButton = document.getElementById("finishScreen").querySelector("button");

const controllerElement = document.getElementById("controller");
const finishScreenElement = document.getElementById("finishScreen");
const charSelectElement = document.getElementById("charSelection");

restartButton.addEventListener("click", () => {
    doSend(selfClient.id+":rs");
});

document.addEventListener("playerWon", (onPlayerWon) => {
    controllerElement.classList.add("hidden");
    finishScreenElement.classList.remove("hidden");
    
    let nameOfWinner;
    
    charIdentifiers.forEach(e => {
        if(e.characterInitial === onPlayerWon.detail.playerName)
        {
            nameOfWinner = e.characterName;
            return;
        } 
    });

    charNameElement.innerHTML = nameOfWinner;
    winnerImg.src = "imgs/"+onPlayerWon.detail.playerName+".png";
});