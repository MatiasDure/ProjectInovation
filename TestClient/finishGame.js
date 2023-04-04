var title = document.getElementById("finishScreen").querySelector("h2");


document.addEventListener("playerWon", (onPlayerWon) => {
    document.getElementById("controller").classList.add("hidden");
    document.getElementById("finishScreen").classList.remove("hidden");
    
    title.innerHTML = onPlayerWon.detail.playerName;
});