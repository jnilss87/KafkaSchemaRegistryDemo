# Example 3

In this example we will show:

* How to set multiple message types in the same topic

## Step 1: Fill in the credentials

Same as Example 1, fill in the configurations under Common/ConfluentCloudFixture.cs with your own credentials if not
already done.

## Step 2: Create a topic with multiple message types

We want to refine the message type we used in Example 2 to send multiple message types in the same topic.
In this example we will send a message for:

* A chat message (as before)
* A user getting permission to join a server
* A server with its channels
* A user created

### Step 2.1: Create the topic

To create a topic, run the test CreateTestTopic under ConfluentCloud and if successful, the topic will be created.

### Step 2.2: Send messages

Run the test ProduceNewServerMessages under Producer, this will send messages to the topic and also automatically
register the schemas to the schema registry.

### Step 2.3: View messages in the topic

Go to the Confluent Cloud UI and view the messages in the topic. You will see the message being sent in the topic is the
Server message only. Verify that the schema is registered in the schema registry.

## Step 3: Simulate a chat application

We have built an application that is used as a chatting platform. Let's simulate a chat application that sends messages
to the topic.

### Step 3.1: Start the consumer

Run the test ConsumeProtobufMessages under Consumer, this will continuously receive messages from the topic. Keep the
test running in its own test runner.

### Step 3.2: Create a server

Run the ProduceNewServerMessage under Producer, this will create a new server object and send it, the consumer will
receive the message and keep in memory the server and its channels.

### Step 3.3: Create a user

Run the ProduceNewUserMessage under Producer, this will create a new user, the consumer will receive the message and
keep in memory the user.

### Step 3.4: Send a chat message

Run the ProduceChatMessage under Producer, this will send a chat message, the consumer will print the message with the
user and server information.

### Step 3.5: (Optional) Expanding the chat application with more features

You can expand the chat application with more features, like sending a message to a specific channel, or sending a
message to a specific user. Also, I've intentionally left out the user getting permission to join a server,
you are free to implement this feature.

## Step 4: Clean up

### Step 4.1: Delete the topic
After you are done, you can delete the topic by running the test DeleteTestTopic under ConfluentCloud.
This will delete the topic and all its configurations.

### Step 4.2: Delete the schema
Also, you need to delete the schema by running the test DeleteSchemaSoft under Schema,
then the test DeleteSchemaPermanent under Schema.