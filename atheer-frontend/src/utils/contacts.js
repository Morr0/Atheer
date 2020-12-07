let _endpoint;
let _endpointContacts;

const INITIATED_FROM_CONTACT = "Contact";
const INITIATED_FROM_POST = "Post";

module.exports.init = function(endpoint){
    _endpoint = endpoint;

    _endpointContacts = `${endpoint}api/contact/`;
}

module.exports.contact = async function(title, content, email){
    let payload = {
        initiatedFrom: INITIATED_FROM_CONTACT
    };

    await contact(title, content, email, payload);
}

module.exports.contactForPost = async function(postYear, postTitleShrinked, title, content, email){
    let payload = {
        initiatedFrom: INITIATED_FROM_POST,
        postCreatedYear: Number(postYear),
        postTitleShrinked: postTitleShrinked
    };

    await contact(title, content, email, payload);
}

async function contact(title, content, email, payload){
    payload.contacterEmail = email; 
    payload.title = title;
    payload.content = content;

    const res = await fetch(_endpointContacts, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload),
        mode: "no-cors"
    });
    
    if (res.status !== 200){
        throw Error();
    }
}