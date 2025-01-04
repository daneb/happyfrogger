---
title: "C# Type System"
date: 2021-09-21 10:01:22 +0200
category: tech 
subcategory: csharp
description: Fundamentals
slug: type-system
---

![TypeSystem](/images/csharp_type_system.png)

**_Exploring the Terminology:_**

_C# 1's type system is static, explicit, and safe._

**Statically** typed is where each variable is of a particular type, and that type is known at compile time. In other words, the compiler does type checking at compilation time.

In the test method _StaticTypeWontCompile()_ you can see the string "Hello". But because we gave it the type Object, the compiler sees its type as Object and not String. Hence the _.Length_ method being unavailable. This test won't run because the compiler will state **object does not contain a definition for 'Length'**.

[gist:e0ed55842fdaa89527ace839d2abfe34]

**Explicitly Typed** is where the type of every variables must be explicitly stated. Of course, explicit vs implicit types is only relevant in statically typed languages.

[gist:85bd70dc0111f3663038e7e72eebe79b]

**Safe Type System** is the characteristic of code that allows the developer to be certain that a value or object will exhibit certain properties so that he/she can use it in a specific way without fear of unexpected or undefined behavior.

In the method _ThisIsSafe()_ the compiler will clearly state that an _Operator '+' cannot be applied to operands of type 'int' and 'bool'_. This safety applies for the majority of C# up until we get to Collections, Inheritance, Overriding and Interfaces as witnessed in method _ThisIsUnsafe_ which compiles perfectly.

[gist:c30ff2e9ad9c122ea12af3ba3532b159]

**Reference**

1. [C# In Depth - Jon Skeet](https://csharpindepth.com/)
2. [StackOverflow Why Is CSharp Statically Typed](https://stackoverflow.com/questions/859186/why-is-c-sharp-statically-typed)
3. [StackOverflow What is Type Safey in .NET](https://stackoverflow.com/questions/2437469/what-is-type-safe-in-net)
4. [C#s Type System - Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/)
