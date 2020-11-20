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
                label="Topics (CSV)"
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
const MODE_NEW_ARTICLE = "new";
const MODE_EDIT_ARTICLE = "edit";

export default {
    name: "ArticleEdit",
    props: {
        operation: String,
    },
    data(){
        return {
            createdYear: this.$route.query.createdYear,
            titleShrinked: this.$route.query.titleShrinked,
            mode: MODE_NEW_ARTICLE,
            article: {}
        };
    },
    methods: {
        checkout: async function(){
            this.validate();
        },
        validate: function(){

        },


        idChange: function (){
            // TODO handle changes
        }
    },
    watch: {
        // Watching the URL path changes to render the new post once it happens
        '$route.query': {
            handler: function(params) {
                const that = this;
                this.$store.state.postsUtil.post(this.createdYear, this.titleShrinked)
                    .then((data) => {
                        that.article = MODE_EDIT_ARTICLE;
                        return that.article = data;
                    })
                    .then((data) => {
                        if (data === undefined){
                            return that.$router.push({name: "Placeholder"});
                        }
                    });
            },
            deep: true,
            immediate: true
        },

        // Id changes
        // createdYear: function(){
        //     this.idChange();
        // },
        // titleShrinked: function(){
        //     this.idChange();
        // }

    }
}
</script>