<template>
    <Article :article="article" :showContent="true" />
</template>

<script>
import Article from "@/components/Article.vue";

export default {
    components: {
        Article
    },
    data: function (){
        console.log(this.$route);

        return {
            article: []
        };
    }
}
</script>

<style>

</style>