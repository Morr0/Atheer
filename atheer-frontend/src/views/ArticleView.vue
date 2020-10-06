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
    props: {
        year: String,
        titleShrinked: String
    },
    computed: {
        article: async function (){
            return await this.$store.state.postsUtil.posts(this.year, this.titleShrinked);
        }
    },
    mounted(){
        
    }
}
</script>

<style>

</style>