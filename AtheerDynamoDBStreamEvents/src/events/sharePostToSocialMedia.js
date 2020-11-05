module.exports = async function (aws, record){
    console.log(record);

    if ("TTL" in record.dynamodb.NewImage)
        return;
    
    const article = record.dynamodb.NewImage;
    const year = article.CreatedYear;
    const titleShrinked = article.TitleShrinked;
    
    // Launch an EC2 instance to handle back and forth OAuth
    const ec2Client = new aws.EC2();
    return ec2Client.runInstances({
        InstanceType: "t2.micro",
        InstanceInitiatedShutdownBehavior: "terminate",
        UserData: `
        #!/bin/bash
        yum install git -y
        yum install docker -y
        service docker start
        POST_YEAR=${year}
        POST_TITLE=${titleShrinked}
        cd /home/ec2-user/app
        git clone https://github.com/Morr0/Atheer .
        cd ./AtheerSocialMediaSharer
        docker build --build-arg POST_YEAR=POST_YEAR --build-arg POST_TITLE=POST_TITLE -t share:1 .
        docker run share:1
        sleep 10m; sudo shutdown -h now`
    }).promise();
}