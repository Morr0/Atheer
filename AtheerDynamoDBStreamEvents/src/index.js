const aws = require("aws-sdk");

const deleteDueToTTL = require("./events/deleteDueToTTL");

exports.handler = async (event) => {
    console.log(JSON.stringify(event));
    for (const key in event.Records){
        const record = event.Records[key];
        
        switch (record.eventName){
            case "REMOVE":
                await deleteDueToTTL(aws, record);
        }
    }
    
    const response = {
        statusCode: 200,
        body: JSON.stringify('DynamoDB stream processed'),
    };
    return response;
};