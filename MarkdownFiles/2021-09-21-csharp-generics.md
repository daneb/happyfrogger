---
title: "Quick Tour Of Generics In C#"
date: 2021-09-21 10:25:22 +0200
description: Fundamentals
category: tech
subcategory: csharp
slug: quick-tour-generics
---

![Generics](/images/csharp.jpg) 

Generics have been around since C# 2.0 and has become a tool we leverage so naturally now in C# we almost don't even think about it. So let's change that...

### Definitions

_Generics introduce to the .NET Framework the concept of type parameters, which make it possible to design classes and methods that defer the specification of one or more types until the class or method is declared and instantiated by client code_.

Essentially generics allow you parameterize types and methods. Just as normal methods have _parameters_ to tell them what _values_ to use, generic types and methods have _type parameters_ to tell them what _types_ to use.

### Clarification

Type parameters are placeholders for a type.

There are _constructed types_ and _generic types_.
A constructed type is when the type arguments are specified. For example,

```
Dictionary<int, string>()
```

A generic type is when the type arguments are not specified. For example,

```
Dictionary<TKey, TValue>()
```

As per the code snippet below, writing a method to swap two _Integer_ values is pretty rudimentary. But what if later we needed to do the very same but with _Strings_. This would result in two methods, or perhaps overloads.

[gist:13124e5ec2369117aff8fc27a3a861e5]

Generics to the rescue. We've isolated the type as a form of abstraction (`T`) to act as a placeholder for the type we intend to use.

[gist:9ab19f0a644a3e79525f932e4e34cd5f]

When the method is actually called, that placeholder is replaced with the type of the values used.

[gist:f318890ecb9849ced43bccfe86744093]

The value of Generics here is that it grants you _productivity_ improvements, _expressiveness_ and moves some safety concerns from _execution time to compile time_.

Note the two ways in which the _method_ can be called. One with the type explicit, and the other implicit through _type inference_. The compiler is inferring the _type parameters_ based on the method arguments you pass.

A _generic_ is simply a placeholder for a type.
