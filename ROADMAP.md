# Atheer
## Roadmap:
### List of things to work on:
- Let client render current date in local time 
- Correct tag attachment to article UI
- Correct tag attachment on backend  
- Add Polly to handle retries of errors with scheduling background service, recaptcha service, S3
- Migrate from FirstName and LastName to Name
- Login and Register with Github
- Benchmark performance in a Docker containers to simulate production server
- Add health check endpoints
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Extract all words to an international word class (basically add very basic internationalizing support)
- Sort by most recent/oldest (done the backend just do the UI)
- Word counter auto generated after each add/update of article
- List top tags
- Correct scheduled articles releaser to get articles older than today in a transaction
- 

### List of longer term goals (not in order):

#### Analytics dashboard for admin
#### Commenting system
#### Improved text editor includes:
    - live preview
    - add images on the fly and automatically taken care of to S3
    - remove images and automatically removed from S3
    - no need to type Markdown but still can
    - ability to add audio to play
#### Article pipeline after an article is updated which allows for:
    - making table of contents and attaching it to the beginning
    - narrating an article where 70% or more is text
    - ability to play narrated article