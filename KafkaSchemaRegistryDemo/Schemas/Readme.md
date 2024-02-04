# Schemas

A schema in Kafka is a way to define the structure of the data that is being sent to Kafka. It is a way to enforce a
contract between the producer and the consumer.
The schemas are defined using protobuf and each version can be found in the `/protos` directory.

## How to generate the C# classes from the schema

The C# classes are generated using grpc tools and settings can be found in the csproj file, it's currently set to
generate the classes in the `Generated` directory when using `DEBUG` configuration.