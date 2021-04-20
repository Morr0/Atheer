# Atheer
## Roadmap:
### List of things to work on:
- Fix "Edit" not showing on drafted articles 
- Implement OnlyVisibleToNetwork filter to only allow traffic from same network
- Word counter auto generated after each add/update of article
- Revamp the article add/edit editor and make it cleaner
- Revamp the controllers for article/series  
- Correct scheduled articles releaser to get articles older than today in a transaction
- Commenting system
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Limit size of image uploaded to 4MB
- Complete the JSON feed to full specs
- Sort by most recent/oldest (done the backend just do the UI)
- 

### List of longer term goals (not in order):

#### Analytics dashboard for admin
#### Improved text editor includes:
    - live preview
    - add images on the fly and automatically taken care of to S3
    - remove images and automatically removed from S3
    - no need to type Markdown but still can
    - ability to add audio to play