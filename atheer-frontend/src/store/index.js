import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex)

const {
    VUE_APP_API = "http://localhost:5000/"
} = process.env;
console.log(process.env.VUE_APP_API);

// Load post utility
const postsUtil = require("../utils/posts.js");
postsUtil.init(VUE_APP_API);

export default new Vuex.Store({
    state: {
        postsUtil: postsUtil
    },
    mutations: {
    },
});