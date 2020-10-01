import Vue from 'vue'
import VueRouter from 'vue-router'
import Articles from '../views/Articles.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Articles',
    component: Articles
  },
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
