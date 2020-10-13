<template>
	<v-main>
		<Article v-for="article in articles" :key="article.titleShrinked || Math.random()" :article="article" />
		 <v-btn v-if="loadMoreMaybe" 
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
	metaInfo: {
		title: "Home"
	},
	data: function (){
		let articles = [];
		const that = this;
		this.$store.state.postsUtil.posts(this.year)
			.then((data) => that.articles = data)
			.then((data) => {
				if (data === undefined){
					return that.$router.push({name: "Placeholder"});
				}
			});

		return {
			articles: articles,
			loadMoreMaybe: true
		};
	},
	methods: {
		maybeLoadMore: function (){
			const that = this;
			this.$store.state.postsUtil.morePosts()
				.then((data) => {
					if (data === undefined)
						that.loadMoreMaybe = false;
					else 
						that.articles = that.articles.concat(data); 
				});
		}
	}
}
</script>