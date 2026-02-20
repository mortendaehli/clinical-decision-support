# Implement a General Clinical Decision Support Service for Scoring Algorithms

## Status

Accepted

## Context and Problem Statement

Nursing home patients are at considerable risk for rapid deterioration. To identify important changes in condition as
soon as possible, workers perform physical assessments at regular intervals to determine when extra monitoring is
required to keep a patient stable. During an assessment, several measurements are feed into an algorithm that produces
a single numeric value called a NEWS score. Our assignment is to create an API endpoint that accepts a defined set of
physical measurements and returns the appropriate NEWS score for the values given.

However, we need to take a step back and think about what this actually means in a clinical setting and the clinical
domain in general. The NEWS score is just a single heuristic that is used. This is a very common pattern in healthcare,
where there are many different heuristics that are used to make decisions. Other examples include the NEWS2 score, the
CHESS scale (which is a frailty score used in long-term care), and the APACHE score (which is used in intensive care).
Each of these heuristics is based on a set of measurements and a set of rules for how to calculate the score.

These are algorithms that are used to triage, prioritize, monitor, and make decisions about patient care. As such,
they are part of what we call Clinical Decision Support (CDS). When used to triage, prioritize, and monitor patients,
this software quickly becomes a Medical Device (Software as a Medical Device - SaMD) according to regulations like the
MDR. This means that we need to be very careful about how we design, implement, and deploy this software, and we need
to be highly disciplined about how we validate and verify it.

For the scope of this case, we will not focus heavily on the regulatory compliance aspects, but we must keep this
context in mind as we design and implement the foundational architecture.

## Considered Options

1. **Option 1:** A simple, static NEWS scoring service (hardcoded specifically for NEWS).
2. **Option 2:** A more general clinical decision support service that can support multiple heuristics and rule sets.
3. **Option 3:** A full-fledged, event-driven clinical decision support engine that can support complex rules, multiple heuristics, and real-time asynchronous monitoring.

## Decision Outcome

**Chosen option: Option 2 (A more general clinical decision support service that can support multiple heuristics).**

Given the obvious time restrictions when solving a simple case interview (with 45 minutes allocated for the presentation
and discussion of the technical solution ), we have to limit the scope of our implementation. However, we want to design
a solution that reflects professional expertise and thought-processes, ensuring the codebase is extensible and can be
easily adapted to support more complex use cases in the future.

Therefore, we will choose Option 2. This allows us to keep the focus of the assignment on the endpoint and the
underlying calculation service, while demonstrating how to build a generalized domain model that treats NEWS as just
one of many possible clinical calculation strategies.

### Pros and Cons of the Options

#### Option 1: Simple static NEWS scoring service

* **Good, because** it is the fastest to implement and directly satisfies the minimum requirements of the assignment.
* **Bad, because** it tightly couples the API and business logic to one specific algorithm, violating the Open-Closed Principle.
* **Bad, because** adding new clinical scores in the future would require modifying the core logic, increasing regression risks and complicating MDR auditing.

#### Option 2: General CDS service supporting multiple heuristics (Chosen)

* **Good, because** it utilizes Domain-Driven Design (DDD) to separate the rule engine/strategy from the API transport layer.
* **Good, because** new algorithms can be added simply by implementing a new strategy or configuration class without altering the core calculation engine.
**Good, because** it facilitates strict unit testing of boundary conditions (e.g., ensuring all starting values are exclusive and ending values are inclusive ) in isolation.


* **Bad, because** it requires slightly more initial boilerplate (interfaces, factories, or strategy patterns) than a naive script.

#### Option 3: Full-fledged event-driven CDS engine

* **Good, because** it represents the ideal target architecture for a scalable, real-time clinical system (e.g., subscribing to FHIR Observation events).
* **Good, because** it fully decouples data ingestion from clinical scoring calculations, allowing independent scaling.
* **Bad, because** it is too complex to fully implement and demo effectively within the constraints of a standard case assignment. *(Note: We will discuss this as our target architectural vision for Phase 2 during the interview presentation).*
