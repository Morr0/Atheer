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
    props: {
        year: String,
        titleShrinked: String
    },
    data(){
        return {
            article: {}
        };
    },
    mounted(){
        const that = this;
        this.$store.state.postsUtil.posts(this.year, this.titleShrinked)
            .then((data) => that.article = data);
    }
}
</script>

<style>

</style>