# Atheer
## Roadmap:
### List of things to work on:
- Add unit tests to TagFactory
- Add ability to persist Navbar items
- Allow application on startup to fetch nav bar items from database
- Allow admin to add/remove navbar items
- Limit size of image uploaded to 4MB
- Integrate with some kind of a feed (Json feed or RSS), publish api for it
- Design internals of ArticleSeries
- Add UI to create a new article series
- Allow in the article editor to link this article to an article series
- Allow an article series to be in a state of finished once you don't want any more articles in it
- Sort by most recent/oldest (done the backend just do the UI)
- Word counter auto generated after each add/update of article
- List top tags
- Correct scheduled articles releaser to get articles older than today in a transaction
- Commenting system
- Implement OnlyVisibleToNetwork filter to only allow traffic from same network

### List of things if have time:
- Extract all string constants referenced both in razor and C# into their respective classes
- Extract all words to an international word class (basically add very basic internationalizing support)
- 

### List of longer term goals (not in order):

#### Analytics dashboard for admin
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