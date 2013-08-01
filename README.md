# Welcome to state.cs

The current stable release is 2.0.1.

## Version 2 notes - please read before downloading
Version 2 breaks apart the state machine model and the state machine state. This facilitates creating a single state machine model and using it with many different state machine states without any overhead of resetting the state machine, serializing/deserializing state or rebuilding the machine.

###API changes
The following are breaking API changes from version 1.5.x:

The old State class is replaced by the new SimpleState and ComplexState classes. A simple state is a leaf-level state in a state machine hierarchy whereas a complex state can have child regions. This removes minor overheads for simple states.

State machine state is represented by the new IState interface, a default implementation of which is found in the new State class.

The implementaion of the visitor pattern has been removed, as the state machine state is now under your direct control.

## Introduction
State.js provides a hierarchical state machine capable of managing orthogonal regions; a variety of pseudo state kinds are implemented including initial, shallow & deep history, choice, junction and entry & exit points.

State.cs is a C# implementation of a state machine library that largely follows UML 2 state machine semantics. Given the need to make this an executable model, there are certain features that are not supported, please read [UML compliance](https://github.com/steelbreeze/state.cs/wiki/UML Compliance) for more information.

## Versioning
The versions are in the form {major}.{minor}.{build}
* Major changes introduce significant new behaviour and will update the [public API](https://github.com/steelbreeze/state.cs/wiki/Steelbreeze.Behavior-Namespace).
* Minor changes introduce features, bug fixes, etc, but note that they also may break the public API.
* Build changes can introduce features, though usually are fixes and performance enhancements; these will never break the public API.

## Documentation
Please see the [wiki](https://github.com/steelbreeze/state.cs/wiki) for documentation.

## Building state.cs
Simply add all the .cs files and the resources to an existing or new assembly. That's it...

## Usage
If you're using state.cs, please drop me a mail; I'd love to hear about how this is getting used...

## Licence
Copyright © 2013 Steelbreeze Limited.

State.cs is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.
[![githalytics.com alpha](https://cruel-carlota.pagodabox.com/837a719cc38ffa18e895dc5f8f72768e "githalytics.com")](http://githalytics.com/steelbreeze/state.cs)
