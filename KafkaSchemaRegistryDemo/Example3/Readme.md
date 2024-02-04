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
