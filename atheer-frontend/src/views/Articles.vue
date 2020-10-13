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
		const that = this;
		this.$store.state.postsUtil.posts(this.year)
			.then((data) => articles = data)
			.then((data) => {
				if (data === undefined){
					// return that.$router.replace();
					return that.$router.push({name: "Home"});
				}
			});

		return {
			articles: articles
		};
	},
}
</script>