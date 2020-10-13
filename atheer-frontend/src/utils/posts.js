let _endpoint = undefined;

let _endpointArticles = undefined;

module.exports.init = async function (endpoint){
    _endpoint = endpoint;

    _endpointArticles = `${endpoint}api/blog/`;
}

// PAGINATION trackers
// Constants
const X_AthBlog_Last_Year = "x_AthBlog_Last_Year";
const X_AthBlog_Last_Title = "x_AthBlog_Last_Title";
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

    const res = await fetch(endpoint);
    if (res.status === 404)
        return undefined;

    const data = await res.json();
    let returnable = data;

    if (data.constructor === Array)
        takeCareOfPaginationStuffIfNeeded(year, res, data);

    // If empty array and looking for one item, return {}
    // If empty array and looking for one item, return the first
    if (data.constructor === Array)
        if (year && titleShrinked)
            if (data.length == 0)
                return {};
            else if (data.length === 1)
                returnable = data[0];

    return returnable;
}

function takeCareOfPaginationStuffIfNeeded(year, res, data){
    const pageLastYear = res.headers.get(X_AthBlog_Last_Year);
    const pageLastTitle = res.headers.get(X_AthBlog_Last_Title);

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

        const res = await fetch(endpoint, {
            headers: {
                X_AthBlog_Last_Year: _pageApiLastYear,
                X_AthBlog_Last_Title: _pageApiLastTitle
            }
        });
        const data = await res.json();

        takeCareOfPaginationStuffIfNeeded(_pageYear, res, data);
        return data;
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