import Vue from 'vue';
import VueRouter from 'vue-router';

import Articles from '../views/Articles.vue';
import ArticleView from "../views/ArticleView.vue";

Vue.use(VueRouter)

const routes = [
    {
        path: "/article/:year/:titleShrinked",
        name: "ArticleView",
        props: true,
        component: ArticleView
    },
    {
        path: "/",
        name: "Articles",
        component: Articles
    },
    {
        path: "*",
        redirect: "/"
    }
  ];

  const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
});

export default router;