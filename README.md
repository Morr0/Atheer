# Atheer
A blog CMS (content management system). It is the second blog project I am working on as I want this to be able to be used by anyone unlike my [previous project](https://github.com/Morr0/Project-Blog). This project will emphasize backend more than frontend as I will leave the presentation as simple as possible.

### Current components:

- Atheer Backend (API): corresponds for reading blog data as well as manipulating post specific things like likes and shares.
- Atheer Editor App (Editor client): corresponds for writing/editing/deleting blogs.
- Atheer Frontend (Frontend): corresponds to what the consumer of the blog sees, implements the API to fetch blogs from.
- Atheer Events: corresponds to events from the database.

### Implementation languages and reasons for use:
- Atheer Backend (API): Used the .NET core runtime with C# to develop the system because of ease of using a typed system as well as to utilize the .NET core ecosystem. It is developed as a standalone WebApi however it is intended to be used as a lambda function and it is so. The reason for using [lambda](https://aws.amazon.com/lambda/), there is no need to manage any infrastructure for a blog.
- Atheer Editor App (Editor client): Used the .NET WPF to develop it with C#. This is an easy choice for me as looks are not really important. Could have gone with a simple web app, does not matter, I did WPF for ease of using the XAML editor by Visual Studio. To use it, you need a secret to add/update/delete a blog post, which is stored in [AWS Systems Manager Parameter Store](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-parameter-store.html). I did that just so I can an already made system rather than rolling up a user system with login for now which is unnecessary.
- Atheer Frontend (Frontend): Used the popular Javascript [VueJs](https://vuejs.org/v2/guide/) framework to build the frontend while integrating with the API. I used this rather .NET core frontend technologies like Razor pages for the reason that I have used before along with NodeJs. Used docker on it, the reason for it is currently temporary to just use a dynamic website whereas later on will use a static site generator out of this dynamic site.
- Atheer Events: A node.js handler to currently one event.