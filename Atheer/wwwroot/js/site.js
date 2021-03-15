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
    });
    
    const fullUrl = window.location.href;
    navigator.clipboard.writeText(fullUrl).then(() => {
        document.getElementById("shareInfo").innerText = "Successfully copied link to clipboard";
        document.getElementById("share").disabled = true;
    });
};

// FOR LocalDate view component
window.addEventListener("load", (e) => {
    const dateElms = document.querySelectorAll(".x-utc-date");
    dateElms.forEach(elm => {
        const utcDate = elm.dataset.utcdate;
        elm.innerText = (new Date(utcDate)).toLocaleDateString(window.navigator.language);
    });
});