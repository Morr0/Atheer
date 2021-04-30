# Atheer
## Roadmap:
### List of things to work on:
- Add Cloudfront distribution on top of EC2 in Cloudformation
- Add Cloudformation resource to add link to icon
- Hook up logging
- Hook up logging with NotFound and Forbidden endpoints
- Minify Html, Css and Javascript
- Update popular tags view to not display any private (forcefully unlisted && unlisted && draft)
- Commenting system
- Rate limiting system on authentication endpoints and locking (let client cache results)
- Upload Markdown file to editor when creating new one
- Ability to download article as Markdown by reader
- 

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Limit size of image uploaded to acceptable size and dimensions
- Reconsider client side validation and do it with Bootstrap 5
-

### List of longer term goals (not in order):

#### Analytics dashboard for admin
#### Article scheduler (rethink system)
#### Improved text editor includes:
    - live preview
    - add images on the fly and automatically taken care of to S3
    - remove images and automatically removed from S3
    - no need to type Markdown but still can
    - ability to add audio to play