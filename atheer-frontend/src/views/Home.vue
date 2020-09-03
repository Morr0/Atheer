<template>
  <div class="home">
    <Posts />
  </div>
</template>

<script>
import Posts from "@/views/Posts.vue";

export default {
  name: 'Home',
  components: {
      Posts
  }
}
</script>
