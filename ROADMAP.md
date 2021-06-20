# Atheer
## Roadmap:
### List of things to work on:
- Optimize data pulled from MongoDB cloud
- Properly cache article/s, i.e. handle logged in and logged out cases in server
- Configure 'robots.txt'
- Implement TOTP authentication (by user choice)
- Upload Markdown file to editor when creating new one
- Ability to download article as Markdown by reader
- For article scheduling, add currently in schedule
- Article Scheduling (rethink)
- Pull analytics from Clicky by Lambda and add data to tables
- Email system using one of the providers and link it to DNS
- Confirm users by email or else can't add/update anything other than his/her profile
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Check using anti-virus for any uploaded file
- Limit size of image uploaded to acceptable size and dimensions
- Reconsider client side validation and do it with Bootstrap 5 and get rid of JQuery
- Log external service abstractions
- Minify Html, Css and Javascript
- Hook up CDN in front of server in Cloudformation
- Implement banning user by admin
- Delay request for (n + 1) attempt by a lot
- Add failover in DNS to a different provided site
- 

### List of longer term goals (not in order):

#### Commenting system
#### Analytics dashboard for admin
#### Improved text editor includes:
    - live preview
    - add images on the fly and automatically taken care of to S3
    - remove images and automatically removed from S3
    - no need to type Markdown but still can
    - ability to add audio to play

### Maybe???
#### User friendship and chat
#### Collaboration on articles