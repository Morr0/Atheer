<template>
    <v-main>
        <v-card>
            <v-card-title primary-title>
                <div v-if="showContent">
                    {{article.title || "This is a sample title"}}
                </div>
                <div v-else>
                    <router-link :to="{name: `Article`, params: getParams}">
                        {{article.title || "This is a sample title"}}
                    </router-link>
                </div>
            </v-card-title>
            <v-card-text>
                <v-main>
                    {{article.description || "This is a sample description"}}
                </v-main>
                
                <v-main v-if="showContent">
                    {{article.content || "This is a sample content"}}
                </v-main>
            </v-card-text>
        </v-card>
    </v-main>
</template>

<script>
export default {
    props: {
        article: {},
        showContent: Boolean
    },
    computed: {
        getParams: function (){
            return {
                year: this.article.createdYear, 
                titleShrinked: this.article.titleShrinked
            };
        }
    }
}
</script>

<style>

</style>