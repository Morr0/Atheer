<template>
    <div>
        <Post v-for="post in posts" :key="post.titleShrinked" :post="post" />
    </div>
</template>

<script>
import Post from "@/components/Post.vue";

export default {
    components: {
        Post
    },
    data() {
        return {
            posts: [
                {
                    titleShrinked: "ttt.tt",
                    title: "Title here TTT TT"
                },
                {
                    titleShrinked: "tbbb",
                    title: "Title here TBBB"
                },
            ]
        };
    }
}
</script>

<style>

</style>