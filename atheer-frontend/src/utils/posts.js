let _endpoint = undefined;

let _endpointArticles = undefined;

module.exports.init = async function (endpoint){
    _endpoint = endpoint;

    _endpointArticles = `${endpoint}api/blog/`;
}

// PAGINATION trackers
// Constants
const x_athblog_last_year = "x_AthBlog_Last_Year";
const x_athblog_last_title = "x_AthBlog_Last_Title";
// For when loading multiple posts only
let _pageYear = undefined;
let _pagePostsThusFar = [];
let _pageApiLastYear = null;
let _pageApiLastTitle = null;

module.exports.posts = async function (year = undefined, titleShrinked = undefined){
    let endpoint = _endpointArticles;

    if (year){
        endpoint = `${endpoint}${year}`;
    
        if (titleShrinked)
            endpoint = `${endpoint}/${titleShrinked}`;
    }

    endpoint = `${endpoint}?size=1`;

    const res = await fetch(endpoint);
    if (res.status === 404)
        return undefined;

    const data = await res.json();
    const posts = data.posts;
    let returnable = posts;

    if (posts.constructor === Array)
        takeCareOfPaginationStuffIfNeeded(year, res, data);

    // If empty array and looking for one item, return {}
    // If empty array and looking for one item, return the first
    if (posts.constructor === Array)
        if (year && titleShrinked)
            if (posts.length == 0)
                return {};
            else if (posts.length === 1)
                returnable = data[0];

    return returnable;
}

function takeCareOfPaginationStuffIfNeeded(year, res, data){
    console.log("X1");
    const pageLastYear = data.x_AthBlog_Last_Year;
    const pageLastTitle = data.x_AthBlog_Last_Title;
    console.log(pageLastYear);
    console.log(pageLastTitle);
    window.res = res.headers;

    if (pageLastYear && pageLastTitle){
        console.log("X2");
        _pageYear = year;
        _pagePostsThusFar = _pagePostsThusFar.concat(data);
        _pageApiLastYear = pageLastYear;
        _pageApiLastTitle = pageLastTitle;
    } else {
        console.log("X22");
        _pageYear = undefined;
        _pagePostsThusFar = [];
        _pageApiLastYear = null;
        _pageApiLastTitle = null;
    }
}

// Won't do anything unless there is more posts to be loaded
module.exports.morePosts = async function (){
    console.log("X3");
    if (_pageApiLastYear && _pageApiLastTitle){
        console.log("X4");
        let endpoint = _endpointArticles;
        if (_pageYear)
            endpoint = `${endpoint}${_pageYear}`;
        

        const res = await fetch(endpoint, {
            headers: {
                X_AthBlog_Last_Year: _pageApiLastYear,
                X_AthBlog_Last_Title: _pageApiLastTitle,
            }
        });
        const data = await res.json();

        takeCareOfPaginationStuffIfNeeded(_pageYear, res, data);
        return data.posts;
    }

    return undefined;
}

module.exports.like = async function (year, titleShrinked){
    const endpoint = `${_endpoint}api/blog/like/${year}/${titleShrinked}`;
    const res = await fetch(endpoint, {
        method: "POST"
    });

    return res.status === 400? undefined : await res.json();
}

module.exports.share = async function (year, titleShrinked){
    const endpoint = `${_endpoint}api/blog/share/${year}/${titleShrinked}`;
    const res = await fetch(endpoint, {
        method: "POST"
    });

    return res.status === 400? undefined : await res.json();
}