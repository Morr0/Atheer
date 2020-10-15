// This is called when an existing record present in DynamoDB was deleted due to a TTL
// It publishes the item back to the same table

module.exports = async function (aws, record){
    console.log(JSON.stringify(record));
    
    const post = reformatPostForPublish(record.dynamodb.OldImage);
    console.log("Post was reformatted for publishing");
    console.log(JSON.stringify(post));
    
    return writeToDynamoDB(aws, post);
}

function reformatPostForPublish(post){
    post.CreatedYear.N = post.CreatedYear.N.substring(1);
    if (post.TTL)
        delete "TTL" in post;
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
        
        // TODO take care of additional data types
        if (value.N){
            item[key] = Number.parseInt(value.N);
        } else if (value.BOOL){
            item[key] = value.BOOL ? true : false;
        } else if (value.S){
            item[key] = value.S;
        }
    }
    
    const params = {
      TableName: tableName,
      Item: item
    };
    console.log("Before writing");
    
    return docClient.put(params).promise();
}