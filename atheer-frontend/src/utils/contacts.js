let _endpoint;
let _endpointContacts;

const INITIATED_FROM_CONTACT = "Contact";
const INITIATED_FROM_POST = "Post";

module.exports.init = function(endpoint){
    _endpoint = endpoint;

    _endpointContacts = `${endpoint}api/contact/`;
}

module.exports.contact = async function(title, content, email){
    const body = {
        contacterEmail: email, 
        title: title,
        content: content,
        initiatedFrom: INITIATED_FROM_CONTACT
    };

    const res = await fetch(_endpointContacts, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });
    
    if (res.status !== 200){
        throw Error();
    }
}