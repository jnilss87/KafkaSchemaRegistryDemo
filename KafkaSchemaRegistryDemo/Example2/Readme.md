# Example 2

In this example we will show:

* How to evolve a schema
* Compatibility checks

## Step 1: Fill in the credentials

Same as Example 1, fill in the configurations under Common/ConfluentCloudFixture.cs with your own credentials if not
already done.

## Step 2: Create subject

Now we want to start over and make sure that we use the schema registry from the beginning. We will create a new subject
with the same schema as the previous example.

### Step 2.1: Create register schema version 1

Run the test RegisterSchemaVersion1 under Schema, this will create a schema and register it to a value subject.

### Step 2.2: View the schema in the schema registry

Go to the Confluent Cloud UI and view the schema in the schema registry. You will see that the schema is now registered
under subject see Example2Config.cs.
The schema is registered with BACKWARD compatibility.

### Step 2.3: Check what compatibility the schema has

Run the test GetCompatibilitySettingsForSubject under Schema, this will check what compatibility the schema has.
This will fail and say that the subject does not have subject-level compatibility configured, I assume that this is a
bug and that the default compatibility BACKWARD is used.

### Step 2.3: Set subject-level compatibility to BACKWARD

Run the test SetBackwardCompatibility under Schema, this will set the compatibility to BACKWARD for the subject.
Check the compatibility again by running the test GetCompatibilitySettingsForSubject under Schema, this will now say
that the subject has BACKWARD compatibility.

## Step 3: Evolve the schema to version 2

New requirements have come in! We now support Channels and want to add this to the schema, at first channels is not a
well-defined concept, so we will just add it as a string.

### Step 3.1: Check compatibility with version 1

Run PrintSchemaV2 under Schema, this will print the new schema to the console.

Run the test CheckCompatibilityV2 under Schema, this will check if the new schema is compatible with the latest version
of the subject.

### Step 3.2: Register schema version 2

Run the test RegisterSchemaVersion2 under Schema, this will create a new version of the schema and register it to the
same subject.

### Step 3.2: View the schema in the schema registry

Go to the Confluent Cloud UI and view the schema in the schema registry. You will see that the schema is now registered
with two versions.

## Step 4: Evolve the schema to version 3

After a while we have realized that we need to extend channels to be id based, so that channels don't need to be
uniquely named.

### Step 4.1: Change field type, check compatibility

We decide to change the channel field from string to a new message type Channel that has an id field and a name field.
Run the test CheckCompatibilityV3Incompatible under Schema, this will check if the new schema is compatible with the
latest version
Since this schema breaks compatibility, it will fail.

### Step 4.2: Add new field, check compatibility

Since we can't change the field type, we decide to add a new field to the schema and deprecate the old field, this is
done by adding a new message type Channel and adding new field then marking the old field number as reserved.

Run PrintSchemaV3 under Schema, this will print the new schema to the console.

Run CheckCompatibilityV3 under Schema, this will check if the new schema is compatible with the latest version
This will pass, and we can register the new schema.

### Step 4.3: Register schema version 3

Run the test RegisterSchemaVersion3 under Schema, this will create a new version of the schema and register it to the
same subject.

### Step 4.4: View the schema in the schema registry

Go to the Confluent Cloud UI and view the schema in the schema registry. You will see that the schema is now registered

## Step 5: Forward compatibility check

To illustrate differences between forward and backward compatibility, we will now set the compatibility to FORWARD and
try to register a new schema.

### Step 5.1: Set subject-level compatibility to FORWARD

Run the test SetForwardCompatibility under Schema, this will set the compatibility to FORWARD for the subject.

### Step 5.2: Try to register schema version 4

We got additional requirements! Channels now belongs to Servers and each User have permissions to servers.
The message will now include a new message type Server and a new message type ServerPermission, we will also add a new
field to the message type Channel with server id.
_This is not a good way to structure the schema, but it will serve as a good example._

### Step 5.3: Check compatibility with version 3

Run PrintSchemaV4 under Schema, this will print the new schema to the console.

Run the test CheckCompatibilityV4 under Schema, this will check if the new schema is compatible with the latest version.
This will fail, and we can't register the new schema. This is expected since the new schema breaks FORWARD compatibility
by adding new message types and non-optional fields. If a consumer is using the old schema, it will not be able to
deserialize the new message.

### Step 5.4: Set subject-level compatibility to BACKWARD

Run the test SetBackwardCompatibility under Schema, this will set the compatibility to BACKWARD for the subject again.

### Step 5.5: Register schema version 4

Run the test RegisterSchemaVersion4 under Schema, this will create a new version of the schema and register it to the
same subject.

### Step 5.6: View the schema in the schema registry

Go to the Confluent Cloud UI and view the schema in the schema registry. You will see that the schema is now registered.
You can also run the test GetAllSchemaVersions under Schema to see that the schema is registered with four versions.

### Step 6: Clean up

Clean up Subject by running the test DeleteSchemasSoft under Schema, this will softly delete the subject and all its
versions, this is part of the deletion process and will not delete the subject immediately.
Then run the test DeleteSchemasPermanent under Schema, this will permanently delete the subject and all its versions.




