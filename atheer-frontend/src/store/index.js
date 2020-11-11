import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex)

let {
    VUE_APP_TITLE = "Atheer",
    VUE_APP_DESCRIPTION = "Atheer Blog",
    VUE_APP_API = "http://localhost:5000/"
} = process.env;

// Validations
{
    // Add slash at the end of api string if it doesn't have
    if (!VUE_APP_API.endsWith('/'))
        VUE_APP_API = `${VUE_APP_API}/`;
}

console.log(VUE_APP_TITLE);
console.log(VUE_APP_API);

// Load post utility
const postsUtil = require("../utils/posts.js");
postsUtil.init(VUE_APP_API);

const contactsUtil = require("../utils/contacts.js");
contactsUtil.init(VUE_APP_API);

export default new Vuex.Store({
    state: {
        postsUtil: postsUtil,
        contactsUtil: contactsUtil,
        title: VUE_APP_TITLE,
        description: VUE_APP_DESCRIPTION
    },
    mutations: {
    },
});