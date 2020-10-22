import click
import json
import boto3
import uuid
import time

# Constants
constAtheerPostsTableName = "Atheer-Post-TableName"
constAtheerBackendName = "AtheerBackendName"

def create_bucket(client, bucket_name):
    try:
        response = client.create_bucket(
        Bucket = bucket_name,
        Tagging = "Project=Atheer"
        )
    except:
        # Will skip since will assume the user already has that bucket for continuous deployments
        pass 

def put_templates_into_bucket(client, bucket_name: str, setup):
    with open("./AtheerBackend/serverless.template") as openFile:
        data = openFile.read()
        client.put_object(
            Bucket = bucket_name,
            # TODO refactor the line below
            Key = setup[constAtheerBackendName] + "BackendTemplate",
            Body = data,
            Tagging = "Project=Atheer"
        )

@click.command()
@click.option("--file", default = "setup.json", help = "The *.json file holding the setup config")
def main(file):
    print(file)
    # TODO handle not exists
    with open(file, "r") as openFile:
        data = openFile.read()
        print(data)
        setup = json.loads(data)
        print(setup)

        # Inject default if not exist
        if constAtheerBackendName not in setup:
            setup[constAtheerBackendName] = "Atheer-backend"
        if constAtheerPostsTableName not in setup:
            setup[constAtheerPostsTableName] = "Atheer-BlogPosts"

        # Publish to S3 the templates
        # bucket_name = setup[constAtheerBackendName] + "Bucket"
        # s3 = boto3.client("s3")
        # create_bucket(s3, bucket_name)
        # put_templates_into_bucket(s3, bucket_name, setup)

        # Publish the Atheer Backend using it's template
        with open("./AtheerBackend/serverless.template") as openFile:
            data = openFile.read()
            client = boto3.client("cloudformation")
            stack_name = setup[constAtheerBackendName] + str(uuid.uuid4())
            cs_response = client.create_stack(
                StackName = stack_name,
                TemplateBody = data,
                Parameters = [
                    {
                        "ParameterKey": "NameParameter",
                        "ParameterValue": setup[constAtheerBackendName]
                    }
                ],
                Capabilities = [
                    "CAPABILITY_AUTO_EXPAND"
                ]
            )

            # Wait until the change set is created
            # while True:
            #     cs_description = client.describe_change_set(
            #         StackName = stack_name,
            #         ChangeSetName = stack_name
            #     )

            #     time.sleep(8)
            #     if cs_description["Status"] == "CREATE_COMPLETE":
            #         break


if __name__ == "__main__":
    main()