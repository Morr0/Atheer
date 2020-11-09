<template>
	<v-main>
		<v-card-title>
			{{year ? `${year} posts` : "Latest posts"}}
		</v-card-title>

		<Article v-for="article in articles" :key="article.titleShrinked || Math.random()" :article="article" />
		 <v-btn v-if="loadMore" 
		 text type="button" @click="maybeLoadMore">Load more ???</v-btn>
	</v-main>
</template>

<script>
import Article from "@/components/Article.vue";

export default {
	components: {
		Article
	},
	props: {
		year: String
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
		this.$store.state.postsUtil.posts(this.year)
			.then((data) => {
                if (!data){
                    return that.$router.push({name: "Placeholder"});
                }

                that.loadMore = data.canLoadMore;
                return that.articles = data.posts;
            });

		return {
			articles: articles,
			loadMore: false
		};
	},
	methods: {
		maybeLoadMore: function (){
			const that = this;
			this.$store.state.postsUtil.morePosts()
				.then((data) => {
                    that.loadMore = data.canLoadMore;
                    that.articles = that.articles.concat(data.posts); 
				});
		}
	}
}
</script>