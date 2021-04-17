const awsSdk = require("aws-sdk");
// const fetch = require("node-fetch");

const {
    S3BucketName,
    DomainNameToNotify
} = process.env;

exports.handler = async (event) => {
    const record = event.Records[0];
    const {createdYear, titleShrinked, content} = JSON.parse(record.body);

    const cleanedArticleContent = cleanAllHtml(content);
    console.log(`Cleaned Article Content:\n${cleanedArticleContent}\n`);

    console.log("Beginning audio narration");
    const audioStream = await narrate(cleanedArticleContent);
    console.log("Finished audio narration");

    const fileName = `${createdYear}-${titleShrinked}`;
    console.log("Saving to S3");
    const audioUrl = await addToBucket(fileName, audioStream);
    console.log("Saved to S3");

    // TODO notify webhook using no package other than nodejs ones
    // console.log("Notifying Webhook");
    // await notifyWebhook(createdYear, titleShrinked, audioUrl);
    // console.log("Notified Webhook");
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
        Body: audioStream
    };
    await s3Client.putObject(params).promise();

    const url = `https://${S3BucketName}.s3.amazonaws.com/${s3Key}`;
    console.log(`S3 url: ${url}`);
    return url;
};

// const notifyWebhook = async (createdYear, titleShrinked, audioUrl) => {
//     const endpoint = `https://${DomainNameToNotify}/api/article/narration/complete`;
//     const body = {
//         "CreatedYear": createdYear,
//         "TitleShrinked": titleShrinked,
//         "S3Url": audioUrl
//     };
//     try {
//         await fetch(endpoint, {
//             method: "PATCH",
//             body: JSON.stringify(body),
//             headers: {
//                 "Content-Type": "application/json"
//             }
//         });
//     } catch (e){
//         console.log(`The url: ${endpoint} was not found`);
//     }
// }