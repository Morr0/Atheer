# Atheer
## Roadmap:
### List of things to work on:
- Hook up logging with Forbidden/Unauthorized endpoints
- Refresh config every 5 mins
- Minify Html, Css and Javascript
- Equalize Header elements in article view
- Update popular tags view to not display any private (forcefully unlisted && unlisted && draft)
- Commenting system
- Rate limiting system on authentication endpoints and locking (let client cache results)
- Upload Markdown file to editor when creating new one
- Ability to download article as Markdown by reader
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Check using anti-virus for any uploaded file
- Limit size of image uploaded to acceptable size and dimensions
- Reconsider client side validation and do it with Bootstrap 5
- Log external service abstractions

### List of longer term goals (not in order):

#### Analytics dashboard for admin
#### Article scheduler (rethink system)
#### Improved text editor includes:
    - live preview
    - add images on the fly and automatically taken care of to S3
    - remove images and automatically removed from S3
    - no need to type Markdown but still can
    - ability to add audio to play