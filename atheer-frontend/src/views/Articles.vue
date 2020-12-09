<template>
	<v-main>
		<div v-if="admin">
            <v-list two-line>
                <BareArticle v-for="article in articles" :key="article.titleShrinked || Math.random()" :article="article" />
            </v-list>
        </div>



        <div v-else>
            <v-card-title>
			    {{year ? `${year} posts` : "Latest posts"}}
            </v-card-title>

            <Article v-for="article in articles" :key="article.titleShrinked || Math.random()" :article="article" />        </div>
	</v-main>
</template>

<script>
import Article from "@/components/Article.vue";
import BareArticle from "@/components/BareArticle.vue";

export default {
	components: {
        BareArticle,
        Article
	},
	props: {
        year: String,
        admin: Boolean
	},
    metaInfo(){
        document.loadMore = this.loadMore;
        return {
            title: "Home",
            content: 'width=device-width, initial-scale=1',
            charset: "utf-8",
            meta: [
                {
                    name: "description",
                    content: this.$store.state.description || "Atheer Home page"
                },
                {
                    property: "og:title",
                    content: this.$store.state.title || "Atheer home"
                },
                {
                    property: "og:description",
                    content: this.$store.state.description || "Atheer Blog"
                },
                // TODO add logo of Atheer here
                // {
                //     property: "og:image",
                //     content: this.$page.post.image || ""
                // }
            ]
        };
    },
	data: function (){
		let articles = [];
        const that = this;

        const functionToCall = this.admin ? this.$store.state.postsUtil.barePosts() : this.$store.state.postsUtil.posts(this.year)
		functionToCall.then((data) => {
                if (!data){
                    return that.$router.push({name: "Placeholder"});
                }

                return that.articles = data;
            });

		return {
			articles: articles,
		};
    }
}
</script>