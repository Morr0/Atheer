import Vue from 'vue';
import VueRouter from 'vue-router';
import Meta from "vue-meta";

import Articles from '../views/Articles.vue';
import ArticleView from "../views/ArticleView.vue";

Vue.use(VueRouter);
Vue.use(Meta);

const routes = [
    {
        path: "/article/:year/:titleShrinked",
        name: "ArticleView",
        props: true,
        component: ArticleView
    },
    {
        path: "/article/:year",
        name: "ArticlesByYear",
        component: Articles,
        props: true
    },
    {
        path: "/",
        name: "Articles",
        component: Articles
    },
    {
        path: "*",
        redirect: "/",
        name: "Home"
    }
  ];

  const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
});

export default router;