# Atheer
A blog CMS (content management system). It is the second blog project I am working on as I want this to be able to be used by anyone unlike my [previous project](https://github.com/Morr0/Project-Blog). This project will emphasize backend more than frontend as I will leave the presentation as simple as possible.

### Current components:

- Atheer Backend (API): the backend.
- Atheer Frontend (Frontend): corresponds to what the consumer of the blog sees, implements the API to fetch blogs from.
- Atheer Events: corresponds to events from the database.

### Implementation languages and reasons for use:
- Atheer Backend (API): Used the .NET core runtime with C# to develop the system because of ease of using a typed system as well as to utilize the .NET core ecosystem.
- Atheer Frontend (Frontend): Used the popular Javascript [VueJs](https://vuejs.org/v2/guide/) framework to build the frontend while integrating with the API.
- Atheer Events: A node.js handler to currently one event.

Docker is used for both Backend and Frontend.