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
	data: async function (){
		let articles = [];
		this.$store.state.postsUtil.posts().then(async (res) => {
			this.articles = await res.json();
		});

		return {
			articles: articles
		};
	},
	mounted(){
		console.log(this.articles);
	}
}
</script>

<style>

</style>