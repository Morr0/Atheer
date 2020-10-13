<template>
    <div>
<!-- A PLACE HOLDER COMPONENT TO REROUTE TO THE Home VIEW WHEN REROUTING IS 
WITHIN THE SAME COMPONENT. -->
    </div>
</template>

<script>
// TODO collect data of the previous endpoint so it is known where did users go
export default {
    metaInfo: {
        title: "Redirecting ..."
    },
    mounted(){
        return this.$router.push({name: "Home"});
    }
}
</script>