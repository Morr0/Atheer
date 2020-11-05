const aws = require("aws-sdk");

const deleteDueToTTL = require("./events/deleteDueToTTL");
const sharePostToSocialMedia = require("./events/sharePostToSocialMedia");

exports.handler = async (event) => {
    console.log(JSON.stringify(event));
    // To catch errors so it does not retry
    try {
        for (const key in event.Records){
            const record = event.Records[key];
            
            switch (record.eventName){
                case "REMOVE":
                    await deleteDueToTTL(aws, record);
                case "INSERT":
                    await sharePostToSocialMedia(aws, record);
            }
        }
    } catch (error){
        console.log(error);
    }
    
    const response = {
        statusCode: 200,
        body: JSON.stringify('DynamoDB stream processed'),
    };
    return response;
};