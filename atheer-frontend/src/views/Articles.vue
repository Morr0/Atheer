<template>
	<v-main>
		<Article v-for="article in articles" :key="article.titleShrinked || Math.random()" :article="article" />
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
		this.$store.state.postsUtil.posts(this.year)
			.then((data) => this.articles = data);

		return {
			articles: articles
		};
	},
	mounted(){
		
	}
}
</script>

<style>

</style>