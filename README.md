<img src="https://repository-images.githubusercontent.com/495369194/7373d34b-41e3-4eb7-90a7-773cc33f3fa5" align="right"></img>
# Message Silo

[![siloctl](https://github.com/MessageSilo/MessageSilo/actions/workflows/siloctl.yml/badge.svg)](https://github.com/MessageSilo/MessageSilo/actions/workflows/siloctl.yml)

Message Silo is a powerful and user-friendly message queue monitoring platform that allows users to easily monitor and correct their dead-lettered messages from various platforms such as Azure Service Bus, AWS SQS, RabbitMQ, and more. 
With its intuitive and easy-to-use interface, users can quickly and easily identify and resolve issues with their messaging queues, ensuring that their systems run smoothly and efficiently at all times. 
Additionally, the platform offers a range of advanced features such as message searching and filtering, push notifications, and detailed metrics and analytics, making it the ideal solution for businesses of all sizes that rely on messaging queues to drive their operations.

## Note
This project is currently under development and the features are not fully implemented!

## Upcoming features

**Integration**
- [x] Azure Service Bus
- [x] RabbitMQ
- [ ] AWS SQS
- [ ] Others...

**Monitoring**
- [ ] Dashboard
- [ ] Custom queries against messages from different sources
- [ ] Statistics
- [ ] Alerts


**Dead Letter Corrector**

- [x] CRUD (with CLI)
- [x] Correct messages with custom JS func. 
- [x] Automatic resend 
- [ ] Correct messages with a custom HTTP call

## Build & Run

```bash
cd MessageSilo.Silo
dotnet run

cd MessageSilo.API
dotnet run
```

API listening on: https://localhost:5000

## Contribution
If you are interested in contributing to the project, please open an issue or submit a pull request.