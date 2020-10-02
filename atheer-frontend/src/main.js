<<<<<<< HEAD
import Vue from 'vue'
=======
import { createApp } from 'vue'
>>>>>>> 460499ccc276f20fbae07362b8f0c711e84db0b7
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
<<<<<<< HEAD
import vuetify from './plugins/vuetify';

Vue.config.productionTip = false

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
=======

createApp(App).use(store).use(router).mount('#app')
>>>>>>> 460499ccc276f20fbae07362b8f0c711e84db0b7
