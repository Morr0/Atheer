# Atheer
## Roadmap:
### List of things to work on:
- Add article version and wasPublished
- Update popular tags view to not display any private (forcefully unlisted && unlisted && draft && not currently scheduled)
- Update article version for each update
- Rate limiting system on authentication endpoints and locking (let client cache results)
- Upload Markdown file to editor when creating new one
- Ability to download article as Markdown by reader
- For article scheduling, add currently in schedule
- Article Scheduling (rethink)
- Pull analytics from Clicky by Lambda and add data to tables
- Properly cache article/s, i.e. handle logged in and logged out cases
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Check using anti-virus for any uploaded file
- Limit size of image uploaded to acceptable size and dimensions
- Reconsider client side validation and do it with Bootstrap 5 and get rid of JQuery
- Log external service abstractions
- Minify Html, Css and Javascript
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