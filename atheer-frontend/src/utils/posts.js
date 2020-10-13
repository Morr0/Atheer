let _endpoint = undefined;

let _endpointArticles = undefined;

module.exports.init = async function (endpoint){
    _endpoint = endpoint;

    _endpointArticles = `${endpoint}api/blog/`;
}

// PAGINATION BEGIN
let _pageSize = 10;
// For when loading multiple posts only
let _pageYear = undefined;
let _pagePostsThusFar = [];
let _pageApiLastYear = null;
let _pageApiLastTitle = null;
// PAGINATION END

module.exports.post = async function (year, titleShrinked){
    const endpoint = `${_endpointArticles}${year}/${titleShrinked}`;
    const res = await fetch(endpoint);
    return res.status === 404 ? undefined : await res.json();
}

module.exports.posts = async function (year = undefined, titleShrinked = undefined){
    let endpoint = _endpointArticles;

    if (year)
        endpoint = `${endpoint}${year}`;
    endpoint = `${endpoint}?size=${_pageSize}`;

    const res = await fetch(endpoint);
    if (res.status === 404)
        return undefined;

    const data = await res.json();
    takeCareOfPaginationStuffIfNeeded(year, data);
    return data.posts;
}

function takeCareOfPaginationStuffIfNeeded(year, data){
    const pageLastYear = data.x_AthBlog_Last_Year;
    const pageLastTitle = data.x_AthBlog_Last_Title;

    if (pageLastYear && pageLastTitle){
        _pageYear = year;
        _pagePostsThusFar = _pagePostsThusFar.concat(data);
        _pageApiLastYear = pageLastYear;
        _pageApiLastTitle = pageLastTitle;
    } else {
        _pageYear = undefined;
        _pagePostsThusFar = [];
        _pageApiLastYear = null;
        _pageApiLastTitle = null;
    }
}

// Won't do anything unless there is more posts to be loaded
module.exports.morePosts = async function (){
    if (_pageApiLastYear && _pageApiLastTitle){
        let endpoint = _endpointArticles;
        if (_pageYear)
            endpoint = `${endpoint}${_pageYear}`;
        endpoint = `${endpoint}?size=${_pageSize}`;

        const res = await fetch(endpoint, {
            headers: {
                X_AthBlog_Last_Year: _pageApiLastYear,
                X_AthBlog_Last_Title: _pageApiLastTitle,
            }
        });
        const data = await res.json();

        takeCareOfPaginationStuffIfNeeded(_pageYear, data);
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