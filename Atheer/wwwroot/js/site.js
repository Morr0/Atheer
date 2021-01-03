// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const like = (createdYear, titleShrinked) => {
    const origin = window.location.origin;
    const url = `${origin}/api/article/like?createdYear=${createdYear}&titleShrinked=${titleShrinked}`;
    
    $.ajax({
        type: "POST",
        url: url,
        success: (result) => {
            let likes = Number.parseInt($("#likes")[0].innerText);
            likes++;
            $("#likes")[0].innerText = `${likes.toString()} Likes | `;
        }
    });
};

const share = (createdYear, titleShrinked) => {
    const origin = window.location.origin;
    const url = `${origin}/api/article/share?createdYear=${createdYear}&titleShrinked=${titleShrinked}`;
    $.ajax({
        type: "POST",
        url,
    });
    
    const fullUrl = window.location.href;
    navigator.clipboard.writeText(fullUrl).then(() => {
        $("#shareInfo")[0].innerText = "Successfully copied link to clipboard";
    });
};