# Atheer
## Roadmap:
### List of things to work on:
- Integrate with Clicky for analytics
- Verify user by Recaptcha at sign up
- Verify user by Recaptcha when have not logged in for a week
- Add VPC gateway endpoint for when accessing through VPC
- Add Cloudfront in front of S3 for when accessing images from the public 
- Add user last seen
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Extract all words to an international word class (basically add very basic internationalizing support)
- Sort by most recent/oldest (done the backend just do the UI)
- Word counter auto generated after each add/update of article
- List top tags

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