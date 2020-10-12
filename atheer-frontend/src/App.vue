<template>
  <v-app>
    <v-app-bar
      app
      light
    >
    <div class="d-flex align-center">
        <!-- <v-img
            alt="Vuetify Logo"
            class="shrink mr-2"
            contain
            src="https://cdn.vuetifyjs.com/images/logos/vuetify-logo-dark.png"
            transition="scale-transition"
            width="40"
        /> -->
    </div>


    <!-- To make the title have a link if was not in main view -->
        <v-card-title v-if="$route.path.length === 1" primary-title style="color:#010000;">
            Atheer Blog
        </v-card-title>
        <v-card-title v-else primary-title>
            <router-link class="no_underline" style="color:#010000;" :to="{name: `Articles`}">
                Atheer Blog
            </router-link>
        </v-card-title>


      <v-spacer></v-spacer>

      <v-btn
        text
        :to="toApps"
      >
        <span class="mr-2">
            Apps
        </span>
      </v-btn>

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
        <!-- <Articles /> -->
        <v-container grid-list-xs>
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
    metaInfo: {
        title: "Atheer",
        htmlAttrs: {
            lang: "en",
            amp: true
        }
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
    }
};
</script>

<style>
.no_underline {
    text-decoration: none;
}
</style>
