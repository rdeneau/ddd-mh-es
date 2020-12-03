# Event sourcing exercises

📝 **Info**: Done at day 3 / 5 of [Domain Models in Practise](https://skillsmatter.com/courses/737-domain-models-in-practice), Nov 2020, given by [Marco Heimeshoff](https://skillsmatter.com/legacy_profile/marco-heimeshoff) • [@Heimeshoff](https://twitter.com/Heimeshoff).

🏷️ **Tags**: architecture event-sourcing domain-driven-design event-driven-design semantic-code domain-modelling

## Domain

"Cinemarco's Quality Reservation System" (CQRS 😛) : hundred movie theaters in Europe. COVID -> how to improve experience: digitalize?

## Command and Command Handlers exercise

☝️ Instructions: use TDD, use Command as input, YAGNI

💡 Tips: Get momentum going and start building everything in one file and only when it gets a bit messy, extract classes into files and folders. Here is a nice video to elaborate: [Elm Europe 2017 - Evan Czaplicki - The life of a file](https://www.youtube.com/watch?v=XpDsk374LDE).

Tasks:

1. A Customer reserves specific seats at a specific screening (for simplicity, assume there exists only one screening for the time being). If available, the seats should be reserved.
2. The user will be informed, if not all seats from the reservation are available.
3. Remove all primitive data types from the domain. Use only Value Objects and Entities within the domain.
4. Bonus points: Make illegal states un-representable

## Event Sourcing exercise

Tasks:

1. Refactor the previous test so that the command handlers use EventSourcing as a persistance mechanism. Check Assertions on the state.
2. Publish the Events and test purely with Events and Commands, no dependency on domain state in your tests is allowed.
3. If no booking happens within 12 minutes, the reservation is cancelled.
4. Reservation is only possible up to 15 minutes before the screening.
5. The reservation system calculates the  total price of the reserved tickets
6. Additional behavior: When a second customer tries to reserve already reserved seats, the system treats them as unavailable
