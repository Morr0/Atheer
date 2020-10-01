<template>
	<v-main>
		<!-- <Article :v-for="article in articles" :v-key="article.titleShrinked || Math.random()" :article="article" /> -->
		<Article :article="{title: `This is a title`}" />
	</v-main>
</template>

<script>
import Article from "@/components/Article.vue";

export default {
	components: {
		Article
	},
	data: function (){
		return {
			articles: [
				{
					titleShrinked: "Brr"
				},
				{
					titleShrinked: "Brr2"
				}
			]
		};
	},
}
</script>

<style>

</style>