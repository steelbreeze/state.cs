# Welcome to state.cs

The current stable release is 4.0.0.

If you're using state.cs I'd love to hear about it; please e-mail me at mesmo@steelbreeze.net 

## Version 4 notes
Version 4 passes the state machine state through to the Entry, Exit and Effect callbacks; it also preserves the full type of the state machine state (the subclass of IState).

In order to do this, all element classes have had to be parameterised; in order to keep code cruft to a minimum, helper classes have been added to each parent element class (StateMachine, Region, CompositeState and OrthogonalState) to create child elements and transitions.

For conveniance, there is a reference StateBase abstract implementation of IState; you only need to inherit from it and pass the inherited class at the template parameter TState.

Also taken the opportunity of a major version upgrade to fix some naming inconsistancies.

## Introduction
State.js provides a hierarchical state machine capable of managing orthogonal regions; a variety of pseudo state kinds are implemented including initial, shallow & deep history, choice, junction and entry & exit points.

State.cs is a C# implementation of a state machine library that largely follows UML 2 state machine semantics. Given the need to make this an executable model, there are certain features that are not supported, please read [UML compliance](https://github.com/steelbreeze/state.cs/wiki/UML Compliance) for more information.

## Versioning
The versions are in the form {major}.{minor}.{build}
* Major changes introduce significant new behaviour and will update the [public API](http://www.steelbreeze.net/state.cs/API.pdf).
* Minor changes introduce features, bug fixes, etc, but note that they also may break the public API.
* Build changes can introduce features, though usually are fixes and performance enhancements; these will never break the public API.

## Documentation
Please see the [API documentation](http://www.steelbreeze.net/state.cs/API.pdf) for documentation.

## Building state.cs
Simply add all the .cs files and the resources to an existing or new assembly. That's it...

## Usage
If you're using state.cs, please drop me a mail; I'd love to hear about how this is getting used...

## Licence
Copyright © 2013-4 Steelbreeze Limited.

State.cs is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.
[![githalytics.com alpha](https://cruel-carlota.pagodabox.com/837a719cc38ffa18e895dc5f8f72768e "githalytics.com")](http://githalytics.com/steelbreeze/state.cs)
