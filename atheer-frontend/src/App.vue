<template>
  <v-app>
    <v-app-bar
      app
      light
    >
    <!-- To make the title have a link if was not in main view -->
        <v-toolbar-title @click="toHome" primary-title style="color:#010000;" class="cursor">
            Atheer Blog
        </v-toolbar-title>

      <v-spacer></v-spacer>

      <v-btn
        text
        :to="toContact"
      >
        <span class="mr-2">
            Contact me
        </span>
      </v-btn>
      
    </v-app-bar>
    <v-main>
        <v-container style="max-width:70%;">
            <router-view></router-view>
        </v-container>
        
    </v-main>
  </v-app>
</template>

<script>
import Articles from "@/views/Articles.vue";

export default {
    name: 'App',

    components: {
        Articles
    },
    metaInfo(){
        return {
            title: "Home",
            titleTemplate: `%s | ${this.$store.state.title || "Atheer home"}`,
            content: 'width=device-width, initial-scale=1',
            charset: "utf-8",
            meta: [
                {
                    name: "description",
                    content: this.$store.state.description || "Atheer Home page"
                },
                {
                    property: "og:title",
                    content: this.$store.state.title || "Atheer home"
                },
                {
                    property: "og:description",
                    content: this.$store.state.description || "Atheer Blog"
                },
                // TODO add logo of Atheer here
                // {
                //     property: "og:image",
                //     content: this.$page.post.image || ""
                // }
            ]
        };
    },
    computed: {
        toApps: function (){
            return {
                name: "ArticleView",
                params: {
                    year: "0", 
                    titleShrinked: "apps"
                }
            };
        },
        toContact: function (){
            return {
                name: "ArticleView",
                params: {
                    year: "0", 
                    titleShrinked: "contact"
                }
            };
        }
    },
    methods: {
        toHome: function (){
            if (this.$route.path.length === 1)
                return;

            return this.$router.push({name: "Articles"});
        }
    }
};
</script>

<style>
.no_underline {
    text-decoration: none;
}

.cursor {
    cursor: pointer;
}
</style>
