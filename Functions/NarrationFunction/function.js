const awsSdk = require("aws-sdk");
const https = require("https");

const {
    S3BucketName,
    DomainNameToNotify
} = process.env;

exports.handler = async (event) => {
    console.log(DomainNameToNotify)
    const record = event.Records[0];
    const {createdYear, titleShrinked, content} = JSON.parse(record.body);

    const cleanedArticleContent = cleanAllHtml(content);
    console.log(`Cleaned Article Content:\n${cleanedArticleContent}\n`);

    console.log("Beginning audio narration");
    const audioStream = await narrate(cleanedArticleContent);
    console.log("Finished audio narration");

    const fileName = `${createdYear}-${titleShrinked}`;
    console.log("Saving to S3");
    const fileKey = await addToBucket(fileName, audioStream);
    console.log("Saved to S3");

    console.log("Notifying Webhook");
    await notifyWebhook(createdYear, titleShrinked, fileKey);
};

const cleanAllHtml = (htmlText) => {
    const pattern = /<.*?>/g;
    return htmlText.replace(pattern, "");
};

const narrate = async (content) => {
    const pollyClient = new awsSdk.Polly();

    const params = {
        OutputFormat: "mp3",
        Text: content,
        VoiceId: "Matthew"
    };
    const response = await pollyClient.synthesizeSpeech(params).promise();
    return response.AudioStream;
};

const addToBucket = async (fileName, audioStream) => {
    const s3Client = new awsSdk.S3();

    const s3Key = `ArticleNarration/${fileName}.mp3`;
    const params = {
        Bucket: S3BucketName,
        Key: s3Key,
        Body: audioStream,
        ACL: "public-read"
    };
    await s3Client.putObject(params).promise();

    const url = `https://${S3BucketName}.s3.amazonaws.com/${s3Key}`;
    console.log(`S3 url: ${url}`);
    return s3Key;
};

const notifyWebhook = async (createdYear, titleShrinked, s3BucketKey) => {
    const body = {
        "CreatedYear": createdYear,
        "TitleShrinked": titleShrinked,
        "S3BucketKey": s3BucketKey
    };
    const bodyString = JSON.stringify(body);

    try {
        const options = {
            hostname: DomainNameToNotify,
            port: 443,
            path: "/api/article/narration/complete",
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
                "Content-Length": bodyString.length
            }
        };
        await sendRequest(options, bodyString);
        console.log("Notified Webhook");
    } catch (e){
        console.log(`Could not send webhook since it was not found`);
        console.log(e);
    }
}

const sendRequest = (options, data) => {
    return new Promise((resolve, reject) => {
         const req = https.request(options);

         req.on("error", (e) => reject(e));

         req.write(data);
         req.end();
    });
};