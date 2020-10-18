<template>
    <v-main>
        <v-card style="padding:16px;">
            <div>
                <h1 v-if="showContent" style="color:#010000;">
                {{article.title || "This is a sample title"}}
                </h1>
                <h1 v-else>
                    <router-link class="no_underline" style="color:#010000;" :to="{name: `ArticleView`, params: getParams}">
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

            <div v-if="showContent" class="article" v-html="content || `This is a sample content`">
            </div>

            <v-card-text>
                <span v-if="article.likeable">
                    <v-btn text @click="like">Like</v-btn> | {{article.likes}} Likes
                </span>
                <v-btn v-if="article.shareable" text @click="share">Share</v-btn>
            </v-card-text>
        </v-card>

        <v-snackbar v-model="showShareSnackbar">
            Link copied.
        </v-snackbar>
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
    data(){
        return {
            showShareSnackbar: false,
        };
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
            const co = mdToHTMLConverter.makeHtml(this.article.content);
            console.log(co);
            return co;
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

            this.showShareSnackbar = true;

            const article = await this.$store.state.postsUtil.share(String(this.article.createdYear), this.article.titleShrinked);
            // Update likes only if the article is likeable
            if (this.article.likeable)
                this.$emit("update:article", article);
        }
    }
}
</script>

<style>
.article img {
    max-width: 100%;
    max-height: 56.2%;
}

.article p code {
    background: black;
    color: white;
}

.article pre code {
    background: rgba(0, 0, 0, 0);
    color: white;
}

.article pre {
    background: black;
    max-width: 100%;
    overflow-x: scroll;
    padding: 8px;
}
</style>