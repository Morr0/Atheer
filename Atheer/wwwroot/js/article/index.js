const site = window.location.origin;

const like = (createdYear, titleShrinked) => {
    const likesLabelForButton = document.getElementById("likes");

    fetch(`${site}/api/article/like?createdYear=${createdYear}&titleShrinked=${titleShrinked}`, {
        method: "POST"
    }).then((res) => {
        if (res.status === 200){
            let likes = Number.parseInt(likesLabelForButton.innerText);
            likes++;
            likesLabelForButton.innerText = `${likes.toString()} Likes | `;

            document.getElementById("like").disabled = true;
        }
    });
};

const share = (createdYear, titleShrinked) => {
    fetch(`${site}/api/article/share?createdYear=${createdYear}&titleShrinked=${titleShrinked}`, {
        method: "POST"
    }).then();

    const fullUrl = window.location.href;
    navigator.clipboard.writeText(fullUrl).then(() => {
        document.getElementById("shareInfo").innerText = "Successfully copied link to clipboard";
        document.getElementById("share").disabled = true;
    });
};

const halfHeight = window.innerHeight / 5;
const goUpBtn = document.getElementById("go-up-btn");
window.addEventListener("scroll", (event) => {
    goUpBtn.hidden = window.scrollY <= halfHeight;
});

window.scrollBy({
    behavior: "smooth",
    top: 0,
    left: 250
});

goUpBtn.addEventListener("click", () => {
    window.scrollTo({
        top: 0,
        left: window.screenX,
        behavior: "smooth"
    });
});

const jumpableHeaders = document.querySelectorAll(".jumpableHeader");
if (jumpableHeaders.length > 0){
    let string = [`<ul class="list-group flex-grow-0">`];
    for (let header of jumpableHeaders) {
        const name = header.innerText.replace(":", "");
        string.push(`<li class="list-group-item"><a href="#${header.id}">${name}</a></li>`);
    }
    string.push("</ul>");

    const tableOfContentsDiv = document.getElementById("toc").innerHTML = string.join("");
    document.getElementById("tocOuter").hidden = false;
}

(() => {
    const content = document.getElementById("articleContent").textContent;

    const wordsCount = content.replace(/[^w ]/, "").split(/\s+/).length;
    const minsToRead = Math.floor(wordsCount / 240) + 1;
    document.getElementById("wpm").innerHTML = `${minsToRead} minutes to read`;
})();