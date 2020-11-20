<template>
    <div>
        <Articles :admin="true" />
    </div>
</template>

<script>
import Articles from "@/views/Articles.vue";
import ArticleEdit from "@/components/ArticleEdit.vue";

export default {
    components: {
        Articles,
        ArticleEdit,
    }
}
</script>

<style>

</style>