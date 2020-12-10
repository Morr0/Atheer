<template>
    <v-list-item>
        <router-link :to="to">
            <v-list-item-title v-text="article.title"></v-list-item-title>
        </router-link>
            <br />
            {{`created: ${Date(this.article.lastUpdatedDate)}, Draft: ${article.draft}, Unlisted: ${article.unlisted}, Scheduled: ${article.scheduled}`}}
    </v-list-item>
</template>

<script>
export default {
    props: {
        article: Object
    },
    computed: {
        to: function(){
            return {
                name: "ArticleEdit",
                query: {
                    createdYear: this.article.createdYear,
                    titleShrinked: this.article.titleShrinked
                }
            };
        }
    }
}
</script>