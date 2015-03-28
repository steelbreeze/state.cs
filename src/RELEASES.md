# Release notes

# Version 5.0.0
Version 5.0 is a complete re-write of the state machine. It makes large use of multicast delegates to pre-evaulate the steps required to perform state transitions. This leads to considerable performance benefits no evaluations are made relating to the structure of the model at runtime as it does not change at runtime.

# Version 5.2.0
Renamed IContext to IActiveStateConfiguration: this is more in line with the terminology in the UML specification.

Renamed Context to StateMachineInstance and ContextBase to StateMachineInstanceBase.

Removed XContext and XContextBase. While an interesting example of another type of state machine instance information management, it wasn't fully serializable so of little interest going forward.

Added Create... extension methods to main classes to faciltate simpler model building. This removes the need for many template parameters in model building.

Changed StateMachine to inherit from State. This is intented for a potential future enhancement to allow one state machine to be used within another.

Implemented a Visitor patten with the introcuction of the Visitor class and Accept methods on the Transition and all Element classes.

Moved the bootstrapping code to use the visitor and refactored a little.

Migrated array-based code to use HashSets for simpler, cleaner code.
