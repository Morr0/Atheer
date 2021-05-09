window.addEventListener("load", (e) => {
    const dateElms = document.querySelectorAll(".x-utc-date");
    dateElms.forEach(elm => {
        const utcDate = elm.dataset.utcdate;
        elm.innerText = (new Date(utcDate)).toLocaleDateString(window.navigator.language);
    });
});