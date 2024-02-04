# Example 1

In this example we will show:

* That there are different ways of sending a message to kafka
  and that kafka does not care about the message format.
* How to create a schema for the topic
* What happens when we send a message that does not conform to the schema

## Step 1: Fill in the credentials

Fill in the configurations under Common/ConfluentCloudFixture.cs with your own credentials.

## Step 2: Create topic

To create a topic, run the test CreateTestTopic under ConfluentCloud and if successful, the topic will be created.

## Step 3: Test the chat application

We have built an application that is used as a chatting platform. At first, we did not know what the message format
would be, so we just sent a byte array to kafka.

### Step 3.1: Send messages

Run the ProduceRawCsharpMessages under Producer, this is will continuously send messages to the topic.
If successful, you will see output that the message has been sent.
Abort the test after a few seconds.

### Step 3.2: View messages in the topic

Go to the Confluent Cloud UI and view the messages in the topic. You will see that the messages are in the byte array
format.

### Step 3.3: Receive messages

Run the ConsumeRawCsharpMessages under Consumer, this will continuously receive messages from the topic.
If successful, you will see the messages that been received.
Abort the test after a few seconds.

## Step 4: Create a Schema for the message

Now we have settled on a message format, and we want to enforce this format to get all the benefits of schema registry.

### Step 4.1: Create a schema and register it

Run the test AttachSchemaToTopic under Schema, this will create a schema and register it to the topic using
TopicNameStrategy.
_Optionally you can see how we get the schema definition from the file and send it to the schema registry, by looking at
the test PrintSchema under Schema._

### Step 4.2: Send messages with the schema

Run ProduceProtobufMessages under Producer, this will continuously send messages to the topic using the schema.
If successful, you will see output that the message has been sent.
Abort the test after a few seconds.

### Step 4.3: View messages in the topic

Go to the Confluent Cloud UI and view the messages in the topic. You will see that the messages are now in the schema

### Step 4.4: Receive messages with the schema

Run ConsumeProtobufMessages under Consumer, this will continuously receive messages from the topic using the schema.
If successful, you will see the messages that been received.
Abort the test after a few seconds.

## Step 5: Send a message that does not conform to the schema

We made a mistake while deploying the application and we started to send messages with the old byte array format.

### Step 5.1: Send messages

Run the ProduceRawCsharpMessages under Producer again, kafka will accept the message since it does not care about the
format.

### Step 5.2: Receive messages

Run the ConsumeProtobufMessages under Consumer, this will not be able to deserialize the message, and you will see an
error.
This is expected since the message does not conform to the schema that is being used to deserialize the message.
_If you try to run the ConsumeRawCsharpMessages under Consumer, you will see that it might take a while before the
messages are received, this is because the consumer group keeps track of which consumer has which offset and will wait
for a timeout before it will send the message to another consumer in the group._

## Step 6: Clean up

Run the test DeleteTestTopic under ConfluentCloud to delete the topic.
Run the test DeleteSchema under Schema to delete the schema, since it won't be deleted when the topic is deleted.
