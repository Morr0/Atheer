# Atheer
## Roadmap:
### List of things to work on:
- Make code more readable in an article
- Admin can make a basic user an editor
- Admin can make an editor a basic user
- Ability to search using an article's title, content, description
- Sort by most recent/oldest
- S3 abstraction with choice of public/private data storage
- Ability for a user to upload profile image
- Word counter auto generated after each add/update of article
- Article image stored to S3
- List top tags
- Integrate with Clicky for analytics
- Verify user by Recaptcha at sign up
- Verify user by Recaptcha when have not logged in for a week
- Update cloudformation for S3 and add VPC gateway endpoint for when accessing through VPC
- Fix DOTNET_CLI_HOME bug
- 

### List of things if have time:
- Update cookie full name after name change of user
- Extract all string constants referenced both in razor and C# into their respective classes
- Extract all words to an international word class (basically add very basic internationalizing support)

### List of longer term goals (not in order):

#### Analytics dashboard for admin
#### Commenting system
#### Improved text editor includes:
    - live preview
    - add images on the fly and automatically taken care of to S3
    - remove images and automatically removed from S3
    - no need to type Markdown but still can
    - add <iframe>
    - ability to add audio to play
#### Article pipeline after an article is updated which allows for:
    - making table of contents and attaching it to the beginning
    - narrating an article where 70% or more is text
    - ability to play narrated article