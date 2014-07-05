# Welcome to state.cs

The current stable release is 4.1.1.

If you're using state.cs I'd love to hear about it; please e-mail me at mesmo@steelbreeze.net 

## Version 4.1 notes
Version 4.1 also passes the state machine state through to the transition guard conditions.

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

## The MIT License (MIT)
Copyright (c) 2014 Steelbreeze Limited

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

[![githalytics.com alpha](https://cruel-carlota.pagodabox.com/837a719cc38ffa18e895dc5f8f72768e "githalytics.com")](http://githalytics.com/steelbreeze/state.cs)
