<template>
    <v-main>
        <v-card style="padding:16px;">
            <div>
                <h1 v-if="showContent">
                {{article.title || "This is a sample title"}}
                </h1>
                <h1 v-else>
                    <router-link :to="{name: `ArticleView`, params: getParams}">
                        {{article.title || "This is a sample title"}}
                    </router-link>
                </h1>
            </div>
            

            <v-card-subtitle v-if="dates">
                {{dates}}
            </v-card-subtitle>

            <v-card-text>
                {{description}}
            </v-card-text>

            <div v-if="showContent" v-html="content || `This is a sample content`">
            </div>

            <v-card-text v-if="article.likeable">
                <v-btn text @click="like">Like</v-btn> | {{article.likes}} Likes
                <v-btn v-if="article.shareable" text @click="share">Share</v-btn>
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
            return this.article.description || "This is a sample description";
        },
        content: function (){
            return mdToHTMLConverter.makeHtml(this.article.content);
        },
        dates: function (){
            const creationDate = this.creationDate ? Date(this.creationDate) : undefined;

            if (this.article.lastUpdatedDate){
                const date = Date(this.article.lastUpdatedDate);
                let returnable = `Last updated: ${this.article.lastUpdatedDate}`;
                if (creationDate)
                    returnable = `${returnable} since creation in ${creationDate}`;

                return returnable;
            } 

            return creationDate;
        }
    },
    methods: {
        like: async function (){
            const article = await this.$store.state.postsUtil.like(String(this.article.createdYear), this.article.titleShrinked);
            this.$emit("update:article", article);
        },
        share: async function (){
            console.log("Share button");
            // Share only if native feature supported
            // Enabled on Android maybe IOS
            if (navigator.share){
                try {
                    const shareData = {
                        title: window.document.title,
                        url: window.document.location.href,
                        text: this.article.description
                    };
                    await navigator.share(shareData);
                }
                // Did not share
                 catch (e){
                    console.log("Did not share");
                }
            } // Just link sharing right now
            else 
                await navigator.clipboard.writeText(window.document.location.href);

            const article = await this.$store.state.postsUtil.share(String(this.article.createdYear), this.article.titleShrinked);
            // Update likes only if the article is likeable
            if (this.article.likeable)
                this.$emit("update:article", article);
        }
    }
}
</script>

<style>

</style>