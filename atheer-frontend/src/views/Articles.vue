<template>
    <div>
		<Article v-for="article in articles" :key="article.titleShrinked || Math.random()" :article="article" />
    </div>
</template>

<script>
import Article from "@/components/Article.vue";

export default {
	components: {
		Article
	},
	data: function (){
		let articles = [];
		this.$store.state.postsUtil.posts()
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