<template>
    <div>
        <v-form>
            <v-row>
                <v-text-field 
                    label="Created Year"
                    v-model="article.createdYear"
                ></v-text-field>
                <v-spacer />
                <v-text-field
                    label="Title Shrinked"
                    v-model="article.titleShrinked"
                ></v-text-field>
            </v-row>

            <v-text-field
                label="Title"
                v-model="article.title"
            ></v-text-field>
            <v-text-field
                label="Description"
                v-model="article.description"
            ></v-text-field>
            <v-text-field
                label="Topics (CSV or SPV)"
                v-model="article.topics"
            ></v-text-field>
            <v-textarea
                label="Content (MD)"
                v-model="article.content"
            ></v-textarea>


            <v-row>
                Properties:           
                <v-switch label="Likeable" v-model="article.likeable"></v-switch>
                <v-spacer />
                <v-switch label="Shareable" v-model="article.shareable"></v-switch>
                <v-spacer />
                <v-switch label="Contactable" v-model="article.contactable"></v-switch>
            </v-row>


            <v-row>
                Publishability:
                <v-switch label="Draft" v-model="article.draft" color="red"></v-switch>
                <v-spacer />
                <v-switch label="Unlisted" v-model="article.unlisted" color="red"></v-switch>
            </v-row>

            <v-btn @click.prevent="checkout">Checkout</v-btn>
        </v-form>
    </div>
</template>

<script>
export default {
    name: "ArticleEdit",
    props: {
        operation: String
    },
    data(){
        return {
            article: {}
        };
    },
    methods: {
        checkout: async function(){
            this.validate();
        },
        validate: function(){

        }
    },
}
</script>