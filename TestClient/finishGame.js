var charNameElement = document.getElementById("finishScreen").querySelector("h2");
var winnerImg = document.getElementById("finishScreen").querySelector("img");

document.addEventListener("playerWon", (onPlayerWon) => {
    document.getElementById("controller").classList.add("hidden");
    document.getElementById("finishScreen").classList.remove("hidden");
    
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