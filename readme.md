# Event sourcing exercises

📝 **Info**: Done at day 3 / 5 of [Domain Models in Practise](https://skillsmatter.com/courses/737-domain-models-in-practice), Nov 2020, given by [Marco Heimeshoff](https://skillsmatter.com/legacy_profile/marco-heimeshoff) • [@Heimeshoff](https://twitter.com/Heimeshoff).

🏷️ **Tags**: #Architecture #CQRS #Command-Query-Responsibility-Segregation #Event-Sourcing #DDD #Domain-Driven-Design #Domain-Modelling #Semantic-testing

## Domain

Company: *Cinemarco's Quality Reservation System* (CQRS 😉) :

- 100 movie theaters in Europe.
- Seats selectable and reservable (before actual booking).

## Day 2 exercise: Command and Command Handlers

☝️ **Instructions**: use TDD, use Command as input, YAGNI, Semantic testing (`Given(events); When(command); Then(events)`).

💡 **Tips**: Get momentum going and start building everything in one file and only when it gets a bit messy, extract classes into files and folders. Here is a nice video to elaborate: [Elm Europe 2017 - Evan Czaplicki - The life of a file](https://www.youtube.com/watch?v=XpDsk374LDE).

Tasks:

1. A Customer reserves specific seats at a specific screening (for simplicity, assume there exists only one screening for the time being). If available, the seats should be reserved.
2. The user will be informed, if not all seats from the reservation are available.
3. Remove all primitive data types from the domain. Use only Value Objects and Entities within the domain.
4. Bonus points: Make illegal states un-representable

## Day 3 exercise : Event Sourcing

Tasks:

1. Refactor the previous test so that the command handlers use EventSourcing as a persistance mechanism. Check Assertions on the state.
2. Publish the Events and test purely with Events and Commands, no dependency on domain state in your tests is allowed.
3. If no booking happens within 12 minutes, the reservation is cancelled.
4. Reservation is only possible up to 15 minutes before the screening.
5. The reservation system calculates the total price of the reserved tickets **(🚧 TODO)**
6. Additional behavior: When a second customer tries to reserve already reserved seats, the system treats them as unavailable

## Day 4 exercise : CQRS

**Scenario**: The customer wants to see the available seats of the screening, chooses from the list which ones to reserve and gets informed about success or failure of the reservation. The reservation is only possible up to 15 minutes before the screening.

Tasks :

- Build a read-model that supports the user with the required information. Write a test to ensure, given the past events, when a query is issued the expected response is delivered.
- Write two tests to ensure each business rule from the scenario by only using the commands and events in your test.
- Write an "integration" test that uses only commands and queries, no events to check the whole business behavior of the system.
- Bonus-behavior: If no booking happens within 12 minutes, the reservation is cancelled. Super duper bonus: Implement a way to send a notification

## Marco's solution

https://gitlab.com/heimeshoff/domain-models-in-practice-2020-11-30
