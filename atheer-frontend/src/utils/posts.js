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
    const result = await res.json();
    let returnable = result;

    // If empty array and looking for one item, return {}
    // If empty array and looking for one item, return the first
    if (result.constructor === Array)
        if (year && titleShrinked)
            if (result.length == 0)
                return {};
            else if (result.length === 1)
                returnable = result[0];

    return returnable;
}

module.exports.like = async function (year, titleShrinked){
    const endpoint = `${_endpoint}api/blog/like/${year}/${titleShrinked}`;
    const res = await fetch(endpoint, {
        method: "POST"
    });

    return res.status === 400? undefined : await res.json();
}