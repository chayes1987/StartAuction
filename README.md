# StartAuction
This is the start auction service for my FYP. It is written in C#. It uses a 0mq binding and also connects to a Redis database.
It is responsible for retrieving the bidders and starting the auction.

## Project Setup

Requires a Redis instance running on port 6379 with the correct namespace.

Namespace: 0

Sample Item:
  key: 1
  value: a set of email addresses

## License

None

## Setting up StartAuction service on AWS

- Created AWS EC2 Windows instance
- Connected with RDP and .pem keyfile
- Copied Redis build to instance
- Copied Service build to instance
- Configure Windows Firewall to allow port access for 0mq

- Service runs and works as expected
