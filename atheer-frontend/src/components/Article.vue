<template>
    <v-main>
        <v-card>
            <v-card-title primary-title>
                {{article.title || "This is a sample title"}}
            </v-card-title>
            <v-card-text>
                <v-content>
                    {{article.description || "This is a sample description"}}
                </v-content>

                <v-content>
                    {{article.content || "This is a sample content"}}
                </v-content>
            </v-card-text>
        </v-card>
    </v-main>
</template>

<script>
export default {
    props: {
        article: {}
    }
}
</script>

<style>

</style>