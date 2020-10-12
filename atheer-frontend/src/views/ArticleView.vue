<template>
    <Article v-if="article" :article="article" :showContent="true" />
</template>

<script>
import Article from "@/components/Article.vue";

export default {
    name: "ArticleView",
    components: {
        Article
    },
    metaInfo(){
        return {
            title: this.article.title || "Untitled",
            description: this.article.description
        }
    },
    props: {
        year: String,
        titleShrinked: String
    },
    data(){
        return {
            article: {}
        };
    },
    watch: {
        // Watching the URL path changes to render the new post once it happens
        '$route.params': {
            handler: function(params) {
                const that = this;
                this.$store.state.postsUtil.posts(this.year, this.titleShrinked)
                    .then((data) => that.article = data);
            },
            deep: true,
            immediate: true
      }
    }

}
</script>

<style>

</style>