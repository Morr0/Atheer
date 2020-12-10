<template>
    <div>
        <v-form>
            <v-row>
                <v-text-field 
                    label="Created Year"
                    v-model="article.createdYear"
                    @change="idChange"
                ></v-text-field>
                <v-spacer />
                <v-text-field
                    label="Title Shrinked"
                    v-model="article.titleShrinked"
                    @change="idChange"
                ></v-text-field>
                <v-btn ref="refreshButton" v-if="mode === `edit`" text @click.prevent="refresh">Refresh</v-btn>
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
    data(){
        return {
            createdYear: this.$route.query.createdYear,
            titleShrinked: this.$route.query.titleShrinked,
            mode: MODE_NEW_ARTICLE,
            changedIdsWhileEditing: false,
            article: {}
        };
    },
    methods: {
        checkout: async function (){
            this.validate();
        },
        validate: function (){

        },
        refresh: async function (){
            if (this.changedIdsWhileEditing){
                console.log("Trying to refresh");
                try {
                    await this.getPost();
                } catch (error){
                    this.reset();
                }

                this.mode = MODE_EDIT_ARTICLE; 
                this.changedIdsWhileEditing = false;
            } else
                console.log("Nothing has changed so will not refresh");
        },
        reset: function (){
            console.log("Post does not exist");
            this.$set(this.article, "createdYear", this.createdYear);
            this.$set(this.article, "titleShrinked", this.titleShrinked);
            this.changedIdsWhileEditing = false;
            console.log("RESET DONE");
        },
        idChange: function (){
            if (this.article.createdYear !== this.createdYear || this.article.titleShrinked != this.titleShrinked){
                this.mode = MODE_EDIT_ARTICLE;
                this.changedIdsWhileEditing = true;
            } else {
                this.changedIdsWhileEditing = false;
            }
        },
        getPost: async function (){
            if (this.mode === MODE_EDIT_ARTICLE){
                const createdYear = this.changedIdsWhileEditing ? this.article.createdYear : this.createdYear;
                const titleShrinked = this.changedIdsWhileEditing ? this.article.titleShrinked : this.titleShrinked;

                console.log("HERE");
                const article = await this.$store.state.postsUtil.post(createdYear, titleShrinked);
                if (article) 
                    this.article = article;
                else
                    throw new Error();

                this.createdYear = createdYear;
                this.titleShrinked = titleShrinked;
            }
        }
    },
    mounted(){
        console.log(this.createdYear);
        console.log(this.titleShrinked);
        this.mode = this.titleShrinked && this.createdYear ? MODE_EDIT_ARTICLE : MODE_NEW_ARTICLE;

        const that = this;  
        this.getPost().catch((error) => that.$router.push({name: "Placeholder"}));
    }
}
</script>