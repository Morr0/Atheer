# atheer-frontend

The frontend of the Atheer blog management system.

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Run your unit tests
```
npm run test:unit
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).

### Docker image
- To build use ``` docker build -t atheer-frontend:latest . ```. 
- To run use ``` docker run -p <HOST PORT>:8080 -e VUE_APP_API=<BACKEND API URL> atheer-frontend ```. 
