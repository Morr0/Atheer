// This is called when an existing record present in DynamoDB was deleted due to a TTL
// It publishes the item back to the same table

module.exports = async function (aws, record){
    // docs = https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/time-to-live-ttl-streams.html
    // Verify was called by TTL
    if (!record.userIdentity || !record.userIdentity.type || record.userIdentity.type !== "Service")
        return;

    // Clear to execute
    console.log(JSON.stringify(record));
    
    const post = reformatPostForPublish(record.dynamodb.OldImage);
    console.log("Post was reformatted for publishing");
    console.log(JSON.stringify(post));
    
    return writeToDynamoDB(aws, post);
}

function reformatPostForPublish(post){
    post.CreatedYear.N = post.CreatedYear.N.substring(1);
    delete post.TTL;
    return post;
}

function writeToDynamoDB(aws, post){
    const docClient = new aws.DynamoDB.DocumentClient({region: "ap-southeast-2"});
    // TODO use current table name
    const tableName = "Atheer-Blog";
    console.log("Transforming post to dynamoDB object");
    
    let item = {}
    for (const key in post){
        let value = post[key];
        
        if (value.N){
            item[key] = Number.parseInt(value.N);
        } else if (value.BOOL){
            item[key] = value.BOOL ? true : false;
        } else if (value.S){
            item[key] = value.S;
        }
    }

    // Due to unknown reasons
    if (!item.CreatedYear){
        item.CreatedYear = Number.parseInt(new Date().getFullYear());
    }
    
    const params = {
      TableName: tableName,
      Item: item
    };
    console.log("Before writing");
    
    return docClient.put(params).promise();
}