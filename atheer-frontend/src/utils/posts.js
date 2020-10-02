let _endpoint = undefined;

let _endpointArticles = undefined;

module.exports.init = async function (endpoint){
    _endpoint = endpoint;

    _endpointArticles = `${endpoint}api/blog/`;
}

module.exports.posts = async function (year = undefined, titleShrinked = undefined){
    let endpoint = _endpointArticles;

    if (year){
        endpoint = `${endpoint}${year}`;
    
        if (titleShrinked)
            endpoint = `${endpoint}/${titleShrinked}`;
    }

    const res = await fetch(endpoint);
    return await res.json();
}