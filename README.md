# Message Silo

Message Silo is a powerful and user-friendly message queue monitoring platform that allows users to easily monitor and correct their dead-lettered messages from various platforms such as Azure Service Bus, AWS SMS, RabbitMQ, and more. 
With its intuitive and easy-to-use interface, users can quickly and easily identify and resolve issues with their messaging queues, ensuring that their systems run smoothly and efficiently at all times. 
Additionally, the platform offers a range of advanced features such as message searching and filtering, push notifications, and detailed metrics and analytics, making it the ideal solution for businesses of all sizes that rely on messaging queues to drive their operations.

## Note
This project is currently under development and the features are not fully implemented!

## Upcoming features


**Integration**
- [x] Azure Service Bus
- [ ] RabbitMQ
- [ ] AWS SMS
- [ ] Others...

**Monitoring**
- [ ] Dashboard
- [ ] Custom queries against messages from different sources
- [ ] Statistics
- [ ] Alerts


**Dead Letter Corrector**

- [x] Correct messages with custom JS func. 
- [ ] Automatic resend 
- [ ] Correct messages with a custom HTTP call

## Build & Run

```bash
cd MessageSilo
docker-compose up --build
```

## Contribution
If you are interested in contributing to the project, please open an issue or submit a pull request.