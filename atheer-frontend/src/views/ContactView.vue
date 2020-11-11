<template>
    <div>
        <v-form>
            <v-text-field
                name="title"
                label="Title"
                v-model="title"
                min-length="10"
                max-length="250"
                counter
                required
            ></v-text-field>
            <v-text-field
                name="content"
                label="Content"
                v-model="content"
                min-length="20"
                max-length="2500"
                counter
                required
            ></v-text-field>
            <v-text-field
                name="email"
                label="Email"
                v-model="email"
                max-length="250"
                required
            ></v-text-field>
            <v-btn
            class="mr-4"
            @click="contact"
            >
            Contact
            </v-btn>
        </v-form>

        <v-snackbar v-model="showMessageBar">
            {{message}}
        </v-snackbar>
    </div>
</template>

<script>
// Consatants
const TITLE_MINIMUM = 10;
const CONTENT_MINIMUM = 20;
const TITLE_MAXIMUM = 250;
const CONTENT_MAXIMUM = 2500;

export default {
    name: "ContactView",
    data(){
        return {
            title: "",
            content: "",
            email: "",

            showMessageBar: false,
            message: "",
        };
    },
    methods: {
        contact: async function(){
            if (this.validate()){
                console.log("Valid");

                try {
                    await this.$store.state.contactsUtil.contact(this.title, this.content, this.email);

                    this.message = "You have successfully had a contact✔";
                    this.showMessageBar = true;
                } catch (error){
                    this.message = "An error has occured during transmission please retry in a few seconds";
                    this.showMessageBar = true;
                }
            }
        },
        validate: function(){
            if (this.title.length < TITLE_MINIMUM)
            {
                this.message = "The title must be at least 10 characters";
                this.showMessageBar = true;
                return;
            }

            if (this.content.length < CONTENT_MINIMUM)
            {
                this.message = "The content must be at least 20 characters";
                this.showMessageBar = true;
                return;
            }

            if (this.title.length > TITLE_MAXIMUM)
            {
                this.message = "The title must be less than 251 characters";
                this.showMessageBar = true;
                return;
            }

            if (this.content.length > CONTENT_MAXIMUM)
            {
                this.message = "The content must be less than 2501 characters";
                this.showMessageBar = true;
                return;
            }

            if (this.title.length < TITLE_MINIMUM)
            {
                this.message = "The title must be at least 10 characters";
                this.showMessageBar = true;
                return;
            }

            if (!this.validateIsEmail()){
                this.message = "Please provide a correct email address";
                this.showMessageBar = true;
                return;
            }

            return true;
        },
        validateIsEmail: function(){
            const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(this.email).toLowerCase());
        }
    }
}
</script>

<style>

</style>