import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

// Load post utility
const postsUtil = require("../utils/posts.js");
postsUtil.init("https://localhost:5001/");

export default new Vuex.Store({
  state: {
      postsUtil: postsUtil
  },
  mutations: {
  },
  actions: {
  },
  modules: {
  }
})
