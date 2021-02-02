const fs = require("fs");
const aws = require("aws-sdk");
aws.config.update({
    region: "ap-southeast-2"
});

(async () => {
    const client = new aws.DynamoDB();
    
    const articles = await getFromDynamoDBAsOrganised(client, "Atheer-Blog");
    const users = await getFromDynamoDBAsOrganised(client, "Atheer-Users");
    
    const articlesSql = getSqlForTargetTableName(articles, "Article");
    const usersSql =  getSqlForTargetTableName(users, "User");
    
    const combinedSql = `${JSON.stringify(articlesSql)}\n${JSON.stringify(usersSql)}`;
    fs.writeFileSync("sql.sql", combinedSql, {
        encoding: "utf8"
    });
})();

async function getFromDynamoDBAsOrganised(client, tableName){
    const res = await client.scan({
        TableName: tableName
    }).promise();
    
    let organisedItems = [];
    for (const item of res.Items){
        let organisedItem = {};
        
        for (const name in item){
            // Since no longer using topics
            if (name === "Topics") continue;
            
            const attribute = item[name];
            
            if (attribute.S) organisedItem[name] = attribute.S;
            else if (attribute.N) organisedItem[name] = attribute.N;
            else if (attribute.BOOL) organisedItem[name] = attribute.BOOL;
        }
        
        organisedItems.push(organisedItem);
    }
    
    return organisedItems;
}

function getSqlForTargetTableName(data, targetTableName){
    const sqlTargetTableName = `"${targetTableName}"`;
    let statements = [];
    
    for (const record of data){
        let names = [];
        let values = [];
        
        for (const name in record){
            names.push(`"${name}"`);
            const value = record[name];
            values.push(`'${value}'`);
        }
        
        const commaSeparatedNames = names.join(',');
        const commaSeparatedValues = values.join(',');
        const statement = `INSERT INTO ${sqlTargetTableName}(${commaSeparatedNames}) VALUES (${commaSeparatedValues});`;
        statements.push(statement);
    }
    
    return statements.join();
}