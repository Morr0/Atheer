<template>
    <Article :article="article" :showContent="true" />
</template>

<script>
import Article from "@/components/Article.vue";

export default {
    name: "ArticleView",
    components: {
        Article
    },
    data: function (){
        console.log(this.$route);
        this.$store.state.postsUtil.posts(this.$route.params.year, this.$route.params.titleShrinked)
            .then((data) => this.article = data);

        return {
            article: {}
        };
    }
}
</script>

<style>

</style>