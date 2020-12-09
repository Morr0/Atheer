let _endpoint = undefined;

let _endpointArticles = undefined;

module.exports.init = async function (endpoint){
    _endpoint = endpoint;

    _endpointArticles = `${endpoint}api/blog/`;
}

module.exports.post = async function (year, titleShrinked){
    const endpoint = `${_endpointArticles}${year}/${titleShrinked}`;
    const res = await fetch(endpoint);
    return res.status === 200 ? await res.json() : undefined;
}

module.exports.barePosts = async function (){
    const endpoint = _endpointArticles;

    const res = await fetch(endpoint);
    return res.status === 200 ? await res.json() : undefined;
}

module.exports.posts = async function (year = undefined){
    let endpoint = _endpointArticles;
    if (year) 
        endpoint = `${endpoint}${year}`;

    const res = await fetch(endpoint);
    return res.status === 200 ? await res.json() : undefined;
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