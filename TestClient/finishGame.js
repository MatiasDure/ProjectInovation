var title = document.getElementById("finishScreen").querySelector("h2");
var winnerImg = document.getElementById("finishScreen").querySelector("img");

document.addEventListener("playerWon", (onPlayerWon) => {
    document.getElementById("controller").classList.add("hidden");
    document.getElementById("finishScreen").classList.remove("hidden");
    
    title.innerHTML = onPlayerWon.detail.playerName + " won!";
    winnerImg.src = "imgs/"+onPlayerWon.detail.playerName+".png";
});