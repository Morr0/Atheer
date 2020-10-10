<template>
    <v-main>
        <v-card>
            <v-card-title primary-title>
                <div v-if="showContent">
                    {{article.title || "This is a sample title"}}
                </div>
                <div v-else>
                    <router-link :to="{name: `ArticleView`, params: getParams}">
                        {{article.title || "This is a sample title"}}
                    </router-link>
                </div>
            </v-card-title>
            <v-card-text>
                <v-main v-html="description || `This is a sample description`">
                </v-main>
                
                <v-main v-if="showContent" v-html="content || `This is a sample content`">
                </v-main>
            </v-card-text>

            <v-card-text>
                <v-btn text @click="like">Like</v-btn>
            </v-card-text>
        </v-card>
    </v-main>
</template>

<script>
const Converter = require("showdown").Converter;
const mdToHTMLConverter = new Converter();

export default {
    props: {
        article: {},
        showContent: Boolean
    },
    computed: {
        getParams: function (){
            return {
                year: String(this.article.createdYear), 
                titleShrinked: this.article.titleShrinked
            };
        },
        description: function (){
            return mdToHTMLConverter.makeHtml(this.article.description);
        },
        content: function (){
            return mdToHTMLConverter.makeHtml(this.article.content);
        }
    },
    methods: {
        like: async function (){
            const article = await this.$store.state.postsUtil.like(String(this.article.createdYear), this.article.titleShrinked);
            this.$emit("update:article", article);
        }
    }
}
</script>

<style>

</style>