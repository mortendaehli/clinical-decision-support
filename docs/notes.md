# Notes and questions

- Do we care about individual vital sign measurements, or just complete News-scoring schemas? For simplicity, we can
    start with the latter and consider adding more granular endpoints later if needed.
- News score is actually a form where you fill in different ranges for each vital sign, not the vital sign values
  themselves. This has been simplified in this case, so it does not reflect the real world per se.

## Casing
- The case defines the measurement types as upper-case TEMP, HR and RR. this is not a problem, but it is worth noting
    that in the real world we would likely want to write it out in full (e.g. "temperature", "heart_rate", "respiratory_rate")
    for clarity and to avoid confusion. However, for the sake of simplicity and brevity in this example, we can stick with the strict upper-case abbreviations.

## Async handler signatures
- We use async handlers in order to be ready for future extensions such as outbox pattern and database persistence.
