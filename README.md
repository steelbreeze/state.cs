# Welcome to state.cs

The current stable release is 3.5.4.

If you're using state.cs I'd love to hear about it; please e-mail me at mesmo@steelbreeze.net 

Version 3.3 brings a few small API changes: removed CompositeState.Initialise as it was not intended to be the root state machine; use Region instead; FinalState and PseudoState are now sealed; IState now takes an Element rather than Object as the element type.

## Version 3 notes - please read before downloading
Version 3 has focused on creating a clean public and protected API; there should be no implementation details visible in the namespace.

I have endevoured to keep the API as similar as possible to the version 2, but with a couple of notable changes:
* PseudoStateKind has changed to an enum (its methods are now internal extension methods).
* EntryPoint and ExitPoint pseudo states have been removed as they weren't really adding anything that can't be done with a choice or initial.
* Other base classes or interfaces have become internal only.
* FinalState now inherits from SimpleState (as per UML); note that use of the Entry and Exit actions will cause a compiler error (if anyone can help me find a way to hide them I'd love to hear about it).
* 

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
