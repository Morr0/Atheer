# Atheer
## Roadmap:
### List of things to work on:
- Revamp the article add/edit editor and make it cleaner
- List all series for user when add/editing article
- Use DateTime for all datetime strings in codebase and `TIMESTAMP` for Postgresql
- Correct scheduled articles releaser to get articles older than today in a transaction
- Reconsider client side validation and do it with Bootstrap 5
- Revamp the UI of series
- Revamp the controllers for article/series
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