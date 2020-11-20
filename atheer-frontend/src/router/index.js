import Vue from 'vue';
import VueRouter from 'vue-router';
import Meta from "vue-meta";

import Articles from '../views/Articles.vue';
import ArticleView from "../views/ArticleView.vue";
import Placeholder from "../views/Placeholder.vue";
import ContactView from "../views/ContactView.vue";
import AdminView from "../views/AdminView.vue";
import ArticleEdit from "../components/ArticleEdit.vue";

Vue.use(VueRouter);
Vue.use(Meta);

const routes = [
    {
        path: "/admin",
        name: "Admin",
        component: AdminView
    },
    {
        path: "/admin/article",
        name: "AdminArticle",
        component: ArticleEdit,
        props: true
    },
    {
        path: "/article/:year/:titleShrinked",
        name: "ArticleView",
        props: true,
        component: ArticleView
    },
    {
        path: "/articles/0",
        redirect: "/"
    },
    {
        path: "/article/0",
        redirect: "/"
    },
    {
        // BACKWARD COMPATIBILITY
        path: "/article/:year",
        component: Articles,
        props: true
    },
    {
        path: "/articles/:year",
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
        path: "/redirector",
        name: "Placeholder",
        component: Placeholder
    },
    {
        path: "/contact",
        name: "Contact",
        component: ContactView
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